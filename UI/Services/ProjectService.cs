namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using Macabre2D.UI.Services.Content;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class ProjectService : NotifyPropertyChanged, IProjectService {
        private const string AssetsLocation = @"Assets";
        private readonly IFileService _fileService;
        private readonly ILoggingService _loggingService;
        private readonly ISceneService _sceneService;
        private readonly Serializer _serializer;
        private Project _currentProject;
        private bool _hasChanges;

        public ProjectService(
            Serializer serializer,
            IFileService fileService,
            ILoggingService loggingService,
            ISceneService sceneService) {
            this._serializer = serializer;
            this._fileService = fileService;
            this._loggingService = loggingService;
            this._sceneService = sceneService;
        }

        public Project CurrentProject {
            get {
                return this._currentProject;
            }

            private set {
                if (value != null) {
                    var oldProject = this.CurrentProject;
                    if (this.Set(ref this._currentProject, value) && this._currentProject != null) {
                        this.CurrentProject.PropertyChanged += this.CurrentProject_PropertyChanged;
                    }

                    if (oldProject != null) {
                        oldProject.PropertyChanged -= this.CurrentProject_PropertyChanged;
                    }

                    this.RaisePropertyChanged(nameof(this.HasChanges));
                }
            }
        }

        public bool HasChanges {
            get {
                return this._currentProject != null && (this._hasChanges || this._sceneService.HasChanges);
            }

            set {
                this.Set(ref this._hasChanges, value);
            }
        }

        public async Task<bool> BuildContent(BuildMode mode) {
            var result = true;
            await Task.Run(() => {
                var assets = this.CurrentProject.AssetFolder.GetAllContentAssets();

                foreach (var configuration in this.CurrentProject.BuildConfigurations) {
                    var dllPaths = new[] {
                        $@"{this._fileService.ProjectDirectoryPath}\bin\{configuration.Platform.ToString()}\{mode.ToString()}\Newtonsoft.Json.dll",
                        $@"{this._fileService.ProjectDirectoryPath}\bin\{configuration.Platform.ToString()}\{mode.ToString()}\Macabre2D.Framework.dll",
                        $@"{this._fileService.ProjectDirectoryPath}\bin\{configuration.Platform.ToString()}\{mode.ToString()}\Macabre2D.Project.Gameplay.dll"
                    };

                    configuration.GenerateContent(this._fileService.ProjectDirectoryPath, assets, this.CurrentProject.AssetManager, this.CurrentProject.GameSettings, this._serializer, dllPaths);
                    var contentFilePath = Path.Combine(this._fileService.ProjectDirectoryPath, $"{configuration.Platform.ToString()}.mgcb");
                    var outputDirectory = Path.Combine(this._fileService.ProjectDirectoryPath, "bin", configuration.Platform.ToString(), mode.ToString(), "Content");
                    Directory.CreateDirectory(outputDirectory);

                    var exitCode = ContentBuilder.BuildContent(
                        out var exception,
                        $"/@:{contentFilePath}", $"/platform:{configuration.Platform.ToString()}",
                        $@"/outputDir:{outputDirectory}",
                        $"/workingDir:{this._fileService.ProjectDirectoryPath}");

                    if (exitCode != 0) {
                        result = false;
                        this._loggingService.LogError($"Content could not be built for '{this.CurrentProject.Name}' in '{mode.ToString()}' mode: {exception?.Message}");
                    }
                }
            });

            return result;
        }

        public string GetPathToProject() {
            return Path.Combine(this._fileService.ProjectDirectoryPath, Project.ProjectFileName);
        }

        public async Task<Project> LoadProject() {
            var projectPath = this.GetPathToProject();

            var project = File.Exists(projectPath) ?
                await Task.Run(() => this._serializer.Deserialize<Project>(projectPath)) :
                await this.CreateProject();

            project.Initialize(this._fileService.ProjectDirectoryPath);
            project.Refresh();
            this.CurrentProject = project;
            await this.BuildContent(BuildMode.Debug);

            if (this.CurrentProject?.LastSceneOpened != null) {
                await this._sceneService.LoadScene(this.CurrentProject, this.CurrentProject.LastSceneOpened);
            }
            else if (this.CurrentProject?.SceneAssets.FirstOrDefault() is SceneAsset sceneAsset) {
                await this._sceneService.LoadScene(this.CurrentProject, sceneAsset);
            }
            else {
                var scene = await this._sceneService.CreateScene(this.CurrentProject.AssetFolder, "Default");
                this.CurrentProject.LastSceneOpened = scene.SceneAsset;
                this._sceneService.HasChanges = true;
            }

            return this.CurrentProject;
        }

        public void NavigateToProjectLocation() {
            Process.Start(this._fileService.ProjectDirectoryPath);
        }

        public async Task<bool> SaveProject() {
            var pathToProject = this.GetPathToProject();
            await Task.Run(() => this.CurrentProject.SaveAssets());
            await this._sceneService.SaveCurrentScene(this.CurrentProject);
            await Task.Run(() => this._serializer.Serialize(this.CurrentProject, pathToProject));
            this.HasChanges = false;
            this._currentProject.LastTimeSaved = DateTime.Now;
            return true;
        }

        internal async Task<Project> CreateProject() {
            var pathToProject = this.GetPathToProject();
            var project = new Project(BuildPlatform.DesktopGL) {
                Name = "ProjectName"
            };

            project.Initialize(this._fileService.ProjectDirectoryPath);
            await Task.Run(() => this._serializer.Serialize(project, pathToProject));
            Directory.CreateDirectory(Path.Combine(this._fileService.ProjectDirectoryPath, AssetsLocation));
            var scene = await this._sceneService.CreateScene(project.AssetFolder, "Default");
            project.SceneAssets.Add(scene.SceneAsset);
            project.StartUpSceneAsset = scene.SceneAsset;
            project.LastSceneOpened = scene.SceneAsset;
            await this._sceneService.SaveCurrentScene(project);

            this.CurrentProject = project;
            this.HasChanges = true;

            return this.CurrentProject;
        }

        private void CurrentProject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.HasChanges = true;
        }
    }
}