﻿namespace Macabresoft.Macabre2D.Tests.Editor.Library.Services {
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
            var pathService = Substitute.For<IPathService>();
            var serializer = Substitute.For<ISerializer>();
            var sceneName = Guid.NewGuid().ToString();
            var directoryPath = Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var filePath = Path.Combine(directoryPath, $"{sceneName}{SceneAsset.FileExtension}");
            fileSystem.DoesDirectoryExist(directoryPath).Returns(true);
            fileSystem.DoesFileExist(filePath).Returns(false);

            var sceneService = new SceneService(fileSystem, pathService, serializer);
            var sceneAsset = sceneService.CreateNewScene(directoryPath, sceneName);

            using (new AssertionScope()) {
                sceneAsset.Should().NotBeNull();
                sceneAsset.Name.Should().Be(sceneName);
                sceneAsset.Content.Should().NotBeNull();
                // ReSharper disable once PossibleNullReferenceException
                sceneAsset.Content.Name.Should().Be(sceneName);
                serializer.Received().Serialize(sceneAsset.Content, filePath);
                serializer.Received().Serialize(
                    Arg.Is<ContentMetadata>(x => x.GetFileNameWithoutExtension() == sceneName && x.ContentFileExtension == SceneAsset.FileExtension),
                    Arg.Is<string>(x => x.StartsWith(directoryPath)));
            }
        }
    }
}