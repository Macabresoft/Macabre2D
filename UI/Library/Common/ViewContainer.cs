namespace Macabre2D.UI.Library.Common {

    using Unity;
    using Unity.Resolution;

    public static class ViewContainer {
        private static IUnityContainer _instance;

        public static IUnityContainer Instance {
            get {
                return _instance;
            }

            set {
                if (_instance == null) {
                    _instance = value;
                }
            }
        }

        public static T Resolve<T>(params ResolverOverride[] overrides) {
            return ViewContainer.Instance.Resolve<T>(overrides);
        }

        public static T Resolve<T>(string name, params ResolverOverride[] overrides) {
            return ViewContainer.Instance.Resolve<T>(name, overrides);
        }
    }
}