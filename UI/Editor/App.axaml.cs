namespace Macabre2D.UI.Editor;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabre2D.UI.Common;
using Macabre2D.UI.Editor.Views.Dialogs;
using Unity;

/// <summary>
/// The main <see cref="Application" />.
/// </summary>
public class App : Application {
    /// <inheritdoc />
    public override void Initialize() {
        Resolver.Container
            .AddNewExtension<AvaloniaUnityContainerExtension>()
            .AddNewExtension<CommonContainerExtension>()
            .AddNewExtension<EditorContainerExtension>();

        AvaloniaXamlLoader.Load(this);
    }

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted() {
        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
            var splashScreen = new SplashScreen();
            desktop.MainWindow = splashScreen;
            splashScreen.Show();
            var mainWindow = new MainWindow();

            Resolver.Container.RegisterInstance(mainWindow);
            var editorSettingsService = Resolver.Resolve<IEditorSettingsService>();
            editorSettingsService.Initialize();
            Resolver.Resolve<IProjectService>().LoadProject();

            if (Resolver.Resolve<ISceneService>().CurrentlyEditing == null) {
                Resolver.Resolve<IEditorService>().SelectedTab = EditorTabs.Project;
            }

            mainWindow.Initialize();
            desktop.MainWindow = mainWindow;
            mainWindow.Show();
            splashScreen.Close();
            mainWindow.WindowState = editorSettingsService.Settings.WindowState;
        }

        base.OnFrameworkInitializationCompleted();
    }
}