namespace Macabresoft.Macabre2D.Editor.UI {

    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Editor.Library;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors;
    using Macabresoft.Macabre2D.Editor.UI.Views;
    using Unity;
    using Unity.Resolution;

    public class App : Application {
        public IUnityContainer Container { get; private set; }

        public override void Initialize() {
            this.Container = new UnityContainer()
                .RegisterMappers()
                .RegisterServices()
                .RegisterLibraryServices()
                .RegisterLibraryTypes()
                .RegisterFrameworkTypes();

            Resolver.Container = this.Container;

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                var mainWindow = new MainWindow();
                Resolver.Container.RegisterInstance(mainWindow);
                mainWindow.InitializeComponent();
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}