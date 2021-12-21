namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Macabresoft.Macabre2D.UI.Editor.Views.Dialogs;
using Unity;

/// <summary>
/// The main <see cref="Application" />.
/// </summary>
public class App : Application {
    /// <inheritdoc />
    public override void Initialize() {
        Resolver.Container.RegisterServices()
            .RegisterLibraryServices()
            .RegisterLibraryTypes()
            .RegisterFrameworkTypes();

        AvaloniaXamlLoader.Load(this);
    }

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted() {
        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var splashScreen = new SplashScreen();
            splashScreen.Show();
            BaseGame.IsDesignMode = true;
            var mainWindow = new MainWindow();
            Resolver.Container.RegisterInstance(mainWindow);

            Resolver.Resolve<IEditorSettingsService>().Initialize();
            Resolver.Resolve<IProjectService>().LoadProject();

            if (Resolver.Resolve<ISceneService>().CurrentScene == null) {
                Resolver.Resolve<IEditorService>().SelectedTab = EditorTabs.Project;
            }

            mainWindow.InitializeComponent();
            desktop.MainWindow = mainWindow;
            splashScreen.Close();
        }

        base.OnFrameworkInitializationCompleted();
    }
}