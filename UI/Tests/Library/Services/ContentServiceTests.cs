namespace Macabresoft.Macabre2D.Tests.Editor.Library.Services {
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ContentServiceTests {
        [Test]
        [Category("Unit Tests")]
        public void MoveContent_ShouldMoveDirectory() {
            var metadata = new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder1, Guid.NewGuid().ToString() }, ".jpg");
            var existing = new[] {
                metadata,
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder2, Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentContainer(existing, Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<string>());
            container.RunRefreshContentTest();

            var directory1 = container.Instance.RootContentDirectory.FindNode(new[] { ContentContainer.Folder1 });
            var originalPath = directory1.GetFullPath();
            var directory2 = container.Instance.RootContentDirectory.FindNode(new[] { ContentContainer.Folder2 }) as IContentDirectory;
            container.Instance.MoveContent(directory1, directory2);
            var newPath = directory1.GetFullPath();

            using (new AssertionScope()) {
                newPath.Should().NotBe(originalPath);
                container.FileSystem.Received().MoveDirectory(originalPath, newPath);
                directory1.Parent.Should().Be(directory2);
                directory2.Children.Should().Contain(directory1);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void MoveContent_ShouldMoveFile() {
            var metadata = new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder1, Guid.NewGuid().ToString() }, ".jpg");
            var existing = new[] {
                metadata,
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder2, Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentContainer(existing, Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<string>());
            container.RunRefreshContentTest();

            var contentFile = container.Instance.RootContentDirectory.FindNode(metadata.SplitContentPath.ToArray());
            var originalPath = contentFile.GetFullPath();
            var secondFolder = container.Instance.RootContentDirectory.FindNode(new[] { ContentContainer.Folder2 }) as IContentDirectory;
            container.Instance.MoveContent(contentFile, secondFolder);
            var newPath = contentFile.GetFullPath();

            using (new AssertionScope()) {
                newPath.Should().NotBe(originalPath);
                container.FileSystem.Received().MoveFile(originalPath, newPath);
                contentFile.Parent.Should().Be(secondFolder);
                secondFolder.Children.Should().Contain(contentFile);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void RefreshContent_Should_ArchiveMetadataWithMissingContent() {
            var metadataToArchive = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder1, ContentContainer.Folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentContainer(Enumerable.Empty<ContentMetadata>(), metadataToArchive, Enumerable.Empty<string>());
            container.RunRefreshContentTest();
        }

        [Test]
        [Category("Unit Tests")]
        public void RefreshContent_Should_CreateMetadataForNewContent() {
            var newContentFiles = new[] {
                Path.Combine(ContentContainer.Folder1, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(ContentContainer.Folder2, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(ContentContainer.Folder1, ContentContainer.Folder1A, $"{Guid.NewGuid()}.jpg"),
                $"{Guid.NewGuid()}.jpg"
            };

            var container = new ContentContainer(Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<ContentMetadata>(), newContentFiles);
            container.RunRefreshContentTest();
        }

        [Test]
        [Category("Unit Tests")]
        public void RefreshContent_Should_HandleAComplexSituation() {
            var existing = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder1, ContentContainer.Folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Guid.NewGuid().ToString() }, ".jpg")
            };

            var metadataToArchive = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder1, ContentContainer.Folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Guid.NewGuid().ToString() }, ".jpg")
            };

            var newContentFiles = new[] {
                Path.Combine(ContentContainer.Folder1, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(ContentContainer.Folder2, $"{Guid.NewGuid()}.jpg"),
                Path.Combine(ContentContainer.Folder1, ContentContainer.Folder1A, $"{Guid.NewGuid()}.jpg"),
                $"{Guid.NewGuid()}.jpg"
            };

            var container = new ContentContainer(existing, metadataToArchive, newContentFiles);
            container.RunRefreshContentTest();
        }

        [Test]
        [Category("Unit Tests")]
        public void RefreshContent_Should_ResolveExistingMetadata() {
            var existing = new[] {
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder1, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder2, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { ContentContainer.Folder1, ContentContainer.Folder1A, Guid.NewGuid().ToString() }, ".jpg"),
                new ContentMetadata(new SpriteSheet(), new[] { Guid.NewGuid().ToString() }, ".jpg")
            };

            var container = new ContentContainer(existing, Enumerable.Empty<ContentMetadata>(), Enumerable.Empty<string>());
            container.RunRefreshContentTest();
        }
    }
}