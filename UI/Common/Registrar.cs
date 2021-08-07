namespace Macabresoft.Macabre2D.UI.Common {
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.AvaloniaInterop;
    using Macabresoft.Macabre2D.UI.Common.MonoGame;
    using Macabresoft.Macabre2D.UI.Common.Services;
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
                .RegisterType<IBuildService, BuildService>(new SingletonLifetimeManager())
                .RegisterType<IContentService, ContentService>(new SingletonLifetimeManager())
                .RegisterType<IEditorService, EditorService>(new SingletonLifetimeManager())
                .RegisterType<IEditorSettingsService, EditorSettingsService>(new SingletonLifetimeManager())
                .RegisterType<IEntityService, EntityService>(new SingletonLifetimeManager())
                .RegisterType<IFileSystemService, FileSystemService>(new SingletonLifetimeManager())
                .RegisterType<ILoggingService, LoggingService>(new SingletonLifetimeManager())
                .RegisterType<IPathService, PathService>(new SingletonLifetimeManager())
                .RegisterType<IPopupService, PopupService>(new SingletonLifetimeManager())
                .RegisterType<IProcessService, ProcessService>(new SingletonLifetimeManager())
                .RegisterType<IProjectService, ProjectService>(new SingletonLifetimeManager())
                .RegisterType<ISaveService, SaveService>(new SingletonLifetimeManager())
                .RegisterType<ISceneService, SceneService>(new SingletonLifetimeManager())
                .RegisterType<ISystemService, SystemService>(new SingletonLifetimeManager())
                .RegisterType<IUndoService, UndoService>(new SingletonLifetimeManager())
                .RegisterType<IValueEditorService, ValueEditorService>(new SingletonLifetimeManager());
        }

        /// <summary>
        /// Registers the required library types.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The container.</returns>
        public static IUnityContainer RegisterLibraryTypes(this IUnityContainer container) {
            return container.RegisterType<IAvaloniaGame, SceneEditorGame>(new PerResolveLifetimeManager())
                .RegisterType<ISceneEditor, SceneEditorGame>(new PerResolveLifetimeManager());
        }
    }
}