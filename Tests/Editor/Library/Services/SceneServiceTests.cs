namespace Macabresoft.Macabre2D.Tests.Editor.Library.Services {
    using System;
    using System.IO;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Editor.Library.Models.Content;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class SceneServiceTests {
        private static readonly string ProjectDirectoryPath = Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        private static readonly string ContentDirectoryPath = Path.Combine(ProjectDirectoryPath, PathService.ContentDirectoryName);
        private static readonly string SceneName = Guid.NewGuid().ToString();
        private static readonly string SceneFilePath = Path.Combine(ContentDirectoryPath, $"{SceneName}{SceneAsset.FileExtension}");

        [Test]
        [Category("Unit Tests")]
        public void CreateNewScene_ShouldCreateScene() {
            var assetManager = Substitute.For<IAssetManager>();
            var fileSystem = Substitute.For<IFileSystemService>();
            var pathService = Substitute.For<IPathService>();
            var serializer = Substitute.For<ISerializer>();
            var contentDirectory = Substitute.For<IContentDirectory>();
            contentDirectory.GetFullPath().Returns(ContentDirectoryPath);
            fileSystem.DoesDirectoryExist(ContentDirectoryPath).Returns(true);
            fileSystem.DoesFileExist(SceneFilePath).Returns(false);

            var sceneService = new SceneService(assetManager, fileSystem, pathService, serializer);
            var sceneAsset = sceneService.CreateNewScene(contentDirectory, SceneName);

            using (new AssertionScope()) {
                sceneAsset.Should().NotBeNull();
                sceneAsset.Name.Should().Be(SceneName);
                sceneAsset.Content.Should().NotBeNull();
                // ReSharper disable once PossibleNullReferenceException
                sceneAsset.Content.Name.Should().Be(SceneName);
                serializer.Received().Serialize(sceneAsset.Content, SceneFilePath);
                serializer.Received().Serialize(
                    Arg.Is<ContentMetadata>(x => x.GetFileNameWithoutExtension() == SceneName && x.ContentFileExtension == SceneAsset.FileExtension),
                    Arg.Is<string>(x => x.StartsWith(ContentDirectoryPath)));
                contentDirectory.Received().AddChild(Arg.Any<ContentFile>());
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void TryLoadScene_LoadsScene_WhenSceneExists() {
            var assetManager = Substitute.For<IAssetManager>();
            var fileSystem = Substitute.For<IFileSystemService>();
            var pathService = new PathService(string.Empty, ProjectDirectoryPath);
            var serializer = Substitute.For<ISerializer>();
            var contentDirectory = Substitute.For<IContentDirectory>();
            contentDirectory.GetFullPath().Returns(ContentDirectoryPath);
            fileSystem.DoesDirectoryExist(ContentDirectoryPath).Returns(true);
            fileSystem.DoesFileExist(SceneFilePath).Returns(false);

            var scene = new GameScene();
            fileSystem.DoesFileExist(SceneFilePath).Returns(true);
            serializer.Deserialize<GameScene>(SceneFilePath).Returns(scene);
            
            var sceneAsset = new SceneAsset();
            var metadata = new ContentMetadata(sceneAsset, new[] { SceneName }, SceneAsset.FileExtension);
            var metadataFilePath = pathService.GetMetadataFilePath(metadata.ContentId);
            fileSystem.DoesFileExist(metadataFilePath).Returns(true);
            serializer.Deserialize<ContentMetadata>(metadataFilePath).Returns(metadata);
            
            var sceneService = new SceneService(assetManager, fileSystem, pathService, serializer);
            var result = sceneService.TryLoadScene(metadata.ContentId, out var loadedSceneAsset);
            
            using (new AssertionScope()) {
                result.Should().BeTrue();
                loadedSceneAsset.Should().Be(sceneAsset);
                loadedSceneAsset.Content.Should().Be(scene);
                sceneService.CurrentScene.Should().Be(loadedSceneAsset.Content);
            }
        }
    }
}