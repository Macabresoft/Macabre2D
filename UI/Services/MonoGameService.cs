namespace Macabre2D.UI.Services {

    using Macabre2D.UI.Controls.SceneEditing;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Linq;
    using System.Windows;

    public sealed class MonoGameService : IMonoGameService {
        private readonly IProjectService _projectService;
        private readonly SceneEditor _sceneEditor;
        private readonly ISceneService _sceneService;

        public MonoGameService(SceneEditor sceneEditor, IProjectService projectService, ISceneService sceneService) {
            this._sceneEditor = sceneEditor;
            this._sceneService = sceneService;
            this._sceneService.PropertyChanged += this.SceneService_PropertyChanged;
            this._sceneEditor.CurrentScene = this._sceneService.CurrentScene?.Scene;
            this._projectService = projectService;
            this._projectService.PropertyChanged += this.ProjectService_PropertyChanged;
            this._sceneEditor.GameSettings = this._projectService.CurrentProject?.GameSettings;
            this.SetContentPath();
        }

        public DependencyObject SceneEditor {
            get {
                return this._sceneEditor;
            }
        }

        public void ResetCamera() {
            this._sceneEditor.ResetCamera();
        }

        private void ProjectService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this._projectService.CurrentProject)) {
                this._sceneEditor.GameSettings = this._projectService.CurrentProject?.GameSettings;
                this.SetContentPath();
            }
        }

        private void SceneService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this._sceneService.CurrentScene)) {
                this._sceneEditor.CurrentScene = this._sceneService.CurrentScene?.Scene;
            }
        }

        private void SetContentPath() {
            if (this._projectService.CurrentProject != null) {
                var configurationName = "Debug"; //TODO : allow release
                var desktopBuildConfiguration = this._projectService.CurrentProject.BuildConfigurations.FirstOrDefault(x => x.Platform == BuildPlatform.DesktopGL);
                this._sceneEditor.SetContentPath(desktopBuildConfiguration.GetCompiledContentPath(this._projectService.GetSourcePath(), configurationName));
            }
        }
    }
}