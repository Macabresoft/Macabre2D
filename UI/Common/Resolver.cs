namespace Macabre2D.UI.Common;

using Unity;
using Unity.Resolution;

/// <summary>
/// A resolver to be used by views and controls to get registered objects.
/// </summary>
public static class Resolver {
    /// <summary>
    /// Gets or sets the container.
    /// </summary>
    /// <value>The container.</value>
    public static IUnityContainer Container { get; } = new UnityContainer();

    /// <summary>
    /// Resolve the requested type using the specified overrides.
    /// </summary>
    /// <typeparam name="T">The type to resolve.</typeparam>
    /// <param name="overrides">The overrides.</param>
    /// <returns>The resolved object.</returns>
    public static T Resolve<T>(params ResolverOverride[] overrides) {
        return Container.Resolve<T>(overrides);
    }

    /// <summary>
    /// Resolve the requested type with the specified name using the specified overrides.
    /// </summary>
    /// <typeparam name="T">The type to resolve.</typeparam>
    /// <param name="name">The name.</param>
    /// <param name="overrides">The overrides.</param>
    /// <returns>The resolved object.</returns>
    public static T Resolve<T>(string name, params ResolverOverride[] overrides) {
        return Container.Resolve<T>(name, overrides);
    }
}