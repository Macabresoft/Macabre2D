namespace Macabre2D.UI.Editor {

    using log4net;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.ServiceInterfaces;
    using Macabre2D.UI.Services;
    using Macabre2D.UI.Views;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Unity;
    using Unity.Lifetime;

    public partial class App : Application {
        private readonly IUnityContainer _container = new UnityContainer();
        private MainWindow _mainWindow;

        protected override void OnExit(ExitEventArgs e) {
            var dialogService = this._container.Resolve<IDialogService>();
            var settingsManager = this._container.Resolve<SettingsManager>();

            var lastOpenTabName = string.Empty;
            if (this._mainWindow.MainTabControl.SelectedItem is TabItem tabItem) {
                lastOpenTabName = tabItem.Header as string;
            }

            settingsManager.Save(lastOpenTabName);
            base.OnExit(e);
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e) {
            base.OnSessionEnding(e);
        }

        protected override async void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            ViewContainer.Instance = this._container;

            this.RegisterTypes();
            await this.LoadMainWindow();
        }

        private async Task LoadMainWindow() {
            var splashScreen = new DraggableSplashScreen();
            splashScreen.ProgressText = "Loading...";
            splashScreen.Show();
            var busyService = this._container.Resolve<IBusyService>();
            var projectService = this._container.Resolve<IProjectService>();
            splashScreen.ProgressText = "Loading project...";
            await busyService.PerformTask(projectService.LoadProject(), true); // TODO: show a splash screen while this is going

            splashScreen.ProgressText = $"{projectService.CurrentProject.Name} loaded!";
            this._mainWindow = this._container.Resolve<MainWindow>();
            var sceneService = this._container.Resolve<ISceneService>();
            if (sceneService?.CurrentScene?.HasChanges != true) {
                sceneService.HasChanges = false;
            }

            splashScreen.ProgressText = "Loading user preferences...";
            var settingsManager = this._container.Resolve<SettingsManager>();
            settingsManager.Initialize();
            var tabName = settingsManager.GetLastOpenTabName();
            var tabs = this._mainWindow.MainTabControl.Items.Cast<TabItem>();
            var selectedTab = tabs.FirstOrDefault(x => x.Header as string == tabName);

            if (selectedTab != null) {
                this._mainWindow.MainTabControl.SelectedItem = selectedTab;
            }

            splashScreen.ProgressText = "Done!";
            splashScreen.Close();
            this._mainWindow.Show();
        }

        private void RegisterTypes() {
            var log = LogManager.GetLogger(typeof(App));
            this._container.RegisterInstance(typeof(ILog), log, new ContainerControlledLifetimeManager());
            this._container.RegisterType<IAssemblyService, AssemblyService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IAssetService, AssetService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IBusyService, BusyService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IComponentService, ComponentService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IDialogService, DialogService>();
            this._container.RegisterType<IFileService, FileService>();
            this._container.RegisterType<ILoggingService, LoggingService>();
            this._container.RegisterType<IMonoGameService, MonoGameService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IProjectService, ProjectService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<ISceneService, SceneService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IStatusService, StatusService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IUndoService, UndoService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IValueEditorService, ValueEditorService>(new ContainerControlledLifetimeManager());
        }
    }
}