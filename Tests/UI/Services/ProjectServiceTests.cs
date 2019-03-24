namespace Macabre2D.Tests.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Serialization;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.FrameworkWrappers;
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
            var assemblyService = Substitute.For<IAssemblyService>();
            var dialogService = Substitute.For<IDialogService>();
            var loggingService = Substitute.For<ILoggingService>();
            var sceneService = Substitute.For<ISceneService>();
            var projectService = new ProjectService(new Serializer(), assemblyService, dialogService, loggingService, sceneService);

            var projectDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestProject");
            if (Directory.Exists(projectDirectory)) {
                Directory.Delete(projectDirectory, true);
            }

            dialogService.ShowCreateProjectDialog(out Arg.Any<Project>(), Arg.Any<string>()).Returns(x => {
                x[0] = new Project() {
                    Name = "TestProject",
                    PathToProject = Path.Combine(projectDirectory, $"TestProject{FileHelper.ProjectExtension}")
                };

                return true;
            });

            dialogService.ShowSaveDiscardCancelDialog().Returns(SaveDiscardCancelResult.Cancel);

            var scene = new Scene() {
                Name = Guid.NewGuid().ToString()
            };

            var sceneWrapper = new SceneWrapper(scene);

            dialogService.ShowYesNoMessageBox(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            sceneService.CreateScene().Returns(sceneWrapper);
            sceneService.SaveCurrentScene(Arg.Any<Project>()).Returns(true);
            sceneService.LoadScene(Arg.Any<Project>(), Arg.Any<SceneAsset>()).Returns(sceneWrapper);

            try {
                Directory.CreateDirectory(projectDirectory);
                var project = await projectService.CreateProject(TestContext.CurrentContext.TestDirectory, projectDirectory);

                Assert.NotNull(project);
                Assert.NotNull(projectService.CurrentProject);
                Assert.AreEqual(project, projectService.CurrentProject);
                Assert.True(File.Exists(project.PathToProject));
            }
            finally {
                if (Directory.Exists(projectDirectory)) {
                    Directory.Delete(projectDirectory, true);
                }
            }
        }
    }
}