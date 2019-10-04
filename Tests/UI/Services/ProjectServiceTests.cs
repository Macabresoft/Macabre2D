namespace Macabre2D.Tests.UI.Services {

    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using Macabre2D.UI.Services;
    using NSubstitute;
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    [TestFixture]
    public static class ProjectServiceTests {

        [Test]
        [Category("Integration Test")]
        public static async Task ProjectService_CreateProjectTest() {
            var fileService = Substitute.For<IFileService>();
            var loggingService = Substitute.For<ILoggingService>();
            var sceneService = Substitute.For<ISceneService>();
            var projectService = new ProjectService(fileService, loggingService, sceneService);

            var projectDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestProject");
            fileService.ProjectDirectoryPath.Returns(projectDirectory);
            if (Directory.Exists(projectDirectory)) {
                Directory.Delete(projectDirectory, true);
            }

            var sceneAsset = new SceneAsset(Guid.NewGuid().ToString());
            sceneService.CreateScene(Arg.Any<FolderAsset>(), Arg.Any<string>()).Returns(sceneAsset);
            sceneService.SaveCurrentScene(Arg.Any<Project>()).Returns(true);
            sceneService.LoadScene(Arg.Any<Project>(), Arg.Any<SceneAsset>()).Returns(sceneAsset);

            try {
                Directory.CreateDirectory(projectDirectory);
                var project = await projectService.CreateProject();

                Assert.NotNull(project);
                Assert.NotNull(projectService.CurrentProject);
                Assert.AreEqual(project, projectService.CurrentProject);
                Assert.True(File.Exists(projectService.GetPathToProject()));
            }
            finally {
                if (Directory.Exists(projectDirectory)) {
                    Directory.Delete(projectDirectory, true);
                }
            }
        }
    }
}