namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Unity;
using Unity.Lifetime;

/// <summary>
/// Registers services and instances to a <see cref="IUnityContainer" />.
/// </summary>
public static class Registrar {
    /// <summary>
    /// Registers the required framework types.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <returns>The container.</returns>
    public static IUnityContainer RegisterFrameworkTypes(this IUnityContainer container) {
        return container.RegisterType<IAssetManager, AssetManager>(new SingletonLifetimeManager())
            .RegisterType<ISerializer, Serializer>(new SingletonLifetimeManager());
    }

    /// <summary>
    /// Registers the services.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <returns>The container.</returns>
    public static IUnityContainer RegisterLibraryServices(this IUnityContainer container) {
        return container.RegisterType<IAssemblyService, AssemblyService>(new SingletonLifetimeManager())
            .RegisterType<IAssetSelectionService, AssetSelectionService>(new SingletonLifetimeManager())
            .RegisterType<IBuildService, BuildService>(new SingletonLifetimeManager())
            .RegisterType<IContentService, ContentService>(new SingletonLifetimeManager())
            .RegisterType<IEditorService, EditorService>(new SingletonLifetimeManager())
            .RegisterType<IEditorSettingsService, EditorSettingsService>(new SingletonLifetimeManager())
            .RegisterType<IEntityService, EntityService>(new SingletonLifetimeManager())
            .RegisterType<IPathService, PathService>(new SingletonLifetimeManager())
            .RegisterType<IProjectService, ProjectService>(new SingletonLifetimeManager())
            .RegisterType<ISaveService, SaveService>(new SingletonLifetimeManager())
            .RegisterType<ISceneService, SceneService>(new SingletonLifetimeManager())
            .RegisterType<ILoopService, LoopService>(new SingletonLifetimeManager())
            .RegisterType<IValueControlService, ValueControlService>(new SingletonLifetimeManager())
            .PerformRegistrations();
    }

    /// <summary>
    /// Registers the required library types.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <returns>The container.</returns>
    public static IUnityContainer RegisterLibraryTypes(this IUnityContainer container) {
        return container.RegisterType<IEditorGame, EditorGame>(new SingletonLifetimeManager());
    }
}