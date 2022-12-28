namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Macabresoft.Macabre2D.UI.Common.DisplayNameHandlers;
using Macabresoft.Macabre2D.UI.Editor.Views.Dialogs;
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
            splashScreen.Show();
            BaseGame.IsDesignMode = true;
            var mainWindow = new MainWindow();
            Resolver.Container.RegisterInstance(mainWindow);

            Resolver.Resolve<IEditorSettingsService>().Initialize();
            Resolver.Resolve<IProjectService>().LoadProject();
            
            DisplayNameHelper.Instance.RegisterHandler(typeof(InputAction), Resolver.Resolve<InputActionsDisplayNameHandler>());
            DisplayNameHelper.Instance.RegisterHandler(typeof(Layers), Resolver.Resolve<LayersDisplayNameHandler>());

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