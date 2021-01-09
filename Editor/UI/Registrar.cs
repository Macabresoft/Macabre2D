namespace Macabresoft.Macabre2D.Editor.UI {
    using Macabresoft.Macabre2D.Editor.Library.Mappers;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Editor.UI.Mappers;
    using Macabresoft.Macabre2D.Editor.UI.Services;
    using Unity;
    using Unity.Lifetime;

    public static class Registrar {
        /// <summary>
        /// Registers the required types.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The container.</returns>
        public static IUnityContainer RegisterMappers(this IUnityContainer container) {
            return container.RegisterType<IValueEditorTypeMapper, ValueEditorTypeMapper>(new SingletonLifetimeManager());
        }

        /// <summary>
        /// Registers services to the container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The container.</returns>
        public static IUnityContainer RegisterServices(this IUnityContainer container) {
            return container.RegisterType<IDialogService, DialogService>(new SingletonLifetimeManager());
        }
    }
}