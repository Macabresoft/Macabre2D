namespace Macabresoft.Macabre2D.Editor.Library {
    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Unity;
    using Unity.Lifetime;

    /// <summary>
    /// Registers services and instances to a <see cref="IUnityContainer" />.
    /// </summary>
    public static class Registrar {

        /// <summary>
        /// Registers the services.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The container.</returns>
        public static IUnityContainer RegisterServices(this IUnityContainer container) {
            container.RegisterType<IContentService, ContentService>(new SingletonLifetimeManager());
            container.RegisterType<IEditorService, EditorService>(new SingletonLifetimeManager());
            container.RegisterType<ISelectionService, SelectionService>(new SingletonLifetimeManager());
            container.RegisterType<ISceneService, SceneService>(new SingletonLifetimeManager());
            container.RegisterType<IUndoService, UndoService>(new SingletonLifetimeManager());
            return container;
        }

        /// <summary>
        /// Registers the required types.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The container.</returns>
        public static IUnityContainer RegisterTypes(this IUnityContainer container) {
            container.RegisterType<IAvaloniaGame, SceneEditor>(new PerResolveLifetimeManager());
            container.RegisterType<ISceneEditor, SceneEditor>(new PerResolveLifetimeManager());
            return container;
        }
    }
}