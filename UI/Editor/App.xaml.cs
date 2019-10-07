namespace Macabre2D.UI.Editor {

    using log4net;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Editor.Properties;
    using Macabre2D.UI.ServiceInterfaces;
    using Macabre2D.UI.Services;
    using Macabre2D.UI.Views;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Unity;
    using Unity.Lifetime;

    public partial class App : Application {
        private readonly IUnityContainer _container = new UnityContainer();
        private ILoggingService _loggingService;
        private MainWindow _mainWindow;
        private IProjectService _projectService;
        private SettingsManager _settingsManager;

        protected override void OnExit(ExitEventArgs e) {
            Settings.Default.ClosedSuccessfully = e.ApplicationExitCode == 0;
            this.SaveSettings();
            base.OnExit(e);
        }

        protected override async void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            ViewContainer.Instance = this._container;

            this.RegisterTypes();
            this.RegisterInstances();
            await this.LoadMainWindow();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            this._loggingService.LogError(e.Exception?.Message ?? $"Macabre2D crashed for unknown reasons, but here's the stack trace: {Environment.NewLine}{Environment.StackTrace}");
            Settings.Default.ClosedSuccessfully = false;
            this.SaveSettings();
        }

        private async Task LoadMainWindow() {
            var splashScreen = new DraggableSplashScreen();
            splashScreen.ProgressText = "Loading...";
            splashScreen.Show();

            this._loggingService = this._container.Resolve<ILoggingService>();
            this._settingsManager = this._container.Resolve<SettingsManager>();

            this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;

            splashScreen.ProgressText = "Loading project...";

            var busyService = this._container.Resolve<IBusyService>();
            this._projectService = this._container.Resolve<IProjectService>();

            if (!Settings.Default.ClosedSuccessfully && this._projectService.GetAutoSaveFiles().Any()) {
                var dialogService = this._container.Resolve<IDialogService>();
                if (dialogService.ShowSelectProjectDialog(out var fileInfo)) {
                    await busyService.PerformTask(this._projectService.LoadProject(fileInfo.FullName), true);

                    if (FileHelper.IsAutoSave(fileInfo.Name)) {
                        this._projectService.HasChanges = true;
                    }
                }
            }

            // If last shutdown was fine or a project wasn't selected above, we just load this normally.
            if (this._projectService.CurrentProject == null) {
                await busyService.PerformTask(this._projectService.LoadProject(), true);
            }

            splashScreen.ProgressText = $"{this._projectService.CurrentProject.Name} loaded!";
            this._mainWindow = this._container.Resolve<MainWindow>();
            var sceneService = this._container.Resolve<ISceneService>();
            if (sceneService?.CurrentScene?.HasChanges != true) {
                sceneService.HasChanges = false;
            }

            splashScreen.ProgressText = "Loading user preferences...";

            this._settingsManager.Initialize();
            var tabName = this._settingsManager.GetLastOpenTabName();
            var tabs = this._mainWindow.MainTabControl.Items.Cast<TabItem>();
            var selectedTab = tabs.FirstOrDefault(x => x.Header as string == tabName);
            var autoSaveService = this._container.Resolve<IAutoSaveService>();
            autoSaveService.Initialize(Settings.Default.NumberOfAutoSaves, Settings.Default.AutoSaveIntervalInMinutes);

            if (selectedTab != null) {
                this._mainWindow.MainTabControl.SelectedItem = selectedTab;
            }

            splashScreen.ProgressText = "Done!";
            splashScreen.Close();

            // If the application closes successfully, this will get set to true before settings are
            // saved again.
            Settings.Default.ClosedSuccessfully = false;
            this._settingsManager.Save(this._settingsManager.GetLastOpenTabName());
            this._mainWindow.Show();
        }

        private void RegisterInstances() {
            var log = LogManager.GetLogger(typeof(App));
            this._container.RegisterInstance(typeof(ILog), log, new ContainerControlledLifetimeManager());
            this._container.RegisterInstance(typeof(SettingsManager), this._container.Resolve<SettingsManager>(), new ContainerControlledLifetimeManager());
        }

        private void RegisterTypes() {
            this._container.RegisterType<IAssemblyService, AssemblyService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IAssetService, AssetService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IAutoSaveService, AutoSaveService>(new ContainerControlledLifetimeManager());
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

        private void SaveSettings() {
            var lastOpenTabName = string.Empty;
            if (this._mainWindow.MainTabControl.SelectedItem is TabItem tabItem) {
                lastOpenTabName = tabItem.Header as string;
            }

            this._settingsManager.Save(lastOpenTabName);
        }
    }
}