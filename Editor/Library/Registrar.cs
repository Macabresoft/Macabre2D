namespace Macabresoft.Macabre2D.Editor.Library {
    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame;
    using Macabresoft.Macabre2D.Editor.Library.Services;
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
            return container.RegisterType<ISerializer, Serializer>(new SingletonLifetimeManager());
        }

        /// <summary>
        /// Registers the services.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The container.</returns>
        public static IUnityContainer RegisterLibraryServices(this IUnityContainer container) {
            return container.RegisterType<IAssemblyService, AssemblyService>(new SingletonLifetimeManager())
                .RegisterType<IEditorService, EditorService>(new SingletonLifetimeManager())
                .RegisterType<IFileSystemService, FileSystemService>(new SingletonLifetimeManager())
                .RegisterType<IProcessService, ProcessService>(new SingletonLifetimeManager())
                .RegisterType<IProjectService, ProjectService>(new SingletonLifetimeManager())
                .RegisterType<ISelectionService, SelectionService>(new SingletonLifetimeManager())
                .RegisterType<ISceneService, SceneService>(new SingletonLifetimeManager())
                .RegisterType<IUndoService, UndoService>(new SingletonLifetimeManager())
                .RegisterType<IValueEditorService, ValueEditorService>(new SingletonLifetimeManager());
        }

        /// <summary>
        /// Registers the required library types.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The container.</returns>
        public static IUnityContainer RegisterLibraryTypes(this IUnityContainer container) {
            return container.RegisterType<IAvaloniaGame, SceneEditor>(new PerResolveLifetimeManager())
                .RegisterType<ISceneEditor, SceneEditor>(new PerResolveLifetimeManager());
        }
    }
}