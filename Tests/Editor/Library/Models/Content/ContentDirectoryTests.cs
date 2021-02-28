namespace Macabresoft.Macabre2D.Tests.Editor.Library.Models.Content {
    using System;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Editor.Library.Models.Content;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.Tests.Editor.Library.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class ContentDirectoryTests {
        [Test]
        [Category("Unit Tests")]
        public void GetContentPath_ShouldReturnPath() {
            var rootPath = Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var fileSystemService = Substitute.For<IFileSystemService>();
            fileSystemService.GetDirectories(Arg.Any<string>()).Returns(Enumerable.Empty<string>());
            var root = new RootContentDirectory(fileSystemService, rootPath);

            var firstA = new ContentDirectory(Guid.NewGuid().ToString(), root);
            var firstB = new ContentDirectory(Guid.NewGuid().ToString(), root);
            var firstC = new ContentFile(root, new ContentMetadata(null, new[] {Guid.NewGuid().ToString()}, ".m2d"));
            var secondA = new ContentDirectory(Guid.NewGuid().ToString(), firstA);
            var secondB = new ContentDirectory(Guid.NewGuid().ToString(), firstA);
            var secondC = new ContentFile(firstB, new ContentMetadata(null, new[] {Guid.NewGuid().ToString()}, ".m2d"));
            var thirdA = new ContentDirectory(Guid.NewGuid().ToString(), secondA);
            var thirdB = new ContentDirectory(Guid.NewGuid().ToString(), secondB);
            var thirdC = new ContentFile(secondA, new ContentMetadata(null, new[] {Guid.NewGuid().ToString()}, ".m2d"));
            var fourth = new ContentFile(thirdA, new ContentMetadata(null, new[] {Guid.NewGuid().ToString()}, ".m2d"));

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
            var fileSystemService = new FakeFileSystemService();

            // Root calls LoadChildDirectories when constructed.
            var root = new RootContentDirectory(fileSystemService, fileSystemService.PathToContentDirectory);

            using (new AssertionScope()) {
                root.Children.Count.Should().Be(fileSystemService.DirectoryToChildrenMap[fileSystemService.PathToContentDirectory].Count());
                var count = 1 + root.Children.Sum(child => this.AssertDirectoryMatches(fileSystemService, root, child.Name));
                count.Should().Be(fileSystemService.DirectoryToChildrenMap.Count);
            }
        }

        private int AssertDirectoryMatches(FakeFileSystemService fileSystemService, IContentDirectory parent, string name) {
            var currentDirectory = parent.Children.OfType<IContentDirectory>().First(x => x.Name == name);
            currentDirectory.Children.Count.Should().Be(fileSystemService.DirectoryToChildrenMap[name].Count());
            return 1 + currentDirectory.Children.Sum(child => this.AssertDirectoryMatches(fileSystemService, currentDirectory, child.Name));
        }
    }
}