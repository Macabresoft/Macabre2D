namespace Macabresoft.Macabre2D.Tests.UI.Common.Services {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Macabresoft.Macabre2D.Framework;
    using NSubstitute;

    public class ContentContainer {
        public const string Folder1 = "Folder1";
        public const string Folder2 = "Folder2";
        public const string Folder1A = "Folder1A";
        private const string BinPath = "bin";
        private const string PlatformsPath = "Platforms";

        private readonly IList<string> _metadataFilePaths = new List<string>();
        private readonly IPathService _pathService = new PathService(BinPath, PlatformsPath, PathService.ContentDirectoryName);
        private readonly IDictionary<string, HashSet<string>> _pathToDirectoryList = new Dictionary<string, HashSet<string>>();
        private readonly IDictionary<string, HashSet<string>> _pathToFileList = new Dictionary<string, HashSet<string>>();

        public ContentContainer(
            IEnumerable<ContentMetadata> existingMetadata,
            IEnumerable<ContentMetadata> metadataToArchive,
            IEnumerable<string> newContentFiles) {
            this.ExistingMetadata = existingMetadata.ToArray();
            this.MetadataToArchive = metadataToArchive.ToArray();
            this.NewContentFiles = newContentFiles.ToArray();

            this.FileSystem.DoesDirectoryExist(this._pathService.PlatformsDirectoryPath).Returns(true);
            this.FileSystem.DoesDirectoryExist(this._pathService.ContentDirectoryPath).Returns(true);
            this.FileSystem.DoesDirectoryExist(this._pathService.MetadataDirectoryPath).Returns(true);
            this.FileSystem.DoesDirectoryExist(this._pathService.MetadataArchiveDirectoryPath).Returns(true);

            var projectFilePath = Path.Combine(this._pathService.ContentDirectoryPath, GameProject.ProjectFileName);
            this.FileSystem.DoesFileExist(projectFilePath).Returns(true);

            var project = new GameProject(this.GameSettings, GameProject.DefaultProjectName, Guid.Empty);
            this.Serializer.Deserialize<GameProject>(projectFilePath).Returns(project);

            this.SetupExistingMetadata();
            this.SetupMetadataToArchive();
            this.SetupNewContentFiles();

            this.FileSystem.GetFiles(this._pathService.MetadataDirectoryPath, ContentMetadata.MetadataSearchPattern).Returns(this._metadataFilePaths);
            this.Instance = new ContentService(
                Substitute.For<IAssemblyService>(),
                this.AssetManager,
                Substitute.For<IBuildService>(),
                this.FileSystem,
                Substitute.For<ILoggingService>(),
                this._pathService,
                this.Serializer,
                Substitute.For<IUndoService>(),
                Substitute.For<IValueEditorService>());
        }

        public IAssetManager AssetManager { get; } = Substitute.For<IAssetManager>();

        public IReadOnlyCollection<ContentMetadata> ExistingMetadata { get; }

        public IFileSystemService FileSystem { get; } = Substitute.For<IFileSystemService>();

        public IGameSettings GameSettings { get; } = Substitute.For<IGameSettings>();

        public IContentService Instance { get; }

        public IReadOnlyCollection<ContentMetadata> MetadataToArchive { get; }

        public IReadOnlyCollection<string> NewContentFiles { get; }
        
        public ISerializer Serializer { get; } = Substitute.For<ISerializer>();

        public void RunRefreshContentTest() {
            this.Instance.RefreshContent();

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

        private void AssertExistingMetadata() {
            foreach (var metadata in this.ExistingMetadata) {
                this.AssetManager.Received().RegisterMetadata(metadata);

                var contentFile = this.Instance.RootContentDirectory.FindNode(metadata.SplitContentPath.ToArray());
                contentFile.Should().NotBeNull();
                contentFile.NameWithoutExtension.Should().Be(metadata.GetFileNameWithoutExtension());
                contentFile.Name.Should().Be(metadata.GetFileName());
                contentFile.GetContentPath().Should().Be(metadata.GetContentPath());
                contentFile.GetFullPath().Should().Be(Path.Combine(this._pathService.ContentDirectoryPath, $"{metadata.GetContentPath()}{metadata.ContentFileExtension}"));
            }
        }

        private void AssertMetadataToArchive() {
            foreach (var metadata in this.MetadataToArchive) {
                var current = Path.Combine(this._pathService.ContentDirectoryPath, ContentMetadata.GetMetadataPath(metadata.ContentId));
                var moveTo = Path.Combine(this._pathService.ContentDirectoryPath, ContentMetadata.GetArchivePath(metadata.ContentId));
                this.FileSystem.Received().MoveFile(current, moveTo);
                this.AssetManager.DidNotReceive().RegisterMetadata(metadata);
                this.Instance.RootContentDirectory.TryFindNode(metadata.SplitContentPath.ToArray(), out var node).Should().BeFalse();
                node.Should().BeNull();
            }
        }

        private void AssertNewContentFiles() {
            foreach (var file in this.NewContentFiles) {
                var splitContentPath = file.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries).ToList();
                splitContentPath[^1] = Path.GetFileNameWithoutExtension(splitContentPath[^1]);
                var contentFile = this.Instance.RootContentDirectory.FindNode(splitContentPath.ToArray()) as ContentFile;
                contentFile.Should().NotBeNull();

                if (contentFile != null) {
                    this.AssetManager.Received().RegisterMetadata(contentFile.Metadata);
                    this.Serializer.Received().Serialize(contentFile.Metadata, Path.Combine(this._pathService.ContentDirectoryPath, ContentMetadata.GetMetadataPath(contentFile.Metadata.ContentId)));
                }
            }
        }

        private void RegisterContent(ContentMetadata metadata, bool contentShouldExist) {
            var splitDirectoryPath = metadata.SplitContentPath.Take(metadata.SplitContentPath.Count - 1).ToList();
            splitDirectoryPath.Insert(0, PathService.ContentDirectoryName);
            var fileName = metadata.GetFileName();

            if (contentShouldExist) {
                var directoryPath = Path.Combine(splitDirectoryPath.ToArray());
                this.AddFileToDirectory(directoryPath, fileName);
            }
            else {
                splitDirectoryPath.Add(fileName);
                this.FileSystem.DoesFileExist(Path.Combine(splitDirectoryPath.ToArray())).Returns(false);
            }
        }

        private void SetupExistingMetadata() {
            foreach (var metadata in this.ExistingMetadata) {
                var metadataFilePath = Path.Combine(this._pathService.ContentDirectoryPath, ContentMetadata.GetMetadataPath(metadata.ContentId));
                this._metadataFilePaths.Add(metadataFilePath);

                this.Serializer.Deserialize<ContentMetadata>(metadataFilePath).Returns(metadata);
                this.RegisterContent(metadata, true);
            }
        }

        private void SetupMetadataToArchive() {
            foreach (var metadata in this.MetadataToArchive) {
                var metadataFilePath = Path.Combine(this._pathService.ContentDirectoryPath, ContentMetadata.GetMetadataPath(metadata.ContentId));
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
                splitDirectoryPath.Insert(0, PathService.ContentDirectoryName);
                this.AddFileToDirectory(Path.Combine(splitDirectoryPath.ToArray()), fileName);
                this.FileSystem.DoesFileExist(Path.Combine(this._pathService.ContentDirectoryPath, file)).Returns(true);
            }
        }
    }
}