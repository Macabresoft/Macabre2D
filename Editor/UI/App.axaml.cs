namespace Macabresoft.Macabre2D.Editor.UI {

    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Editor.Framework;
    using Macabresoft.Macabre2D.Editor.UI.Views;
    using Unity;

    public class App : Application {
        public IUnityContainer Container { get; private set; }

        public override void Initialize() {
            this.Container = new UnityContainer()
                .RegisterServices()
                .RegisterTypes();

            Resolver.Container = this.Container;

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}