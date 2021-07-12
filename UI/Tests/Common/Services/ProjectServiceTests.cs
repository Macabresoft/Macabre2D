namespace Macabresoft.Macabre2D.Tests.UI.Common.Services {
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
    public class ProjectServiceTests {
        private IPathService CreatePathService() {
            var projectDirectoryPath = Path.Combine(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            return new PathService(Guid.NewGuid().ToString(), projectDirectoryPath);
        }

        private IFileSystemService CreateFileSystem(IPathService pathService, bool shouldProjectFileExist) {
            var fileSystem = Substitute.For<IFileSystemService>();
            fileSystem.DoesFileExist(pathService.ProjectFilePath).Returns(shouldProjectFileExist);
            fileSystem.DoesDirectoryExist(pathService.PlatformsDirectoryPath).Returns(true);
            fileSystem.DoesDirectoryExist(pathService.ContentDirectoryPath).Returns(true);
            fileSystem.DoesDirectoryExist(pathService.MetadataDirectoryPath).Returns(true);
            fileSystem.DoesDirectoryExist(pathService.MetadataArchiveDirectoryPath).Returns(true);
            return fileSystem;
        }
        
        [Test]
        [Category("Unit Tests")]
        public void LoadProject_ShouldCreateProject_WhenFileDoesNotExist() {
            var pathService = this.CreatePathService();
            var fileSystem = this.CreateFileSystem(pathService, false);
            var sceneService = Substitute.For<ISceneService>();
            var sceneAsset = new SceneAsset();
            sceneService.CreateNewScene(Arg.Any<IContentDirectory>(), Arg.Any<string>()).Returns(sceneAsset);

            var serializer = Substitute.For<ISerializer>();
            serializer.When(x => x.Serialize(Arg.Any<GameProject>(), Arg.Any<string>()))
                .Do(x => {
                    if (x[1] is string path) {
                        fileSystem.DoesFileExist(path).Returns(true);
                        serializer.Deserialize<GameProject>(path).Returns(new GameProject());
                    }
                });

            var projectService = new ProjectService(
                Substitute.For<IContentService>(),
                fileSystem,
                pathService,
                sceneService,
                serializer);

            var project = projectService.LoadProject();

            using (new AssertionScope()) {
                serializer.Received().Serialize(Arg.Any<GameProject>(), pathService.ProjectFilePath);
                project.Should().NotBeNull();
                projectService.CurrentProject.Should().NotBeNull();
                projectService.CurrentProject.Should().Be(project);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void LoadProject_ShouldLoad_WhenFileExists() {
            var pathService = this.CreatePathService();
            var fileSystem = this.CreateFileSystem(pathService, true);
            var serializer = Substitute.For<ISerializer>();
            var project = new GameProject();
            serializer.Deserialize<GameProject>(pathService.ProjectFilePath).Returns(project);
            var projectService = new ProjectService(
                Substitute.For<IContentService>(),
                fileSystem,
                pathService,
                Substitute.For<ISceneService>(),
                serializer);

            var loadedProject = projectService.LoadProject();

            using (new AssertionScope()) {
                serializer.Received().Deserialize<GameProject>(pathService.ProjectFilePath);
                loadedProject.Should().Be(project);
                projectService.CurrentProject.Should().Be(project);
            }
        }


        [Test]
        [Category("Unit Tests")]
        public void SaveProject_ShouldNotSave_WhenProjectDoesNotExist() {
            var pathService = this.CreatePathService();
            var fileSystem = this.CreateFileSystem(pathService, true);
            var serializer = Substitute.For<ISerializer>();

            var projectService = new ProjectService(
                Substitute.For<IContentService>(),
                fileSystem,
                pathService,
                Substitute.For<ISceneService>(),
                serializer);

            projectService.SaveProject();

            using (new AssertionScope()) {
                serializer.DidNotReceive().Serialize(Arg.Any<IGameProject>(), pathService.ProjectFilePath);
            }
        }

        [Test]
        [Category("Unit Tests")]
        public void SaveProject_ShouldSave_WhenProjectExists() {
            var pathService = this.CreatePathService();
            var fileSystem = this.CreateFileSystem(pathService, true);
            var serializer = Substitute.For<ISerializer>();
            var project = new GameProject();
            serializer.Deserialize<GameProject>(pathService.ProjectFilePath).Returns(project);

            var projectService = new ProjectService(
                Substitute.For<IContentService>(),
                fileSystem,
                pathService,
                Substitute.For<ISceneService>(),
                serializer);

            projectService.LoadProject();
            projectService.SaveProject();

            using (new AssertionScope()) {
                serializer.Received().Serialize(project, pathService.ProjectFilePath);
            }
        }
    }
}