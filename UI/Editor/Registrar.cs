namespace Macabresoft.Macabre2D.UI.Editor;

using Macabresoft.Macabre2D.UI.Common;
using Unity;
using Unity.Lifetime;

/// <summary>
/// Registers required types to the <see cref="IUnityContainer" />.
/// </summary>
public static class Registrar {
    /// <summary>
    /// Registers services to the container.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <returns>The container.</returns>
    public static IUnityContainer RegisterServices(this IUnityContainer container) {
        return container
            .RegisterType<ILocalDialogService, LocalDialogService>(new SingletonLifetimeManager())
            .RegisterType<ICommonDialogService, ILocalDialogService>();
    }
}