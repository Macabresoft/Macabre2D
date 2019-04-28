namespace Macabre2D.UI.Services {

    using Macabre2D.Framework;
    using Macabre2D.Framework.Serialization;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using MGCB;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;
    using System.Windows;

    public sealed class ProjectService : NotifyPropertyChanged, IProjectService {
        private const string AssetsLocation = @"Assets";
        private const string BinariesLocation = @"Binaries";
        private const string BinName = @"bin";
        private const string DebugName = @"Debug";
        private const string DependenciesLocation = @"Dependencies";
        private const string GameplayName = @"Gameplay";
        private const string MGCBExecutableName = "MGCB.exe";
        private const string ReferencesLocation = @"References";
        private const string ReleaseName = @"Release";
        private const short SecondsToAttemptBuildContent = 60;
        private const short SecondsToAttemptDelete = 60;
        private const string SettingsLocation = @"Settings";
        private const string SourceLocation = @"Source";
        private const string TemplateName = @"TotallyUniqueName123ABC";
        private readonly IAssemblyService _assemblyService;
        private readonly IDialogService _dialogService;

        private readonly string[] _linkFiles = new string[] {
            @"libopenal.1.dylib",
            @"libSDL2-2.0.0.dylib",
            @"MonoGame.Framework.dll.config",
            @"x64\libopenal.so.1",
            @"x64\libSDL2-2.0.so.0",
            @"x64\SDL2.dll",
            @"x64\soft_oal.dll",
            @"x86\libopenal.so.1",
            @"x86\libSDL2-2.0.so.0",
            @"x86\SDL2.dll",
            @"x86\soft_oal.dll"
        };

        private readonly ILoggingService _loggingService;

        private readonly string[] _referenceFiles = new string[] {
            @"Macabre2D.Framework.dll",
            @"Newtonsoft.Json.dll"
        };

        private readonly ISceneService _sceneService;
        private readonly Serializer _serializer;
        private Project _currentProject;
        private bool _hasChanges;

        public ProjectService(
            Serializer serializer,
            IAssemblyService assemblyService,
            IDialogService dialogService,
            ILoggingService loggingService,
            ISceneService sceneService) {
            this._serializer = serializer;
            this._assemblyService = assemblyService;
            this._dialogService = dialogService;
            this._loggingService = loggingService;
            this._sceneService = sceneService;
        }

        public Project CurrentProject {
            get {
                return this._currentProject;
            }

            private set {
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

        public bool HasChanges {
            get {
                return this._currentProject != null && (this._hasChanges || this._sceneService.HasChanges);
            }

            set {
                this.Set(ref this._hasChanges, value);
            }
        }

        public async Task<bool> BuildContent(BuildMode mode) {
            var referencePath = this.GetReferencePath();
            if (!Directory.Exists(referencePath)) {
                Directory.CreateDirectory(referencePath);
            }

            foreach (var file in this._referenceFiles) {
                File.Copy(file, Path.Combine(referencePath, file), true);
            }

            foreach (var configuration in this.CurrentProject.BuildConfigurations) {
                configuration.CopyMonoGameFrameworkDLL(referencePath, mode);
            }

            var result = true;
            await Task.Run(() => {
                this.GenerateContentFile(mode);

                var sourcePath = this.GetSourcePath();
                foreach (var configuration in this.CurrentProject.BuildConfigurations) {
                    var currentDirectory = Directory.GetCurrentDirectory();
                    var exitCode = -1;

                    var contentPath = configuration.GetContentPath(sourcePath);
                    var contentFilePath = Path.Combine(contentPath, "Content.mgcb");
                    var contentDirectory = Path.GetDirectoryName(contentFilePath);
                    var outputDirectory = Path.Combine(contentDirectory, "..", "bin", mode.ToString(), "Content");
                    Directory.CreateDirectory(outputDirectory);

                    exitCode = ContentBuilder.BuildContent(
                        out var exception,
                        $"/@:{contentFilePath}", $"/platform:{configuration.Platform.ToString()}",
                        $@"/outputDir:{outputDirectory}",
                        $"/workingDir:{contentDirectory}");

                    if (exitCode != 0) {
                        result = false;
                        this._loggingService.LogError($"Content could not be built for '{this.CurrentProject.Name}' in '{mode.ToString()}' mode: {exception.Message}");
                    }
                }
            });

            return result;
        }

        public async Task<bool> BuildProject(BuildMode mode) {
            var result = await this.BuildContent(mode);
            var tempDirectoryPath = this.GetTempDirectoryPath();

            if (result) {
                await Task.Run(async () => {
                    this.CurrentProject.GameSettings.StartupScenePath = Path.ChangeExtension(this.CurrentProject.StartUpSceneAsset?.GetContentPath(), null);

                    var properties = new Dictionary<string, string> {
                        { "Configuration", mode.ToString() }
                    };

                    var solutionPath = this.GetSolutionPath();
                    var buildParameters = new Microsoft.Build.Execution.BuildParameters();
                    var buildRequest = new Microsoft.Build.Execution.BuildRequestData(solutionPath, properties, null, new string[] { "Build" }, null);
                    var buildResult = Microsoft.Build.Execution.BuildManager.DefaultBuildManager.Build(buildParameters, buildRequest);
                    result &= buildResult.OverallResult == Microsoft.Build.Execution.BuildResultCode.Success;

                    if (result) {
                        if (mode == BuildMode.Debug) {
                            await FileHelper.DeleteDirectory(tempDirectoryPath, SecondsToAttemptDelete, true);
                            FileHelper.CopyDirectory(this.GetBinPath(true), tempDirectoryPath);
                        }
                    }
                    else if (!Directory.Exists(tempDirectoryPath)) {
                        Directory.CreateDirectory(tempDirectoryPath);
                        foreach (var file in this._referenceFiles) {
                            File.Copy(file, Path.Combine(tempDirectoryPath, file), true);
                        }
                    }
                });
            }

            await this._assemblyService.LoadAssemblies(tempDirectoryPath);
            return result;
        }

        public async Task<Project> CreateProject(string initialDirectory = null) {
            return await this.CreateProject(string.Empty, initialDirectory);
        }

        public async Task ExportProject() {
            if (await this.BuildProject(BuildMode.Release)) {
                var sourcePath = this.GetSourcePath();

                foreach (var configuration in this.CurrentProject.BuildConfigurations) {
                    var binaryFolderPath = configuration.GetBinaryFolderPath(sourcePath, BuildMode.Release);
                    var releaseLocation = Path.Combine(this.GetBinariesPath(), configuration.Platform.ToString());
                    await FileHelper.DeleteDirectory(releaseLocation, 10000, true);
                    FileHelper.CopyDirectory(binaryFolderPath, releaseLocation);
                }
            }
            else {
                this._dialogService.ShowWarningMessageBox("Build Failed", "The build failed so the project could not be exported.");
            }
        }

        public string GetBinPath(bool debug) {
            return Path.Combine(
                this.GetSourcePath(),
                GameplayName,
                BinName,
                debug ? DebugName : ReleaseName);
        }

        public string GetSourcePath() {
            return Path.Combine(this.CurrentProject.Directory, SourceLocation);
        }

        public async Task<Project> LoadProject(string location) {
            var shouldRestart = this._currentProject != null;
            var project = await Task.Run(() => this._serializer.Deserialize<Project>(location));
            project.PathToProject = location;
            project.Refresh();
            this.CurrentProject = project;

            if (shouldRestart) {
                var saveResult = this._dialogService.ShowSaveDiscardCancelDialog();
                if (saveResult != SaveDiscardCancelResult.Cancel) {
                    Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
            }
            else {
                await this.BuildProject(BuildMode.Debug);

                if (this.CurrentProject?.LastSceneOpened != null) {
                    await this._sceneService.LoadScene(this.CurrentProject, this.CurrentProject.LastSceneOpened);
                }
                else {
                    var scene = await this._sceneService.CreateScene();
                    this.CurrentProject.LastSceneOpened = scene.SceneAsset;
                    this._sceneService.HasChanges = true;
                }
            }

            return this.CurrentProject;
        }

        public void NavigateToProjectLocation() {
            Process.Start(this.CurrentProject.Directory);
        }

        public void OpenProjectInCodeEditor() {
            Process.Start(this.GetSolutionPath());
        }

        public async Task<bool> SaveProject() {
            if (string.IsNullOrEmpty(this.CurrentProject.PathToProject)) {
                return false;
            }

            await Task.Run(() => this.CurrentProject.SaveAssets());
            await this._sceneService.SaveCurrentScene(this.CurrentProject);
            await Task.Run(() => this._serializer.Serialize(this.CurrentProject, this.CurrentProject.PathToProject));
            this._currentProject.Refresh();
            this.HasChanges = false;
            this._currentProject.LastTimeSaved = DateTime.Now;
            return true;
        }

        public async Task<Project> SelectAndLoadProject(string initialDirectory = null) {
            if (this._dialogService.ShowFileBrowser(FileHelper.ProjectFilter, out var location, initialDirectory)) {
                await this.LoadProject(location);
            }

            return null;
        }

        internal async Task<Project> CreateProject(string copyFromDirectory, string initialDirectory = null) {
            if (this._dialogService.ShowCreateProjectDialog(out var project, initialDirectory)) {
                Directory.CreateDirectory(project.Directory);

                if (File.Exists(project.PathToProject)) {
                    if (!this._dialogService.ShowYesNoMessageBox("Project Already Exists", "A project already exists at the selected location. Overwrite?")) {
                        return null;
                    }

                    var backupPath = project.PathToProject + FileHelper.BackupExtension;
                    File.Delete(backupPath);
                    File.Copy(project.PathToProject, backupPath);
                }

                await Task.Run(() => this._serializer.Serialize(project, project.PathToProject));

                Directory.CreateDirectory(Path.Combine(project.Directory, AssetsLocation));
                Directory.CreateDirectory(Path.Combine(project.Directory, BinariesLocation));
                var dependenciesDirectory = Directory.CreateDirectory(Path.Combine(project.Directory, DependenciesLocation));
                Directory.CreateDirectory(Path.Combine(project.Directory, SettingsLocation));
                var sourceDirectory = Directory.CreateDirectory(Path.Combine(project.Directory, SourceLocation));

                var linksDirectory = Directory.CreateDirectory(Path.Combine(dependenciesDirectory.FullName, "Links"));
                Directory.CreateDirectory(Path.Combine(linksDirectory.FullName, "x64"));
                Directory.CreateDirectory(Path.Combine(linksDirectory.FullName, "x86"));
                foreach (var linkFile in this._linkFiles) {
                    File.Copy(string.IsNullOrEmpty(copyFromDirectory) ? linkFile : Path.Combine(copyFromDirectory, linkFile), Path.Combine(linksDirectory.FullName, linkFile));
                }

                var sourceFileName = "Source.zip";
                ZipFile.ExtractToDirectory(string.IsNullOrEmpty(copyFromDirectory) ? sourceFileName : Path.Combine(copyFromDirectory, sourceFileName), sourceDirectory.FullName);

                if (!string.Equals(project.SafeName, TemplateName, StringComparison.CurrentCultureIgnoreCase)) {
                    var files = new List<string>(Directory.GetFiles(sourceDirectory.FullName, "*.cs", SearchOption.AllDirectories));
                    files.AddRange(Directory.GetFiles(sourceDirectory.FullName, "*.csproj", SearchOption.AllDirectories));
                    files.AddRange(Directory.GetFiles(sourceDirectory.FullName, "*.sln", SearchOption.AllDirectories));

                    foreach (var file in files) {
                        var text = File.ReadAllText(file);
                        text = text.Replace(TemplateName, project.SafeName);
                        File.WriteAllText(file, text);

                        var newFile = file.Replace(TemplateName, project.SafeName);

                        if (!string.Equals(file, newFile, StringComparison.CurrentCultureIgnoreCase)) {
                            File.Delete(newFile);
                            File.Move(file, newFile);
                        }
                    }
                }

                var scene = await this._sceneService.CreateScene();
                scene.SceneAsset = new SceneAsset($"Default{FileHelper.SceneExtension}") {
                    Parent = project.AssetFolder
                };

                project.SceneAssets.Add(scene.SceneAsset);
                project.StartUpSceneAsset = scene.SceneAsset;
                project.LastSceneOpened = scene.SceneAsset;
                await this._sceneService.SaveCurrentScene(project);

                this.CurrentProject = project;
                await this.SaveProject();
                await this.LoadProject(project.PathToProject);
                this.HasChanges = true;

                return this.CurrentProject;
            }

            return null;
        }

        private void CurrentProject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.HasChanges = true;
        }

        private void GenerateContentFile(BuildMode mode) {
            var assets = this.CurrentProject.AssetFolder.GetAllContentAssets();
            var sourcePath = this.GetSourcePath();
            var dllPath = $@"{sourcePath}\{GameplayName}\bin\{mode.ToString()}\{this.CurrentProject.SafeName}.{GameplayName}.dll";
            foreach (var configuration in this.CurrentProject.BuildConfigurations) {
                configuration.GenerateContent(sourcePath, assets, this.CurrentProject.GameSettings, this._serializer, dllPath);
            }
        }

        private string GetBinariesPath() {
            return Path.Combine(this.CurrentProject.Directory, BinariesLocation);
        }

        private string GetReferencePath() {
            return Path.Combine(this.CurrentProject.Directory, DependenciesLocation, ReferencesLocation);
        }

        private string GetSolutionPath() {
            return Path.Combine(this.CurrentProject.Directory, SourceLocation, $"{this.CurrentProject.SafeName}{FileHelper.SolutionExtension}");
        }

        private string GetTempDirectoryPath() {
            return Path.Combine(this.CurrentProject.Directory, "temp", "bin");
        }
    }
}