namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
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
        IReadOnlyCollection<IGameEntity> Children { get; }

        /// <summary>
        /// Gets the components.
        /// </summary>
        /// <value>The components.</value>
        IReadOnlyCollection<IGameComponent> Components { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the layers.
        /// </summary>
        /// <value>The layers.</value>
        Layers Layers { get; }

        /// <summary>
        /// Gets or sets the local position.
        /// </summary>
        /// <value>The local position.</value>
        Vector2 LocalPosition { get; set; }

        /// <summary>
        /// Gets or sets the local scale.
        /// </summary>
        /// <value>The local scale.</value>
        Vector2 LocalScale { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        IGameEntity Parent { get; }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        IGameScene Scene { get; }

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
        /// Removes the child.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>A value indicating whether or not the child was removed.</returns>
        bool RemoveChild(IGameEntity entity);

        /// <summary>
        /// Removes the component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <returns>A value indicating whether or not the component was removed.</returns>
        bool RemoveComponent(IGameComponent component);
    }

    /// <summary>
    /// A <see cref="ITransformable" /> descendent of <see cref="IScene" /> which holds a collection
    /// of <see cref="IGameComponent" />
    /// </summary>
    public class GameEntity : NotifyPropertyChanged, IGameEntity {

        [DataMember]
        private readonly ObservableCollection<IGameEntity> _children = new ObservableCollection<IGameEntity>();

        [DataMember]
        private readonly ObservableCollection<IGameComponent> _components = new ObservableCollection<IGameComponent>();

        private readonly ResettableLazy<Matrix> _transformMatrix;

        [DataMember]
        private bool _isEnabled;

        private bool _isTransformUpToDate;
        private Layers _layers = Layers.Default;
        private Vector2 _localPosition;
        private Vector2 _localScale = Vector2.One;

        [DataMember]
        private string _name;

        private Transform _transform = new Transform();

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

        /// <summary>
        /// Gets or sets the local position.
        /// </summary>
        /// <value>The local position.</value>
        [DataMember(Name = "Local Position")]
        public Vector2 LocalPosition {
            get {
                return this._localPosition;
            }
            set {
                if (this.Set(ref this._localPosition, value)) {
                    this.HandleMatrixOrTransformChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the local scale.
        /// </summary>
        /// <value>The local scale.</value>
        [DataMember(Name = "Local Scale")]
        public Vector2 LocalScale {
            get {
                return this._localScale;
            }
            set {
                if (this.Set(ref this._localScale, value)) {
                    this.HandleMatrixOrTransformChanged();
                }
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
        public IGameEntity Parent { get; private set; }

        /// <inheritdoc />
        public IGameScene Scene { get; private set; }

        /// <inheritdoc />
        public virtual Transform Transform {
            get {
                if (!this._isTransformUpToDate) {
                    this._transform = this.TransformMatrix.DecomposeWithoutRotation2D();
                    this._isTransformUpToDate = true;
                }

                return this._transform;
            }
        }

        /// <summary>
        /// Gets the transform matrix.
        /// </summary>
        /// <value>The transform matrix.</value>
        public Matrix TransformMatrix {
            get {
                return this._transformMatrix.Value;
            }
        }

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
            if (component != null) {
                component.Entity?.RemoveComponent(component);
                this._components.Add(component);

                if (this.Scene != null) {
                    this.Scene.Invoke(() => component.Initialize(this));
                }
            }
        }

        /// <inheritdoc />
        public void Initialize(IGameScene scene, IGameEntity parent) {
            this.Scene = scene;
            this.Parent = parent;

            foreach (var component in this.Components) {
                component.Initialize(this);
            }

            foreach (var child in this.Children) {
                child.Initialize(this.Scene, this);
            }
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
        public bool RemoveChild(IGameEntity entity) {
            var result = false;
            if (this.Scene != null) {
                if (this._children.Contains(entity)) {
                    this.Scene.Invoke(() => this._children.Remove(entity));
                    result = true;
                }
            }
            else {
                result = this._children.Remove(entity);
            }

            return result;
        }

        /// <inheritdoc />
        public bool RemoveComponent(IGameComponent component) {
            var result = false;
            if (this.Scene != null) {
                if (this._components.Contains(component)) {
                    this.Scene.Invoke(() => this._components.Remove(component));
                    result = true;
                }
            }
            else {
                result = this._components.Remove(component);
            }

            return result;
        }

        private bool CanAddChild(IGameEntity entity) {
            return entity != null && entity != this && !this.Children.Any(x => x == entity) && !this.IsDescendentOf(entity);
        }

        private void HandleMatrixOrTransformChanged() {
            this._transformMatrix.Reset();
            this._isTransformUpToDate = false;
            this.RaisePropertyChanged(true, nameof(this.Transform));
        }

        private void OnAddChild(IGameEntity entity) {
            if (this.Scene != null) {
                this.Scene.Invoke(() => entity.Initialize(this.Scene, this));
            }
        }
    }
}