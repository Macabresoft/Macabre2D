namespace Macabre2D.UI.Editor;

using System;
using System.Reflection;
using Avalonia;
using ReactiveUI;
using ReactiveUI.Avalonia;

internal static class Program {
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI(rxAppBuilder =>
            {
                // Enable ReactiveUI
                rxAppBuilder
                    .WithViewsFromAssembly(Assembly.GetExecutingAssembly())
                    .WithRegistration(locator =>
                    {
                        // Register your services here
                        locator.RegisterLazySingleton<IScreen>(() => new MainViewModel());
                    });
            }).RegisterReactiveUIViewsFromEntryAssembly();

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }
}