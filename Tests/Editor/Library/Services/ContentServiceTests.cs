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
        private const string Folder1 = "Folder1";
        private const string Folder2 = "Folder2";
        private const string Folder1A = "Folder1A";
        private const string BinDirectoryName = "bin";
        private const string ContentFileName = "Content.mgcb";
        private const string LeagueMonoXnbName = "League Mono.xnb";
        private const string PlatformName = "DesktopGL";
        private const string SkullXnbName = "skull.xnb";

        [Test]
        [Category("Integration Tests")]
        public void Build_ShouldRunMGCB() {
            var service = new ProjectService(new FileSystemService(), Substitute.For<ISceneService>(), new Serializer(), new UndoService());
            var contentDirectory = Path.Combine(
                TestContext.CurrentContext.TestDirectory,
                PathHelper.GetPathToAncestorDirectory(3),
                ProjectService.ContentDirectory);
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
        public void MoveContent_ShouldMoveFile() {
            var metadata = new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder1, Guid.NewGuid().ToString() }, ".jpg");
            var existing = new[] {
                metadata,
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder2, Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentContainer(existing, Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<string>());
            container.RunInitializeTest();

            var contentFile = container.Instance.RootContentDirectory.FindNode(metadata.SplitContentPath.ToArray());
            var originalPath = contentFile.GetFullPath();
            var secondFolder = container.Instance.RootContentDirectory.FindNode(new[] { ProjectService.ContentDirectory, Folder2 }) as IContentDirectory;
            container.Instance.MoveContent(contentFile, secondFolder);
            var newPath = contentFile.GetFullPath();

            using (new AssertionScope()) {
                newPath.Should().NotBe(originalPath);
                container.FileSystem.Received().MoveFile(originalPath, newPath);
                contentFile.Parent.Should().Be(secondFolder);
                // ReSharper disable once PossibleNullReferenceException
                secondFolder.Children.Should().Contain(contentFile);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void MoveContent_ShouldMoveDirectory() {
            var metadata = new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder1, Guid.NewGuid().ToString() }, ".jpg");
            var existing = new[] {
                metadata,
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder2, Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentContainer(existing, Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<string>());
            container.RunInitializeTest();

            var directory1 = container.Instance.RootContentDirectory.FindNode(new[] { ProjectService.ContentDirectory, Folder1 });
            var originalPath = directory1.GetFullPath();
            var directory2 = container.Instance.RootContentDirectory.FindNode(new[] { ProjectService.ContentDirectory, Folder2 }) as IContentDirectory;
            container.Instance.MoveContent(directory1, directory2);
            var newPath = directory1.GetFullPath();

            using (new AssertionScope()) {
                newPath.Should().NotBe(originalPath);
                container.FileSystem.Received().MoveDirectory(originalPath, newPath);
                directory1.Parent.Should().Be(directory2);
                // ReSharper disable once PossibleNullReferenceException
                directory2.Children.Should().Contain(directory1);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void Initialize_Should_ArchiveMetadataWithMissingContent() {
            var metadataToArchive = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder1, Folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentContainer(Enumerable.Empty<ContentMetadata>(), metadataToArchive, Enumerable.Empty<string>());
            container.RunInitializeTest();
        }

        [Test]
        [Category("Unit Tests")]
        public void Initialize_Should_CreateMetadataForNewContent() {
            var newContentFiles = new[] {
                Path.Combine(ProjectService.ContentDirectory, Folder1, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(ProjectService.ContentDirectory, Folder2, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(ProjectService.ContentDirectory, Folder1, Folder1A, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(ProjectService.ContentDirectory, $"{Guid.NewGuid()}.jpg")
            };

            var container = new ContentContainer(Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<ContentMetadata>(), newContentFiles);
            container.RunInitializeTest();
        }

        [Test]
        [Category("Unit Tests")]
        public void Initialize_Should_ResolveExistingMetadata() {
            var existing = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder1, Folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentContainer(existing, Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<string>());
            container.RunInitializeTest();
        }

        [Test]
        [Category("Unit Tests")]
        public void Initialize_Should_HandleAComplexSituation() {
            var existing = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder1, Folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Guid.NewGuid().ToString() }, ".jpg")
            };

            var metadataToArchive = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Folder1, Folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ProjectService.ContentDirectory, Guid.NewGuid().ToString() }, ".jpg")
            };

            var newContentFiles = new[] {
                Path.Combine(ProjectService.ContentDirectory, Folder1, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(ProjectService.ContentDirectory, Folder2, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(ProjectService.ContentDirectory, Folder1, Folder1A, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(ProjectService.ContentDirectory, $"{Guid.NewGuid()}.jpg")
            };

            var container = new ContentContainer(existing, metadataToArchive, newContentFiles);
            container.RunInitializeTest();
        }
    }
}