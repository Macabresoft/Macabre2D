namespace Macabresoft.Macabre2D.UI.Editor {
    using System;
    using Unity;
    using Unity.Resolution;

    /// <summary>
    /// A resolver to be used by views and controls to get registered objects.
    /// </summary>
    public static class Resolver {
        private static IUnityContainer _container;

        /// <summary>
        /// Gets or sets the container.
        /// </summary>
        /// <value>The container.</value>
        public static IUnityContainer Container {
            get => _container;

            internal set {
                if (_container == null) {
                    _container = value;
                }
                else {
                    throw new NotSupportedException("Cannot instantiate the container twice.");
                }
            }
        }

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
}