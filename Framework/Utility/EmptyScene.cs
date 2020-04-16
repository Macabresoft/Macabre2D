namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An empty scene that exists for scenarios where the current scene has not been initialized.
    /// Components will reference an <see cref="EmptyScene"/> and it will perform zero actions.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IScene"/>
    internal sealed class EmptyScene : IScene, IDisposable {
        private static EmptyScene _instance = new EmptyScene();

        private bool _disposedValue = false;

        /// <summary>
        /// Initializes the <see cref="EmptyScene"/> class.
        /// </summary>
        static EmptyScene() {
        }

        private EmptyScene() {
        }

#pragma warning disable CS0414

        /// <inheritdoc/>
        public event EventHandler<BaseComponent> ComponentCreated;

        /// <inheritdoc/>
        public event EventHandler<BaseComponent> ComponentDestroyed;

        /// <inheritdoc/>
        public event EventHandler<BaseModule> ModuleAdded;

        /// <inheritdoc/>
        public event EventHandler<BaseModule> ModuleRemoved;

#pragma warning restore CS0414

        /// <inheritdoc/>
        public static IScene Instance {
            get {
                return EmptyScene._instance;
            }
        }

        /// <inheritdoc/>
        public Guid AssetId {
            get {
                return Guid.NewGuid();
            }

            set {
                return;
            }
        }

        /// <inheritdoc/>
        public Color BackgroundColor {
            get {
                return default;
            }

            set {
                return;
            }
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<BaseComponent> Components {
            get {
                return new List<BaseComponent>();
            }
        }

        /// <inheritdoc/>
        public bool IsInitialized {
            get {
                return default;
            }
        }

        /// <inheritdoc/>
        public string Name {
            get {
                return default;
            }
        }

        /// <inheritdoc/>
        public T AddComponent<T>() where T : BaseComponent, new() {
            return default;
        }

        /// <inheritdoc/>
        public bool AddComponent(BaseComponent component) {
            return default;
        }

        /// <inheritdoc/>
        public bool AddModule(BaseModule module) {
            return default;
        }

        /// <inheritdoc/>
        public bool AddModule(FixedTimeStepModule module, float timeStep) {
            return default;
        }

        /// <inheritdoc/>
        public T CreateModule<T>() where T : BaseModule, new() {
            return default;
        }

        /// <inheritdoc/>
        public T CreateModule<T>(float timeStep) where T : FixedTimeStepModule, new() {
            return default;
        }

        /// <inheritdoc/>
        public void DestroyComponent(BaseComponent component) {
            return;
        }

        /// <inheritdoc/>
        public void Dispose() {
            this.Dispose(true);
        }

        /// <inheritdoc/>
        public void Draw(FrameTime frameTime) {
            return;
        }

        /// <inheritdoc/>
        public void Draw(FrameTime frameTime, params Camera[] cameras) {
            return;
        }

        /// <inheritdoc/>
        public BaseComponent FindComponent(string name) {
            return default;
        }

        /// <inheritdoc/>
        public T FindComponentOfType<T>() where T : BaseComponent {
            return default;
        }

        /// <inheritdoc/>
        public BaseModule FindModule(string name) {
            return default;
        }

        /// <inheritdoc/>
        public List<T> GetAllComponentsOfType<T>() {
            return new List<T>();
        }

        /// <inheritdoc/>
        public T GetModule<T>() where T : BaseModule {
            return default;
        }

        /// <inheritdoc/>
        public IEnumerable<T> GetModules<T>() where T : BaseModule {
            return new List<T>();
        }

        /// <inheritdoc/>
        public IEnumerable<IDrawableComponent> GetVisibleDrawableComponents() {
            return new List<IDrawableComponent>();
        }

        /// <inheritdoc/>
        public void Initialize() {
            return;
        }

        /// <inheritdoc/>
        public void LoadContent() {
            return;
        }

        /// <inheritdoc/>
        public void QueueEndOfFrameAction(Action<FrameTime> action) {
            return;
        }

        /// <inheritdoc/>
        public void RemoveModule(BaseModule module) {
            return;
        }

        /// <inheritdoc/>
        public T ResolveDependency<T>() where T : new() {
            return default;
        }

        /// <inheritdoc/>
        public T ResolveDependency<T>(Func<T> objectFactory) {
            return default;
        }

        /// <inheritdoc/>
        public void Update(FrameTime frameTime) {
            return;
        }

        private void Dispose(bool disposing) {
            if (!this._disposedValue) {
                ComponentCreated = null;
                ComponentDestroyed = null;
                ModuleAdded = null;
                ModuleRemoved = null;
                this._disposedValue = true;
            }
        }
    }
}