namespace Macabresoft.Macabre2D.Tests.UI.Common.Models.Content {
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public sealed class ContentNodeTests {
        private const string ProjectPath = "Content";

        [Test]
        [Category("Unit Tests")]
        public void ChangeParent_ShouldChangePaths() {
            var pathService = Substitute.For<IPathService>();
            pathService.PlatformsDirectoryPath.Returns(ProjectPath);
            pathService.ContentDirectoryPath.Returns(pathService.PlatformsDirectoryPath, PathService.ContentDirectoryName);

            var root = new RootContentDirectory(Substitute.For<IFileSystemService>(), pathService);
            var directory1 = new ContentDirectory("D1", root);
            var directory2 = new ContentDirectory("D2", root);

            var fileName = "File";
            var fileExtension = ".jpg";
            var file = new ContentFile(directory1, new ContentMetadata(Substitute.For<IAsset>(), new[] { directory1.Name, fileName }, fileExtension));
            file.ChangeParent(directory2);

            using (new AssertionScope()) {
                var expectedContentPath = Path.Combine(directory2.Name, fileName);
                file.GetContentPath().Should().Be(expectedContentPath);

                var expectedFullPath = Path.Combine(pathService.ContentDirectoryPath, $"{expectedContentPath}{fileExtension}");
                file.GetFullPath().Should().Be(expectedFullPath);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void IsDescendentOf_ShouldReturnFalse_WhenChildOfDirectParent() {
            var parent = new ContentDirectory(string.Empty, null);
            var otherDirectory = new ContentDirectory(string.Empty, parent);
            var node = new ContentFile(parent, this.CreateTestMetadata());
            var result = node.IsDescendentOf(otherDirectory);
            result.Should().BeFalse();
        }

        [Test]
        [Category("Unit Tests")]
        public void IsDescendentOf_ShouldReturnTrue_WhenDirectParent() {
            var parent = new ContentDirectory(string.Empty, null);
            var node = new ContentFile(parent, this.CreateTestMetadata());
            var result = node.IsDescendentOf(parent);
            result.Should().BeTrue();
        }

        [Test]
        [Category("Unit Tests")]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void IsDescendentOf_ShouldReturnTrue_WithVariableDepths(int layersDeep) {
            var root = new ContentDirectory(string.Empty, null);
            var parent = root;

            for (var i = 0; i < layersDeep; i++) {
                parent = new ContentDirectory(string.Empty, parent);
            }

            var node = new ContentFile(parent, this.CreateTestMetadata());
            var result = node.IsDescendentOf(root);
            result.Should().BeTrue();
        }

        private ContentMetadata CreateTestMetadata() {
            return new ContentMetadata(Substitute.For<IAsset>(), Enumerable.Empty<string>(), string.Empty);
        }
    }
}