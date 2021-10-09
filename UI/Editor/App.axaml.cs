namespace Macabresoft.Macabre2D.UI.Editor {
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
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
                var mainWindow = new MainWindow();
                Resolver.Container.RegisterInstance(mainWindow);

                // TODO: show a splash screen while all this is happening
                Resolver.Resolve<IEditorSettingsService>().Initialize();

                var projectService = Resolver.Resolve<IProjectService>();
                projectService.LoadProject();

                Resolver.Resolve<IContentService>().Initialize();
                Resolver.Resolve<IEntityService>().Initialize();
                Resolver.Resolve<ISystemService>().Initialize();


                mainWindow.InitializeComponent();
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}