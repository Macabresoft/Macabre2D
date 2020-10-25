namespace Macabresoft.Macabre2D.Editor.Framework {

    using Macabresoft.Macabre2D.Editor.Framework.Services;
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
        public static void RegisterServices(this IUnityContainer container) {
            container.RegisterType<IContentService, ContentService>(new SingletonLifetimeManager());
            container.RegisterType<ISceneService, SceneService>(new SingletonLifetimeManager());
        }
    }
}