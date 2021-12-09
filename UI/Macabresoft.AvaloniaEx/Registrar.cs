namespace Macabresoft.AvaloniaEx;

using Unity;
using Unity.Lifetime;

/// <summary>
/// Registers services and instances to a <see cref="IUnityContainer" />.
/// </summary>
public static class Registrar {
    /// <summary>
    /// Performs common registrations.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <returns>The container.</returns>
    public static IUnityContainer PerformRegistrations(this IUnityContainer container) {
        return container.RegisterType<IFileSystemService, FileSystemService>(new SingletonLifetimeManager())
            .RegisterType<ILoggingService, LoggingService>(new SingletonLifetimeManager())
            .RegisterType<IProcessService, ProcessService>(new SingletonLifetimeManager())
            .RegisterType<IUndoService, UndoService>(new SingletonLifetimeManager());
    }
}