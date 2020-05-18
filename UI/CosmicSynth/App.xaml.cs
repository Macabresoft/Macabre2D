namespace Macabre2D.UI.CosmicSynth {

    using log4net;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.CommonLibrary.Services;
    using Macabre2D.UI.CosmicSynth.Properties;
    using Macabre2D.UI.CosmicSynth.Views;
    using Macabre2D.UI.CosmicSynthLibrary.Controls.SongEditing;
    using Macabre2D.UI.CosmicSynthLibrary.Services;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Unity;
    using Unity.Lifetime;

    public partial class App : Application {
        private readonly IUnityContainer _container = new UnityContainer();
        private ILoggingService _loggingService;
        private MainWindow _mainWindow;
        private SettingsManager _settingsManager;

        protected override void OnExit(ExitEventArgs e) {
            Settings.Default.ClosedSuccessfully = e.ApplicationExitCode == 0;
            this.SaveSettings();
            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            ViewContainer.Instance = this._container;

            this.RegisterTypes();
            this.RegisterInstances();
            this.LoadMainWindow();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            this._loggingService.LogError(e.Exception?.Message ?? $"Macabre2D crashed for unknown reasons, but here's the stack trace: {Environment.NewLine}{Environment.StackTrace}");
            Settings.Default.ClosedSuccessfully = false;
            this.SaveSettings();
        }

        private void FullyLoadAssembly(Assembly assembly) {
            foreach (var assemblyName in assembly.GetReferencedAssemblies()) {
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                if (!loadedAssemblies.Any(a => a.FullName == assemblyName.FullName)) {
                    var loadedAssembly = Assembly.Load(assemblyName);
                    this.FullyLoadAssembly(loadedAssembly);
                }
            }
        }

        private void LoadMainWindow() {
            var splashScreen = new DraggableSplashScreen();
            splashScreen.Show();

            this._loggingService = this._container.Resolve<ILoggingService>();
            this._settingsManager = this._container.Resolve<SettingsManager>();

            this.DispatcherUnhandledException += this.App_DispatcherUnhandledException;

            var busyService = this._container.Resolve<IBusyService>();
            this._mainWindow = this._container.Resolve<MainWindow>();
            this._settingsManager.Initialize();
            splashScreen.Close();

            // If the application closes successfully, this will get set to true before settings are
            // saved again.
            Settings.Default.ClosedSuccessfully = false;
            Application.Current.MainWindow = this._mainWindow;

            this._mainWindow.Show();
        }

        private void RegisterInstances() {
            var log = LogManager.GetLogger(typeof(App));
            this._container.RegisterInstance(typeof(ILog), log, new ContainerControlledLifetimeManager());
            this._container.RegisterInstance(typeof(SettingsManager), this._container.Resolve<SettingsManager>(), new ContainerControlledLifetimeManager());
            this._container.RegisterInstance<IChangeDetectionService>(this._container.Resolve<ISongService>());
        }

        private void RegisterTypes() {
            this._container.RegisterType<IAssemblyService, AssemblyService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IBusyService, BusyService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<ILoggingService, LoggingService>();
            this._container.RegisterType<IUndoService, UndoService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IValueEditorService, ValueEditorService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<ICommonDialogService, CommonDialogService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<ISongService, SongService>(new ContainerControlledLifetimeManager());
            this._container.RegisterType<IPianoRoll, PianoRoll>(new ContainerControlledLifetimeManager());
        }

        private void SaveSettings() {
        }
    }
}