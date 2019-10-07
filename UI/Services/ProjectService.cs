namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using Macabre2D.UI.Services.Content;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public sealed class ProjectService : NotifyPropertyChanged, IProjectService {
        private const string AssetsLocation = @"Assets";
        private readonly IDialogService _dialogService;
        private readonly IFileService _fileService;
        private readonly ILoggingService _loggingService;
        private readonly ISceneService _sceneService;
        private Project _currentProject;
        private bool _hasChanges;

        public ProjectService(
            IDialogService dialogService,
            IFileService fileService,
            ILoggingService loggingService,
            ISceneService sceneService) {
            this._dialogService = dialogService;
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

        public async Task<bool> AutoSaveProject(int maxAutoSaves, bool purgeExcessAutoSaves) {
            if (purgeExcessAutoSaves) {
                var autoSaves = this.GetAutoSaveFiles().ToList();

                while (autoSaves.Count >= maxAutoSaves && autoSaves.Count > 0) {
                    var filePath = autoSaves.ElementAt(0);
                    File.Delete(filePath);
                    autoSaves.RemoveAt(0);
                }
            }

            var newAutoSaveName = $"{nameof(Project)}-{DateTime.Now.ToString(FileHelper.FileDateTimeFormat)}{FileHelper.ProjectAutoSaveExtension}";
            return await this.SaveProject(Path.Combine(this._fileService.ProjectDirectoryPath, newAutoSaveName));
        }

        public async Task<bool> BuildAllAssets(BuildMode mode) {
            var result = true;
            await Task.Run(() => {
                Serializer.Instance.Serialize(this.CurrentProject.AssetManager, Path.Combine(this._fileService.ProjectDirectoryPath, $"{AssetManager.ContentFileName}{FileHelper.AssetManagerExtension}"));
                Serializer.Instance.Serialize(this.CurrentProject.GameSettings, Path.Combine(this._fileService.ProjectDirectoryPath, $"{GameSettings.ContentFileName}{FileHelper.GameSettingsExtension}"));

                var assets = this.CurrentProject.AssetFolder.GetAllContentAssets();
                var dllPaths = new[] {
                    $@"{this._fileService.ProjectDirectoryPath}\bin\Editor\{mode.ToString()}\Newtonsoft.Json.dll",
                    $@"{this._fileService.ProjectDirectoryPath}\bin\Editor\{mode.ToString()}\Macabre2D.Framework.dll",
                    $@"{this._fileService.ProjectDirectoryPath}\bin\Editor\{mode.ToString()}\Macabre2D.Project.Gameplay.dll"
                };

                foreach (var configuration in this.CurrentProject.BuildConfigurations) {
                    configuration.CreateContentFile(this._fileService.ProjectDirectoryPath, assets, false, dllPaths);
                    var contentFilePath = Path.Combine(this._fileService.ProjectDirectoryPath, $"{configuration.Platform.ToString()}.mgcb");
                    var outputDirectory = Path.Combine(this._fileService.ProjectDirectoryPath, "bin", configuration.Platform.ToString(), mode.ToString(), "Content");
                    Directory.CreateDirectory(outputDirectory);

                    var exitCode = ContentBuilder.BuildContent(
                        out var exception,
                        $"/@:{contentFilePath}",
                        $"/platform:{configuration.Platform.ToString()}",
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

        public IEnumerable<string> GetAutoSaveFiles() {
            return Directory.EnumerateFiles(this._fileService.ProjectDirectoryPath, $"*{FileHelper.ProjectAutoSaveExtension}").OrderBy(x => x).ToList();
        }

        public string GetPathToProject() {
            return Path.Combine(this._fileService.ProjectDirectoryPath, Project.ProjectFileName);
        }

        public async Task<Project> LoadProject(string pathToProject) {
            var project = File.Exists(pathToProject) ?
                await Task.Run(() => Serializer.Instance.Deserialize<Project>(pathToProject)) :
                await this.CreateProject();

            GameSettings.Instance = project.GameSettings;

            project.Initialize(this._fileService.ProjectDirectoryPath);
            project.Refresh();
            this.CurrentProject = project;
            await this.BuildAllAssets(BuildMode.Debug);

            if (this.CurrentProject?.LastSceneOpened != null) {
                await this._sceneService.LoadScene(this.CurrentProject, this.CurrentProject.LastSceneOpened);
                this._sceneService.HasChanges = false;
            }
            else if (this.CurrentProject?.SceneAssets.FirstOrDefault() is SceneAsset sceneAsset) {
                await this._sceneService.LoadScene(this.CurrentProject, sceneAsset);
                this._sceneService.HasChanges = false;
            }
            else {
                var scene = await this._sceneService.CreateScene(this.CurrentProject.AssetFolder, "Default");
                this.CurrentProject.LastSceneOpened = scene;
                this._sceneService.HasChanges = true;
            }

            this.HasChanges = false;
            return this.CurrentProject;
        }

        public async Task<Project> LoadProject() {
            var projectPath = this.GetPathToProject();
            await this.LoadProject(projectPath);
            this.HasChanges = false;
            return this.CurrentProject;
        }

        public void NavigateToProjectLocation() {
            Process.Start(this._fileService.ProjectDirectoryPath);
        }

        public async Task<bool> SaveProject() {
            var pathToProject = this.GetPathToProject();
            this._sceneService.CurrentScene.HasChanges = true;
            this.CurrentProject.LastSceneOpened = this._sceneService.CurrentScene;
            await Task.Run(() => this.CurrentProject.SaveAssets());
            var result = await this.SaveProject(pathToProject);
            this.HasChanges = false;
            this._currentProject.LastTimeSaved = DateTime.Now;
            return result;
        }

        internal async Task<Project> CreateProject() {
            var pathToProject = this.GetPathToProject();
            var project = new Project(BuildPlatform.DesktopGL) {
                Name = "ProjectName"
            };

            project.Initialize(this._fileService.ProjectDirectoryPath);
            await Task.Run(() => Serializer.Instance.Serialize(project, pathToProject));
            Directory.CreateDirectory(Path.Combine(this._fileService.ProjectDirectoryPath, AssetsLocation));
            var scene = await this._sceneService.CreateScene(project.AssetFolder, "Default");
            project.SceneAssets.Add(scene);
            project.StartUpSceneAsset = scene;
            project.LastSceneOpened = scene;
            scene.ForceSave();
            this.CurrentProject = project;
            this.HasChanges = true;

            return this.CurrentProject;
        }

        private void CurrentProject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.HasChanges = true;
        }

        private async Task<bool> SaveProject(string pathToProject) {
            var result = true;
            try {
                await Task.Run(() => Serializer.Instance.Serialize(this.CurrentProject, pathToProject));
            }
            catch (Exception e) {
                this._loggingService.LogError($"Failed to save project at '{pathToProject}' with exception: {Environment.NewLine}{e.Message}");
                result = false;
            }

            this.CurrentProject.LastSceneOpened = this._sceneService.CurrentScene;
            return result;
        }
    }
}