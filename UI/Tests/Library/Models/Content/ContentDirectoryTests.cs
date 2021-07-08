namespace Macabresoft.Macabre2D.Tests.Editor.Library.Models.Content {
    using System;
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
    public class ContentDirectoryTests {
        private int AssertDirectoryMatches(TestFileSystemService fileSystemService, IContentDirectory parent, string name) {
            var currentDirectory = parent.Children.OfType<IContentDirectory>().First(x => x.Name == name);
            currentDirectory.Children.Count.Should().Be(fileSystemService.DirectoryToChildrenMap[name].Count());
            return 1 + currentDirectory.Children.Sum(child => this.AssertDirectoryMatches(fileSystemService, currentDirectory, child.Name));
        }

        [Test]
        [Category("Unit Tests")]
        public void GetContentPath_ShouldReturnPath() {
            var rootPath = Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var pathService = Substitute.For<IPathService>();
            pathService.ContentDirectoryPath.Returns(rootPath);

            var fileSystemService = Substitute.For<IFileSystemService>();
            fileSystemService.GetDirectories(Arg.Any<string>()).Returns(Enumerable.Empty<string>());
            var root = new RootContentDirectory(fileSystemService, pathService);

            var firstA = new ContentDirectory(Guid.NewGuid().ToString(), root);
            var firstB = new ContentDirectory(Guid.NewGuid().ToString(), root);
            var firstC = new ContentFile(root, new ContentMetadata(null, new[] { Guid.NewGuid().ToString() }, ".m2d"));
            var secondA = new ContentDirectory(Guid.NewGuid().ToString(), firstA);
            var secondB = new ContentDirectory(Guid.NewGuid().ToString(), firstA);
            var secondC = new ContentFile(firstB, new ContentMetadata(null, new[] { Guid.NewGuid().ToString() }, ".m2d"));
            var thirdA = new ContentDirectory(Guid.NewGuid().ToString(), secondA);
            var thirdB = new ContentDirectory(Guid.NewGuid().ToString(), secondB);
            var thirdC = new ContentFile(secondA, new ContentMetadata(null, new[] { Guid.NewGuid().ToString() }, ".m2d"));
            var fourth = new ContentFile(thirdA, new ContentMetadata(null, new[] { Guid.NewGuid().ToString() }, ".m2d"));

            using (new AssertionScope()) {
                firstA.GetContentPath().Should().Be(firstA.Name);
                firstB.GetContentPath().Should().Be(firstB.Name);
                firstC.GetContentPath().Should().Be(firstC.NameWithoutExtension);
                secondA.GetContentPath().Should().Be(Path.Combine(firstA.Name, secondA.Name));
                secondB.GetContentPath().Should().Be(Path.Combine(firstA.Name, secondB.Name));
                secondC.GetContentPath().Should().Be(Path.Combine(firstB.Name, secondC.NameWithoutExtension));
                thirdA.GetContentPath().Should().Be(Path.Combine(firstA.Name, secondA.Name, thirdA.Name));
                thirdB.GetContentPath().Should().Be(Path.Combine(firstA.Name, secondB.Name, thirdB.Name));
                thirdC.GetContentPath().Should().Be(Path.Combine(firstA.Name, secondA.Name, thirdC.NameWithoutExtension));
                fourth.GetContentPath().Should().Be(Path.Combine(firstA.Name, secondA.Name, thirdA.Name, fourth.NameWithoutExtension));
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void RootContentDirectory_ShouldBuildContentHierarchy() {
            var fileSystemService = new TestFileSystemService();

            // Root calls LoadChildDirectories when constructed.
            var root = new RootContentDirectory(fileSystemService, fileSystemService.TestPathService);

            using (new AssertionScope()) {
                root.Children.Count.Should().Be(fileSystemService.DirectoryToChildrenMap[PathService.ContentDirectoryName].Count());
                var count = root.Children.Sum(child => this.AssertDirectoryMatches(fileSystemService, root, child.Name));
                count.Should().Be(fileSystemService.DirectoryToChildrenMap.Count - 2); // The content and project directory are not in the count, so - 2.
            }
        }
    }
}