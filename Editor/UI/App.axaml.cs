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
                var contentService = Resolver.Resolve<IContentService>();
                
                // TODO: rework this to not be hard coded when multiple projects are supported
                var assemblyDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new NotSupportedException();
                var projectFolder = Path.Combine(assemblyDirectoryPath, "..", "..", "..", "..", "..", "Project");
                var projectFilePath = Path.Combine(projectFolder, GameProject.ProjectFileName);
                var project = File.Exists(projectFilePath) ? projectService.LoadProject(projectFilePath) : projectService.CreateProject(projectFolder);
                var contentDirectoryPath = Path.Combine(projectFolder, GameProject.ContentDirectoryName);
                contentService.Initialize(contentDirectoryPath, project.Assets);
                
                var mainWindow = new MainWindow();
                Resolver.Container.RegisterInstance(mainWindow);
                mainWindow.InitializeComponent();
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}