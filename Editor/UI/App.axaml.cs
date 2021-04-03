namespace Macabresoft.Macabre2D.Editor.UI {
    using System;
    using System.IO;
    using System.Reflection;
    using Avalonia;
    using Avalonia.Controls.ApplicationLifetimes;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Editor.Library;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors;
    using Macabresoft.Macabre2D.Editor.UI.Views;
    using Macabresoft.Macabre2D.Framework;
    using Unity;
    using Unity.Resolution;

    public class App : Application {
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

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                
                var projectService = Resolver.Resolve<IProjectService>();
                projectService.LoadProject();
                
                var mainWindow = new MainWindow();
                Resolver.Container.RegisterInstance(mainWindow);
                mainWindow.InitializeComponent();
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}