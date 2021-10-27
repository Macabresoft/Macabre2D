namespace Macabresoft.Macabre2D.UI.Tests {
    using System;
    using System.IO;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class SceneServiceTests {
        private static readonly string ProjectDirectoryPath = Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        private static readonly string SceneName = Guid.NewGuid().ToString();

        [Test]
        [Category("Unit Tests")]
        public void TryLoadScene_LoadsScene_WhenSceneExists() {
            var fileSystem = Substitute.For<IFileSystemService>();
            var pathService = new PathService(string.Empty, ProjectDirectoryPath);
            var serializer = Substitute.For<ISerializer>();
            var settingsService = Substitute.For<IEditorSettingsService>();
            settingsService.Settings.Returns(new EditorSettings());
            var contentDirectory = Substitute.For<IContentDirectory>();
            contentDirectory.GetFullPath().Returns(pathService.ContentDirectoryPath);
            fileSystem.DoesDirectoryExist(pathService.ContentDirectoryPath).Returns(true);
            var sceneFilePath = Path.Combine(pathService.ContentDirectoryPath, $"{SceneName}{SceneAsset.FileExtension}");
            fileSystem.DoesFileExist(sceneFilePath).Returns(false);
            var scene = new Scene();
            fileSystem.DoesFileExist(sceneFilePath).Returns(true);
            serializer.Deserialize<Scene>(sceneFilePath).Returns(scene);

            var sceneAsset = new SceneAsset();
            var metadata = new ContentMetadata(sceneAsset, new[] { SceneName }, SceneAsset.FileExtension);
            var metadataFilePath = pathService.GetMetadataFilePath(metadata.ContentId);
            fileSystem.DoesFileExist(metadataFilePath).Returns(true);
            serializer.Deserialize<ContentMetadata>(metadataFilePath).Returns(metadata);

            var sceneService = new SceneService(Substitute.For<IEntityService>(), fileSystem, pathService, serializer, settingsService, Substitute.For<ISystemService>());
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