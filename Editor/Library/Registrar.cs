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
            return container.RegisterType<IAssemblyService, AssemblyService>(new SingletonLifetimeManager())
                .RegisterType<IContentService, ContentService>(new SingletonLifetimeManager())
                .RegisterType<IEditorService, EditorService>(new SingletonLifetimeManager())
                .RegisterType<ISelectionService, SelectionService>(new SingletonLifetimeManager())
                .RegisterType<ISceneService, SceneService>(new SingletonLifetimeManager())
                .RegisterType<IUndoService, UndoService>(new SingletonLifetimeManager())
                .RegisterType<IValueEditorService, ValueEditorService>(new SingletonLifetimeManager());
        }

        /// <summary>
        /// Registers the required types.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The container.</returns>
        public static IUnityContainer RegisterTypes(this IUnityContainer container) {
            return container.RegisterType<IAvaloniaGame, SceneEditor>(new PerResolveLifetimeManager())
                .RegisterType<ISceneEditor, SceneEditor>(new PerResolveLifetimeManager());
        }
    }
}