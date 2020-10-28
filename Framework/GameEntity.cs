namespace Macabresoft.Macabre2D.Framework {

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for a <see cref="ITransformable" /> descendent of <see cref="IScene" /> which
    /// holds a collection of <see cref="IGameComponent" />.
    /// </summary>
    public interface IGameEntity : ITransformable, INotifyPropertyChanged {

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        IReadOnlyCollection<IGameEntity> Children { get => new IGameEntity[0]; }

        /// <summary>
        /// Gets the components.
        /// </summary>
        /// <value>The components.</value>
        IReadOnlyCollection<IGameComponent> Components { get => new IGameComponent[0]; }

        /// <summary>
        /// Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the layers.
        /// </summary>
        /// <value>The layers.</value>
        Layers Layers { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        IGameEntity Parent { get => GameScene.Empty; }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        IGameScene Scene { get => GameScene.Empty; }

        /// <summary>
        /// Adds a child of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// An object that implements <see cref="IGameEntity" /> and has an empty constructor.
        /// </typeparam>
        /// <returns>The added child.</returns>
        T AddChild<T>() where T : IGameEntity, new();

        /// <summary>
        /// Adds a child of this entity's default child type.
        /// </summary>
        /// <returns>The added child.</returns>
        IGameEntity AddChild();

        /// <summary>
        /// Adds an existing <see cref="IGameEntity" /> as a child.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void AddChild(IGameEntity entity);

        /// <summary>
        /// Adds a new component of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// A type that implements <see cref="IGameComponent" /> and has an empty constructor.
        /// </typeparam>
        /// <returns>The added component.</returns>
        T AddComponent<T>() where T : IGameComponent, new();

        /// <summary>
        /// Adds the component.
        /// </summary>
        /// <param name="component">The component.</param>
        void AddComponent(IGameComponent component);

        /// <summary>
        /// Gets the component of the specified type if it exists; otherwise, adds the component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The component that was found or added.</returns>
        T GetOrAddComponent<T>() where T : class, IGameComponent, new();

        /// <summary>
        /// Initializes this entity as a descendent of <paramref name="scene" /> and <paramref
        /// name="parent" />.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="parent">The parent.</param>
        void Initialize(IGameScene scene, IGameEntity parent);

        /// <summary>
        /// Determines whether this instance is a descendent of <paramref name="entity" />.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// <c>true</c> if this instance is a descendent of <paramref name="entity" />; otherwise, <c>false</c>.
        /// </returns>
        bool IsDescendentOf(IGameEntity entity);

        /// <summary>
        /// Called when this instance is removed from the <see cref="IGameScene" /> tree.
        /// </summary>
        void OnRemovedFromSceneTree();

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void RemoveChild(IGameEntity entity);

        /// <summary>
        /// Removes the component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <returns>A value indicating whether or not the component was removed.</returns>
        bool RemoveComponent(IGameComponent component);

        /// <summary>
        /// Tries the get ancestral component. This component could be a component on this instance
        /// or a parent going all the way up to the scene.
        /// </summary>
        /// <typeparam name="T">The type of component.</typeparam>
        /// <param name="component">The component.</param>
        /// <returns>A value indicating whether or not the component was found.</returns>
        bool TryGetAncestralComponent<T>(out T? component) where T : class, IGameComponent;

        /// <summary>
        /// Tries the get component a component of the specified type that belongs to this instance.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="component">The component.</param>
        /// <returns>
        /// A value indicating whether or not a component of the specified type was found.
        /// </returns>
        bool TryGetComponent<T>(out T? component) where T : class, IGameComponent;
    }

    /// <summary>
    /// A <see cref="ITransformable" /> descendent of <see cref="IGameScene" /> which holds a
    /// collection of <see cref="IGameComponent" />
    /// </summary>
    public class GameEntity : Transformable, IGameEntity {

        /// <summary>
        /// The default empty <see cref="IGameEntity" /> that is present before initialization.
        /// </summary>
        public static readonly IGameEntity Empty = new EmptyGameEntity();

        [DataMember]
        private readonly ObservableCollection<IGameEntity> _children = new ObservableCollection<IGameEntity>();

        [DataMember]
        private readonly ObservableCollection<IGameComponent> _components = new ObservableCollection<IGameComponent>();

        [DataMember]
        private bool _isEnabled = true;

        private Layers _layers = Layers.Default;

        [DataMember]
        private string _name = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameEntity" /> class.
        /// </summary>
        public GameEntity() : base() {
            this.PropertyChanged += this.OnPropertyChanged;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IGameEntity> Children => this._children;

        /// <inheritdoc />
        public IReadOnlyCollection<IGameComponent> Components => this._components;

        /// <inheritdoc />
        public bool IsEnabled {
            get {
                return this._isEnabled;
            }

            set {
                this.Set(ref this._isEnabled, value);
            }
        }

        /// <inheritdoc />
        [DataMember]
        public Layers Layers {
            get {
                return this._layers;
            }

            set {
                this.Set(ref this._layers, value);
            }
        }

        /// <inheritdoc />
        public string Name {
            get {
                return this._name;
            }

            set {
                this.Set(ref this._name, value);
            }
        }

        /// <inheritdoc />
        public IGameEntity Parent { get; private set; } = GameEntity.Empty;

        /// <inheritdoc />
        public IGameScene Scene { get; private set; } = GameScene.Empty;

        /// <inheritdoc />
        public T AddChild<T>() where T : IGameEntity, new() {
            var entity = new T();
            this._children.Add(entity);
            this.OnAddChild(entity);
            return entity;
        }

        /// <inheritdoc />
        public IGameEntity AddChild() {
            return this.AddChild<GameEntity>();
        }

        /// <inheritdoc />
        public void AddChild(IGameEntity entity) {
            if (this.CanAddChild(entity)) {
                entity.Parent?.RemoveChild(entity);
                this._children.Add(entity);
                this.OnAddChild(entity);
            }
        }

        /// <inheritdoc />
        public T AddComponent<T>() where T : IGameComponent, new() {
            var component = new T();
            this.AddComponent(component);
            return component;
        }

        /// <inheritdoc />
        public void AddComponent(IGameComponent component) {
            component.Entity.RemoveComponent(component);
            this._components.Add(component);
            if (!GameScene.IsNullOrEmpty(this.Scene)) {
                this.Scene.Invoke(() => {
                    this.Scene.RegisterComponent(component);
                    component.Initialize(this);
                });
            }
        }

        /// <inheritdoc />
        public IEnumerable<T> GetComponents<T>() {
            return this.Components.OfType<T>();
        }

        /// <inheritdoc />
        public T GetOrAddComponent<T>() where T : class, IGameComponent, new() {
            if (this.TryGetComponent<T>(out var component) && component != null) {
                return component;
            }
            else {
                return this.AddComponent<T>();
            }
        }

        /// <inheritdoc />
        public void Initialize(IGameScene scene, IGameEntity parent) {
            this.Scene = scene;
            this.Parent = parent;

            foreach (var component in this.Components) {
                component.Initialize(this);
                this.Scene.RegisterComponent(component);
            }

            foreach (var child in this.Children) {
                child.Initialize(this.Scene, this);
            }

            this.HandleMatrixOrTransformChanged();
        }

        /// <inheritdoc />
        public bool IsDescendentOf(IGameEntity entity) {
            var result = false;

            if (entity != null) {
                result = entity == this.Parent || (this.Parent?.IsDescendentOf(entity) == true);
            }

            return result;
        }

        /// <inheritdoc />
        public void OnRemovedFromSceneTree() {
            foreach (var component in this._components) {
                this.Scene.UnregisterComponent(component);
            }

            foreach (var child in this._children) {
                child.OnRemovedFromSceneTree();
            }

            this.Scene = GameScene.Empty;
        }

        /// <inheritdoc />
        public void RemoveChild(IGameEntity entity) {
            if (this.Scene != null) {
                this.Scene.Invoke(() => this.PerformChildRemoval(entity));
            }
            else {
                this._children.Remove(entity);
            }
        }

        /// <inheritdoc />
        public bool RemoveComponent(IGameComponent component) {
            var result = false;

            if (this._components.Contains(component)) {
                if (!GameScene.IsNullOrEmpty(this.Scene)) {
                    this.Scene.Invoke(() => {
                        this._components.Remove(component);
                        this.Scene.UnregisterComponent(component);
                    });
                    result = true;
                }
                else {
                    result = this._components.Remove(component);
                }
            }

            return result;
        }

        /// <inheritdoc />
        public bool TryGetAncestralComponent<T>(out T? component) where T : class, IGameComponent {
            return this.TryGetComponent(out component) || this.Parent.TryGetAncestralComponent(out component);
        }

        /// <inheritdoc />
        public bool TryGetComponent<T>(out T? component) where T : class, IGameComponent {
            component = this.Components.OfType<T>().FirstOrDefault();
            return component != null;
        }

        /// <inheritdoc />
        protected override ITransformable GetParentTransformable() {
            return this.Parent;
        }

        /// <summary>
        /// Called when a property on the current object changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        /// The <see cref="PropertyChangedEventArgs" /> instance containing the event data.
        /// </param>
        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.Parent)) {
                this.HandleMatrixOrTransformChanged();
            }
        }

        private bool CanAddChild(IGameEntity entity) {
            return entity != this && !this.Children.Any(x => x == entity) && !this.IsDescendentOf(entity);
        }

        private void OnAddChild(IGameEntity entity) {
            this.Scene.Invoke(() => entity.Initialize(this.Scene, this));
        }

        private void PerformChildRemoval(IGameEntity entity) {
            if (this._children.Contains(entity)) {
                entity.OnRemovedFromSceneTree();
                this._children.Remove(entity);
            }
        }

        internal class EmptyGameEntity : EmptyTransformable, IGameEntity {

            /// <inheritdoc />
            public event PropertyChangedEventHandler? PropertyChanged;

            /// <inheritdoc />
            public bool IsEnabled {
                get => false;
                set {
                    return;
                }
            }

            /// <inheritdoc />
            public Layers Layers {
                get => Layers.None;
                set { return; }
            }

            /// <inheritdoc />
            public string Name {
                get => "Empty";
                set {
                    return;
                }
            }

            /// <inheritdoc />
            public T AddChild<T>() where T : IGameEntity, new() {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public IGameEntity AddChild() {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public void AddChild(IGameEntity entity) {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public T AddComponent<T>() where T : IGameComponent, new() {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public void AddComponent(IGameComponent component) {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public IEnumerable<T> GetComponents<T>() {
                return new T[0];
            }

            /// <inheritdoc />
            public T GetOrAddComponent<T>() where T : class, IGameComponent, new() {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public void Initialize(IGameScene scene, IGameEntity parent) {
                throw new NotSupportedException("An empty entity cannot be intialized.");
            }

            /// <inheritdoc />
            public bool IsDescendentOf(IGameEntity entity) {
                return false;
            }

            /// <inheritdoc />
            public void OnRemovedFromSceneTree() {
                throw new NotSupportedException("An empty entity should never be added to a scene tree, much less removed.");
            }

            /// <inheritdoc />
            public void RemoveChild(IGameEntity entity) {
            }

            /// <inheritdoc />
            public bool RemoveComponent(IGameComponent component) {
                return false;
            }

            /// <inheritdoc />
            public bool TryGetAncestralComponent<T>(out T? component) where T : class, IGameComponent {
                component = default;
                return false;
            }

            /// <inheritdoc />
            public bool TryGetComponent<T>(out T? component) where T : class, IGameComponent {
                component = null;
                return false;
            }
        }
    }
}