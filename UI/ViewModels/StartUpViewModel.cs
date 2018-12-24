namespace Macabre2D.UI.ViewModels {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public sealed class StartupViewModel {
        private readonly IBusyService _busyService;
        private readonly ISceneService _sceneService;

        public StartupViewModel(IBusyService busyService, IProjectService projectService, ISceneService sceneService) {
            this._busyService = busyService;
            this.ProjectService = projectService;
            this._sceneService = sceneService;
            this.OpenProjectCommand = new RelayCommand(async () => await this.OpenProject());
        }

        public ICommand OpenProjectCommand { get; }

        public IProjectService ProjectService { get; }

        private async Task OpenProject() {
            await this._busyService.PerformTask(this.ProjectService.SelectAndLoadProject(FileHelper.DefaultProjectPath));
        }
    }
}