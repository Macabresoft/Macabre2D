namespace Macabresoft.MonoGame.AvaloniaUI {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Very empty implementation of <see cref="IServiceProvider" />.
    /// </summary>
    /// <seealso cref="System.IServiceProvider" />
    public class MonoGameServiceProvider : IServiceProvider {
        private readonly Dictionary<Type, object> _services;

        /// <inheritdoc />
        public MonoGameServiceProvider() {
            this._services = new Dictionary<Type, object>();
        }

        /// <inheritdoc />
        public void AddService(Type type, object provider) {
            this._services.Add(type, provider);
        }

        /// <inheritdoc />
        public void AddService<T>(T service) {
            this.AddService(typeof(T), service);
        }

        /// <inheritdoc />
        public object GetService(Type type) {
            this._services.TryGetValue(type, out var service);
            return service;
        }

        /// <inheritdoc />
        public T GetService<T>() where T : class {
            var service = this.GetService(typeof(T));
            return (T)service;
        }

        /// <inheritdoc />
        public void RemoveService(Type type) {
            this._services.Remove(type);
        }
    }
}