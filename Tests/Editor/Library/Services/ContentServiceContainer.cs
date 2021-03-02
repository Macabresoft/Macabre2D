namespace Macabresoft.Macabre2D.Tests.Editor.Library.Services {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using NSubstitute;

    public class ContentServiceContainer {
        private const string ContentPath = "Content";
        private static readonly string MetadataDirectoryPath = Path.Combine(ContentPath, ContentMetadata.MetadataDirectoryName);
        private static readonly string ArchiveDirectoryPath = Path.Combine(ContentPath, ContentMetadata.ArchiveDirectoryName);


        private readonly IList<string> _metadataFilePaths = new List<string>();
        private readonly IDictionary<string, HashSet<string>> _pathToDirectoryList = new Dictionary<string, HashSet<string>>();
        private readonly IDictionary<string, HashSet<string>> _pathToFileList = new Dictionary<string, HashSet<string>>();

        public ContentServiceContainer(
            IEnumerable<ContentMetadata> existingMetadata,
            IEnumerable<ContentMetadata> metadataToArchive,
            IEnumerable<string> newContentFiles) {
            this.ExistingMetadata = existingMetadata.ToArray();
            this.MetadataToArchive = metadataToArchive.ToArray();
            this.NewContentFiles = newContentFiles.ToArray();

            this.FileSysstem.DoesDirectoryExist(ContentPath).Returns(true);
            this.FileSysstem.DoesDirectoryExist(MetadataDirectoryPath).Returns(true);
            this.FileSysstem.DoesDirectoryExist(ArchiveDirectoryPath).Returns(true);

            this.SetupExistingMetadata();
            this.SetupMetadataToArchive();
            this.SetupNewContentFiles();

            this.FileSysstem.GetFiles(MetadataDirectoryPath, ContentMetadata.MetadataSearchPattern).Returns(this._metadataFilePaths);
            this.ContentService = new ContentService(this.FileSysstem, this.Serializer);
        }

        public IAssetManager AssetManager { get; } = Substitute.For<IAssetManager>();

        public IContentService ContentService { get; }

        public IReadOnlyCollection<ContentMetadata> ExistingMetadata { get; }

        public IFileSystemService FileSysstem { get; } = Substitute.For<IFileSystemService>();

        public IReadOnlyCollection<ContentMetadata> MetadataToArchive { get; }

        public IReadOnlyCollection<string> NewContentFiles { get; }

        public ISerializer Serializer { get; } = Substitute.For<ISerializer>();

        public void RunTest() {
            this.ContentService.Initialize(ContentPath, this.AssetManager);

            using (new AssertionScope()) {
                foreach (var metadata in this.ExistingMetadata) {
                    this.AssetManager.Received().RegisterMetadata(metadata);

                    var contentFile = this.ContentService.RootContentDirectory.FindNode(metadata.SplitContentPath.ToArray());
                    contentFile.NameWithoutExtension.Should().Be(metadata.GetFileNameWithoutExtension());
                    contentFile.Name.Should().Be(metadata.GetFileName());
                    contentFile.GetContentPath().Should().Be(metadata.GetContentPath());
                    contentFile.GetFullPath().Should().Be(Path.Combine(ContentPath, $"{metadata.GetContentPath()}{Path.GetExtension(metadata.GetFileName())}"));
                }
            }
        }

        private void AddDirectoryToDirectory(string directoryPath, string newDirectoryName) {
            var newDirectoryPath = Path.Combine(directoryPath, newDirectoryName);
            if (this._pathToDirectoryList.TryGetValue(directoryPath, out var directories)) {
                directories.Add(newDirectoryPath);
            }
            else {
                directories = new HashSet<string> { newDirectoryPath };
                this._pathToDirectoryList[directoryPath] = directories;
                this.FileSysstem.GetDirectories(directoryPath).Returns(directories);
            }

            this.FileSysstem.DoesDirectoryExist(newDirectoryPath).Returns(true);
        }

        private void AddFileToDirectory(string directoryPath, string fileName) {
            var splitDirectoryPath = directoryPath.Split(Path.DirectorySeparatorChar);
            if (splitDirectoryPath.Length > 1) {
                for (var i = 1; i < splitDirectoryPath.Length; i++) {
                    this.AddDirectoryToDirectory(Path.Combine(splitDirectoryPath.Take(i).ToArray()), splitDirectoryPath[i]);
                }
            }

            var filePath = Path.Combine(directoryPath, fileName);
            if (this._pathToFileList.TryGetValue(directoryPath, out var files)) {
                files.Add(filePath);
            }
            else {
                files = new HashSet<string> { filePath };
                this._pathToFileList[directoryPath] = files;
                this.FileSysstem.GetFiles(directoryPath).Returns(files);
            }

            this.FileSysstem.DoesFileExist(filePath).Returns(true);
        }

        private void RegisterContent(ContentMetadata metadata, bool contentShouldExist) {
            var directorySplitPath = metadata.SplitContentPath.Take(metadata.SplitContentPath.Count - 1).ToList();
            directorySplitPath.Insert(0, ContentPath);
            var fileName = metadata.GetFileName();

            if (contentShouldExist) {
                var directoryPath = Path.Combine(directorySplitPath.ToArray());
                this.AddFileToDirectory(directoryPath, fileName);
            }
            else {
                directorySplitPath.Add(fileName);
                this.FileSysstem.DoesFileExist(Path.Combine(directorySplitPath.ToArray())).Returns(false);
            }
        }

        private void SetupExistingMetadata() {
            foreach (var metadata in this.ExistingMetadata) {
                var metadataFilePath = Path.Combine(MetadataDirectoryPath, $"{metadata.ContentId.ToString()}{ContentMetadata.FileExtension}");
                this._metadataFilePaths.Add(metadataFilePath);

                this.Serializer.Deserialize<ContentMetadata>(metadataFilePath).Returns(metadata);
                this.RegisterContent(metadata, true);
            }
        }

        private void SetupMetadataToArchive() {
            foreach (var metadata in this.MetadataToArchive) {
                var metadataFilePath = Path.Combine(MetadataDirectoryPath, metadata.GetFileName());
                this._metadataFilePaths.Add(metadataFilePath);

                this.Serializer.Deserialize<ContentMetadata>(metadataFilePath).Returns(metadata);
                this.RegisterContent(metadata, false);
            }
        }

        private void SetupNewContentFiles() {
            foreach (var file in this.NewContentFiles) {
                this.FileSysstem.DoesFileExist(Path.Combine(ContentPath, file)).Returns(true);
            }
        }
    }
}