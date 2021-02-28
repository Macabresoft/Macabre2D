namespace Macabresoft.Macabre2D.Tests.Editor.Library.Services {
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Editor.Library.Models.Content;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public sealed class ContentServiceTests {
        private const string BinDirectoryName = "bin";
        private const string ContentDirectoryName = "Content";
        private const string ContentFileName = "Content.mgcb";
        private const string LeagueMonoXnbName = "League Mono.xnb";
        private const string PlatformName = "DesktopGL";
        private const string SkullXnbName = "skull.xnb";

        [Test]
        [Category("Integration Tests")]
        public void Build_ShouldRunMGCB() {
            var service = new ContentService(new FileSystemService(), new Serializer());
            var contentDirectory = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                PathHelper.GetPathToAncestorDirectory(3),
                ContentDirectoryName);
            var contentFile = Path.Combine(contentDirectory, ContentFileName);
            var binDirectory = Path.Combine(contentDirectory, BinDirectoryName);
            var buildContentDirectory = Path.Combine(binDirectory, PlatformName);
            var skullFilePath = Path.Combine(buildContentDirectory, SkullXnbName);
            var leagueMonoFilePath = Path.Combine(buildContentDirectory, LeagueMonoXnbName);

            if (Directory.Exists(binDirectory)) {
                Directory.Delete(binDirectory, true);
            }

            using (new AssertionScope()) {
                service.Build(new BuildContentArguments(contentFile, PlatformName, false)).Should().Be(0);
                File.Exists(skullFilePath).Should().BeTrue();
                File.Exists(leagueMonoFilePath).Should().BeTrue();
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void Initialize_ShouldResolveExistingMetadata() {
            var assetManager = Substitute.For<IAssetManager>();
            var serializer = Substitute.For<ISerializer>();
            var fileSystemService = Substitute.For<IFileSystemService>();
            var rootPath = "Content";
            var metadataPath = Path.Combine(rootPath, ContentMetadata.MetadataDirectoryName);
            var archivePath = Path.Combine(rootPath, ContentMetadata.ArchiveDirectoryName);

            var contentFileName = Guid.NewGuid().ToString();
            var spriteSheet = new SpriteSheet();
            var metadata = new ContentMetadata(spriteSheet, new[] { contentFileName }, ".jpg");
            var metadataFileName = $"{spriteSheet.ContentId.ToString()}{ContentMetadata.FileExtension}";
            var metadataFilePath = Path.Combine(metadataPath, metadataFileName);
            var contentFilePath = Path.Combine(rootPath, metadata.GetFileName());

            fileSystemService.GetDirectories(rootPath).Returns(Enumerable.Empty<string>());
            fileSystemService.GetFiles(metadataPath, ContentMetadata.MetadataSearchPattern).Returns(new[] { metadataFilePath });
            fileSystemService.DoesFileExist(contentFilePath).Returns(true);
            fileSystemService.DoesDirectoryExist(rootPath).Returns(true);
            fileSystemService.DoesDirectoryExist(metadataPath).Returns(true);
            fileSystemService.DoesDirectoryExist(archivePath).Returns(true);
            serializer.Deserialize<ContentMetadata>(metadataFilePath).Returns(metadata);

            var contentService = new ContentService(fileSystemService, serializer);
            contentService.Initialize(rootPath, assetManager);

            using (new AssertionScope()) {
                assetManager.Received().RegisterMetadata(metadata);
                contentService.RootContentDirectory.Children.Count.Should().Be(1);

                var contentFile = contentService.RootContentDirectory.Children.First();
                contentFile.NameWithoutExtension.Should().Be(metadata.GetFileNameWithoutExtension());
                contentFile.Name.Should().Be(metadata.GetFileName());
                contentFile.GetContentPath().Should().Be(metadata.GetFileNameWithoutExtension());
                contentFile.GetFullPath().Should().Be(contentFilePath);
            }
        }
    }
}