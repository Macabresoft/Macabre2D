namespace Macabresoft.Macabre2D.UI.Editor {
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;
    using Unity;

    /// <summary>
    /// The main <see cref="Application" />.
    /// </summary>
    public class App : Application {
        /// <inheritdoc />
        public override void Initialize() {
            var container = new UnityContainer()
                .RegisterServices()
                .RegisterLibraryServices()
                .RegisterLibraryTypes()
                .RegisterFrameworkTypes();

            Resolver.Container = container;

            AvaloniaXamlLoader.Load(this);
        }

        /// <inheritdoc />
        public override void OnFrameworkInitializationCompleted() {
            if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                BaseGame.IsDesignMode = true;
                var mainWindow = new MainWindow();
                Resolver.Container.RegisterInstance(mainWindow);

                // TODO: show a splash screen while all this is happening
                Resolver.Resolve<IEditorSettingsService>().Initialize();
                Resolver.Resolve<IProjectService>().LoadProject();
                
                if (Resolver.Resolve<ISceneService>().CurrentScene == null) {
                    Resolver.Resolve<IEditorService>().SelectedTab = EditorTabs.Project;
                }

                mainWindow.InitializeComponent();
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}