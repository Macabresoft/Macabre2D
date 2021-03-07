namespace Macabresoft.Macabre2D.Tests.Editor.Library.Services {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models.Content;
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

            this.FileSystem.DoesDirectoryExist(ContentPath).Returns(true);
            this.FileSystem.DoesDirectoryExist(MetadataDirectoryPath).Returns(true);
            this.FileSystem.DoesDirectoryExist(ArchiveDirectoryPath).Returns(true);

            this.SetupExistingMetadata();
            this.SetupMetadataToArchive();
            this.SetupNewContentFiles();

            this.FileSystem.GetFiles(MetadataDirectoryPath, ContentMetadata.MetadataSearchPattern).Returns(this._metadataFilePaths);
            this.ContentService = new ContentService(this.FileSystem, this.Serializer, new UndoService());
        }

        public IAssetManager AssetManager { get; } = Substitute.For<IAssetManager>();

        public IContentService ContentService { get; }

        public IReadOnlyCollection<ContentMetadata> ExistingMetadata { get; }

        public IFileSystemService FileSystem { get; } = Substitute.For<IFileSystemService>();

        public IReadOnlyCollection<ContentMetadata> MetadataToArchive { get; }

        public IReadOnlyCollection<string> NewContentFiles { get; }

        public ISerializer Serializer { get; } = Substitute.For<ISerializer>();

        public void RunInitializeTest() {
            this.ContentService.Initialize(ContentPath, this.AssetManager);

            using (new AssertionScope()) {
                this.AssertExistingMetadata();
                this.AssertMetadataToArchive();
                this.AssertNewContentFiles();
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
                this.FileSystem.GetDirectories(directoryPath).Returns(directories);
            }

            this.FileSystem.DoesDirectoryExist(newDirectoryPath).Returns(true);
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
                this.FileSystem.GetFiles(directoryPath).Returns(files);
            }

            this.FileSystem.DoesFileExist(filePath).Returns(true);
        }
        
        private void AssertNewContentFiles() {
            foreach (var file in this.NewContentFiles) {
                var splitContentPath = file.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList();
                splitContentPath[^1] = Path.GetFileNameWithoutExtension(splitContentPath[^1]);
                var contentFile = this.ContentService.RootContentDirectory.FindNode(splitContentPath.ToArray()) as ContentFile;
                contentFile.Should().NotBeNull();

                if (contentFile != null) {
                    this.AssetManager.Received().RegisterMetadata(contentFile.Metadata);
                    this.Serializer.Received().Serialize(contentFile.Metadata, Path.Combine(ContentPath, ContentMetadata.GetMetadataPath(contentFile.Metadata.ContentId)));
                }
            }
        }

        private void AssertExistingMetadata() {
            foreach (var metadata in this.ExistingMetadata) {
                this.AssetManager.Received().RegisterMetadata(metadata);

                var contentFile = this.ContentService.RootContentDirectory.FindNode(metadata.SplitContentPath.ToArray());
                contentFile.Should().NotBeNull();
                contentFile.NameWithoutExtension.Should().Be(metadata.GetFileNameWithoutExtension());
                contentFile.Name.Should().Be(metadata.GetFileName());
                contentFile.GetContentPath().Should().Be(metadata.GetContentPath());
                contentFile.GetFullPath().Should().Be(Path.Combine(ContentPath, $"{metadata.GetContentPath()}{Path.GetExtension(metadata.GetFileName())}"));
            }
        }

        private void AssertMetadataToArchive() {
            foreach (var metadata in this.MetadataToArchive) {
                var current = Path.Combine(ContentPath, ContentMetadata.GetMetadataPath(metadata.ContentId));
                var moveTo = Path.Combine(ContentPath, ContentMetadata.GetArchivePath(metadata.ContentId));
                this.FileSystem.Received().MoveFile(current, moveTo);
                this.AssetManager.DidNotReceive().RegisterMetadata(metadata);
                this.ContentService.RootContentDirectory.TryFindNode(metadata.SplitContentPath.ToArray(), out var node).Should().BeFalse();
                node.Should().BeNull();
            }
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
                this.FileSystem.DoesFileExist(Path.Combine(directorySplitPath.ToArray())).Returns(false);
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
                var splitPath = file.Split(Path.DirectorySeparatorChar);
                var fileName = splitPath.Last();
                var splitDirectoryPath = splitPath.Take(splitPath.Length - 1).ToList();
                splitDirectoryPath.Insert(0, ContentPath);
                this.AddFileToDirectory(Path.Combine(splitDirectoryPath.ToArray()), fileName);
                this.FileSystem.DoesFileExist(Path.Combine(ContentPath, file)).Returns(true);
            }
        }
    }
}