namespace Macabresoft.Macabre2D.Tests.Editor.Library.Services {
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabre2D.Editor.Library.Services;
    using Macabre2D.Framework;
    using NSubstitute;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class ContentServiceContainer {
        private const string ContentPath = "Content";
        private static readonly string MetadataDirectoryPath = Path.Combine(ContentPath, ContentMetadata.MetadataDirectoryName);
        private static readonly string ArchiveDirectoryPath = Path.Combine(ContentPath, ContentMetadata.ArchiveDirectoryName);
        private readonly IList<string> _contentFilePaths = new List<string>();

        private readonly IList<string> _metadataFilePaths = new List<string>();

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
            this.FileSysstem.GetFiles(ContentPath).Returns(this._contentFilePaths);
            this.FileSysstem.GetDirectories(ContentPath).Returns(Enumerable.Empty<string>());

            this.ContentService = new ContentService(this.FileSysstem, this.Serializer);
        }

        public void RunTest() {
            this.ContentService.Initialize(ContentPath, this.AssetManager);

            using (new AssertionScope()) {
                foreach (var metadata in this.ExistingMetadata) {
                    this.AssetManager.Received().RegisterMetadata(metadata);

                    var contentFile = this.ContentService.RootContentDirectory.Children.First(x => x.Name == metadata.GetFileName());
                    contentFile.NameWithoutExtension.Should().Be(metadata.GetFileNameWithoutExtension());
                    contentFile.Name.Should().Be(metadata.GetFileName());
                    contentFile.GetContentPath().Should().Be(metadata.GetFileNameWithoutExtension());
                    contentFile.GetFullPath().Should().Be(Path.Combine(ContentPath, metadata.GetFileName()));
                }
            }
        }

        public IAssetManager AssetManager { get; } = Substitute.For<IAssetManager>();

        public IContentService ContentService { get; }

        public IReadOnlyCollection<ContentMetadata> ExistingMetadata { get; }

        public IReadOnlyCollection<ContentMetadata> MetadataToArchive { get; }

        public IReadOnlyCollection<string> NewContentFiles { get; }

        public IFileSystemService FileSysstem { get; } = Substitute.For<IFileSystemService>();

        public ISerializer Serializer { get; } = Substitute.For<ISerializer>();

        private void SetupExistingMetadata() {
            foreach (var metadata in this.ExistingMetadata) {
                var metadataFilePath = Path.Combine(MetadataDirectoryPath, metadata.GetFileName());
                var contentFilePath = Path.Combine(ContentPath, metadata.GetFileName());
                this._metadataFilePaths.Add(metadataFilePath);
                this._contentFilePaths.Add(contentFilePath);

                this.Serializer.Deserialize<ContentMetadata>(metadataFilePath).Returns(metadata);
                this.FileSysstem.DoesFileExist(metadataFilePath).Returns(true);
                this.FileSysstem.DoesFileExist(contentFilePath).Returns(true);
            }
        }

        private void SetupMetadataToArchive() {
            foreach (var metadata in this.MetadataToArchive) {
                var metadataFilePath = Path.Combine(MetadataDirectoryPath, metadata.GetFileName());
                var contentFilePath = Path.Combine(ContentPath, metadata.GetFileName());
                this._metadataFilePaths.Add(metadataFilePath);

                this.Serializer.Deserialize<ContentMetadata>(metadataFilePath).Returns(metadata);
                this.FileSysstem.DoesFileExist(metadataFilePath).Returns(true);
                this.FileSysstem.DoesFileExist(contentFilePath).Returns(false);
            }
        }

        private void SetupNewContentFiles() {
            foreach (var file in this.NewContentFiles) {
                this.FileSysstem.DoesFileExist(Path.Combine(ContentPath, file)).Returns(true);
            }
        }
    }
}