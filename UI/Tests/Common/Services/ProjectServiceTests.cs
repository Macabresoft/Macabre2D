namespace Macabresoft.Macabre2D.UI.Tests;

using System;
using System.IO;
using FluentAssertions;
using FluentAssertions.Execution;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Common;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class ProjectServiceTests {
    [Test]
    [Category("Unit Tests")]
    public void LoadProject_ShouldCreateProject_WhenFileDoesNotExist() {
        var contentService = Substitute.For<IContentService>();
        var pathService = this.CreatePathService();
        var fileSystem = this.CreateFileSystem(pathService, false);
        var sceneService = Substitute.For<ISceneService>();

        var rootContent = Substitute.For<IContentDirectory>();
        rootContent.GetFullPath().Returns(pathService.ContentDirectoryPath);
        contentService.RootContentDirectory.Returns(rootContent);

        var sceneAsset = new SceneAsset();
        sceneService.TryLoadScene(Arg.Any<Guid>(), out Arg.Any<SceneAsset>())
            .Returns(x =>
            {
                x[1] = sceneAsset;
                return true;
            });

        var serializer = Substitute.For<ISerializer>();
        serializer.When(x => x.Serialize(Arg.Any<GameProject>(), Arg.Any<string>()))
            .Do(x =>
            {
                if (x[1] is string path) {
                    fileSystem.DoesFileExist(path).Returns(true);
                    serializer.Deserialize<GameProject>(path).Returns(new GameProject());
                }
            });

        var projectService = new ProjectService(
            contentService,
            fileSystem,
            pathService,
            sceneService,
            serializer,
            Substitute.For<IEditorSettingsService>());

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
        var settingsService = Substitute.For<IEditorSettingsService>();
        settingsService.Settings.Returns(new EditorSettings());
        var sceneService = Substitute.For<ISceneService>();
        var sceneAsset = new SceneAsset();
        sceneService.TryLoadScene(Arg.Any<Guid>(), out Arg.Any<SceneAsset>())
            .Returns(x =>
            {
                x[1] = sceneAsset;
                return true;
            });

        var projectService = new ProjectService(
            Substitute.For<IContentService>(),
            fileSystem,
            pathService,
            sceneService,
            serializer,
            settingsService);


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
            serializer,
            Substitute.For<IEditorSettingsService>());

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
        var settingsService = Substitute.For<IEditorSettingsService>();
        settingsService.Settings.Returns(new EditorSettings());
        var sceneService = Substitute.For<ISceneService>();
        var sceneAsset = new SceneAsset();
        sceneService.TryLoadScene(Arg.Any<Guid>(), out Arg.Any<SceneAsset>())
            .Returns(x =>
            {
                x[1] = sceneAsset;
                return true;
            });

        var projectService = new ProjectService(
            Substitute.For<IContentService>(),
            fileSystem,
            pathService,
            sceneService,
            serializer,
            settingsService);

        projectService.LoadProject();
        projectService.SaveProject();

        using (new AssertionScope()) {
            serializer.Received().Serialize(project, pathService.ProjectFilePath);
        }
    }

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
}