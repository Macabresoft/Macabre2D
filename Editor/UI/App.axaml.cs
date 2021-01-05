namespace Macabresoft.Macabre2D.Editor.UI {

    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Editor.Library;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors;
    using Macabresoft.Macabre2D.Editor.UI.Views;
    using Unity;

    public class App : Application {
        public IUnityContainer Container { get; private set; }

        public override void Initialize() {
            this.Container = new UnityContainer()
                .RegisterServices()
                .RegisterTypes();

            Resolver.Container = this.Container;

            var valueEditorService = Resolver.Resolve<IValueEditorService>();
            valueEditorService.Initialize(typeof(EnumEditor), typeof(GenericValueEditor));

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