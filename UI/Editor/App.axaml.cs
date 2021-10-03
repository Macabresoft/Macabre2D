namespace Macabresoft.Macabre2D.UI.Editor {
    using System;
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Avalonia.Platform;
    using Macabresoft.Macabre2D.UI.Common;
    using Macabresoft.Macabre2D.UI.Editor.Views;
    using Unity;

    /// <summary>
    /// The main <see cref="Application" />.
    /// </summary>
    public class App : Application {
        private static readonly Lazy<bool> LazyShowNonNativeMenu = new(
            () => AvaloniaLocator.Current.GetService<IRuntimePlatform>().GetRuntimeInfo().OperatingSystem == OperatingSystemType.WinNT);

        /// <summary>
        /// Gets a value indicating whether or not the non-native menu should be shown. The native menu is for MacOS only.
        /// </summary>
        public static bool ShowNonNativeMenu => LazyShowNonNativeMenu.Value;

        /// <inheritdoc />
        public override void Initialize() {
            var container = new UnityContainer()
                .RegisterMappers()
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