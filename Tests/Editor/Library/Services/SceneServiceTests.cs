namespace Macabresoft.Macabre2D.Tests.Editor.Library.Services {
    using System;
    using System.IO;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class SceneServiceTests {
        [Test]
        [Category("Unit Tests")]
        public void CreateNewScene_ShouldCreateScene_WhenDirectoryExists() {
            var fileSystem = Substitute.For<IFileSystemService>();
            var serializer = Substitute.For<ISerializer>();
            var sceneName = Guid.NewGuid().ToString();
            var directoryPath = Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var filePath = Path.Combine(directoryPath, $"{sceneName}{SceneAsset.FileExtension}");
            fileSystem.DoesDirectoryExist(directoryPath).Returns(true);
            fileSystem.DoesFileExist(filePath).Returns(false);

            var sceneService = new SceneService(fileSystem, serializer);
            var scene = sceneService.CreateNewScene(directoryPath, sceneName);

            using (new AssertionScope()) {
                scene.Should().NotBeNull();
                scene.Name.Should().Be(sceneName);
                serializer.Received().Serialize(scene, filePath);
                serializer.Received().Serialize(
                    Arg.Is<ContentMetadata>(x => x.GetFileNameWithoutExtension() == sceneName && x.ContentFileExtension == SceneAsset.FileExtension),
                    Arg.Is<string>(x => x.StartsWith(directoryPath)));
            }
        }
    }
}