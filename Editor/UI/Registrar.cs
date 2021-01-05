namespace Macabresoft.Macabre2D.Editor.UI {
    using Macabresoft.Macabre2D.Editor.AvaloniaInterop;
    using Macabresoft.Macabre2D.Editor.Library.Mappers;
    using Macabresoft.Macabre2D.Editor.Library.MonoGame;
    using Macabresoft.Macabre2D.Editor.UI.Mappers;
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
    }
}