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
        public void Initialize_ArchiveMetadataWithMissingContent() {
            var folder1 = "Folder1";
            var folder2 = "Folder2";
            var folder1A = "Folder1A";
            var metadataToArchive = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { folder1, folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentServiceContainer(Enumerable.Empty<ContentMetadata>(), metadataToArchive, Enumerable.Empty<string>());
            container.RunTest();
        }

        [Test]
        [Category("Unit Tests")]
        public void Initialize_ShouldResolveExistingMetadata() {
            var folder1 = "Folder1";
            var folder2 = "Folder2";
            var folder1A = "Folder1A";
            var existing = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { folder1, folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentServiceContainer(existing, Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<string>());
            container.RunTest();
        }
    }
}