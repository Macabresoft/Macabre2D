namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;
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
        /// Gets the transform matrix.
        /// </summary>
        /// <value>The transform matrix.</value>
        Matrix TransformMatrix { get; }

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
        /// Gets the components of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of component.</typeparam>
        /// <returns>The components of specified type.</returns>
        IEnumerable<T> GetComponents<T>();

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

        /// <summary>
        /// Tries the get component a component of the specified type that belongs to this instance.
        /// </summary>
        /// <typeparam name="T">The type of component to search for.</typeparam>
        /// <param name="component">The component.</param>
        /// <returns>
        /// A value indicating whether or not a component of the specified type was found.
        /// </returns>
        bool TryGetComponent<T>(out T? component) where T : class;
    }

    /// <summary>
    /// A <see cref="ITransformable" /> descendent of <see cref="IScene" /> which holds a collection
    /// of <see cref="IGameComponent" />
    /// </summary>
    public class GameEntity : NotifyPropertyChanged, IGameEntity {

        /// <summary>
        /// The default empty <see cref="IGameEntity" /> that is present before initialization.
        /// </summary>
        public static readonly IGameEntity Empty = new EmptyGameEntity();

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
        private string _name = string.Empty;

        private Transform _transform = new Transform();

        public GameEntity() {
            this.PropertyChanged += this.OnPropertyChanged;
            this._transformMatrix = new ResettableLazy<Matrix>(this.GetMatrix);
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
        public IGameEntity Parent { get; private set; } = GameEntity.Empty;

        /// <inheritdoc />
        public IGameScene Scene { get; private set; } = GameScene.Empty;

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

        /// <inheritdoc />
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
        public IEnumerable<T> GetComponents<T>() {
            return this.Components.OfType<T>();
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(float rotationAngle) {
            var transform = this.Transform;
            var matrix =
                Matrix.CreateScale(transform.Scale.X, transform.Scale.Y, 1f) *
                Matrix.CreateRotationZ(rotationAngle) *
                Matrix.CreateTranslation(transform.Position.X, transform.Position.Y, 0f);

            return matrix.ToTransform();
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(Vector2 originOffset) {
            var matrix = Matrix.CreateTranslation(originOffset.X, originOffset.Y, 0f) * this.TransformMatrix;
            return matrix.ToTransform();
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(Vector2 originOffset, float rotationAngle) {
            var transform = this.Transform;

            var matrix =
                Matrix.CreateTranslation(originOffset.X, originOffset.Y, 0f) *
                Matrix.CreateScale(transform.Scale.X, transform.Scale.Y, 1f) *
                Matrix.CreateRotationZ(rotationAngle) *
                Matrix.CreateTranslation(transform.Position.X, transform.Position.Y, 0f);

            return matrix.ToTransform();
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale, float rotationAngle) {
            var transform = this.Transform;

            var matrix =
                Matrix.CreateScale(overrideScale.X, overrideScale.Y, 1f) *
                Matrix.CreateTranslation(originOffset.X, originOffset.Y, 0f) *
                Matrix.CreateRotationZ(rotationAngle) *
                Matrix.CreateTranslation(transform.Position.X, transform.Position.Y, 0f);

            return matrix.ToTransform();
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale) {
            var transform = this.Transform;

            var matrix =
                Matrix.CreateScale(overrideScale.X, overrideScale.Y, 1f) *
                Matrix.CreateTranslation(originOffset.X, originOffset.Y, 0f) *
                Matrix.CreateTranslation(transform.Position.X, transform.Position.Y, 0f);

            return matrix.ToTransform();
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation) {
            var position = new Vector2(gridTileLocation.X * grid.TileSize.X, gridTileLocation.Y * grid.TileSize.Y) + grid.Offset;
            return this.GetWorldTransform(position);
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset) {
            var position = new Vector2(gridTileLocation.X * grid.TileSize.X, gridTileLocation.Y * grid.TileSize.Y) + grid.Offset + offset;
            return this.GetWorldTransform(position);
        }

        /// <inheritdoc />
        public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset, float rotationAngle) {
            var position = new Vector2(gridTileLocation.X * grid.TileSize.X, gridTileLocation.Y * grid.TileSize.Y) + grid.Offset + offset;
            return this.GetWorldTransform(position, rotationAngle);
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

        /// <inheritdoc />
        public void SetWorldPosition(Vector2 position) {
            this.SetWorldTransform(position, this.Transform.Scale);
        }

        /// <inheritdoc />
        public void SetWorldScale(Vector2 scale) {
            this.SetWorldTransform(this.Transform.Position, scale);
        }

        /// <inheritdoc />
        public void SetWorldTransform(Vector2 position, Vector2 scale) {
            var matrix =
                Matrix.CreateScale(scale.X, scale.Y, 1f) *
                Matrix.CreateTranslation(position.X, position.Y, 0f);

            if (this.Parent != null) {
                matrix *= Matrix.Invert(this.Parent.TransformMatrix);
            }

            var localTransform = matrix.ToTransform();
            this._localPosition = localTransform.Position;
            this._localScale = localTransform.Scale;
            this.HandleMatrixOrTransformChanged();
            this.RaisePropertyChanged(nameof(this.LocalPosition));
            this.RaisePropertyChanged(nameof(this.LocalScale));
        }

        /// <inheritdoc />
        public bool TryGetComponent<T>(out T? component) where T : class {
            component = this.Components.OfType<T>().FirstOrDefault();
            return component != null;
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
            return entity != null && entity != this && !this.Children.Any(x => x == entity) && !this.IsDescendentOf(entity);
        }

        private Matrix GetMatrix() {
            var transformMatrix =
                Matrix.CreateScale(this.LocalScale.X, this.LocalScale.Y, 1f) *
                Matrix.CreateTranslation(this.LocalPosition.X, this.LocalPosition.Y, 0f);

            if (this.Parent != null) {
                transformMatrix *= this.Parent.TransformMatrix;
            }

            return transformMatrix;
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

        internal class EmptyGameEntity : IGameEntity {

            /// <inheritdoc />
            public event PropertyChangedEventHandler? PropertyChanged;

            /// <inheritdoc />
            public IReadOnlyCollection<IGameEntity> Children { get; } = new IGameEntity[0];

            /// <inheritdoc />
            public IReadOnlyCollection<IGameComponent> Components { get; } = new IGameComponent[0];

            /// <inheritdoc />
            public bool IsEnabled {
                get => false;
                set {
                    return;
                }
            }

            /// <inheritdoc />
            public Layers Layers => Layers.None;

            /// <inheritdoc />
            public Vector2 LocalPosition {
                get => Vector2.Zero;
                set {
                    return;
                }
            }

            /// <inheritdoc />
            public Vector2 LocalScale {
                get => Vector2.One;
                set {
                    return;
                }
            }

            /// <inheritdoc />
            public string Name {
                get => "Empty";
                set {
                    return;
                }
            }

            /// <inheritdoc />
            public IGameEntity Parent => GameScene.Empty;

            /// <inheritdoc />
            public IGameScene Scene => GameScene.Empty;

            /// <inheritdoc />
            public Transform Transform => Transform.Origin;

            /// <inheritdoc />
            public Matrix TransformMatrix => Matrix.Identity;

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
            public Transform GetWorldTransform(float rotationAngle) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(Vector2 originOffset) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(Vector2 originOffset, float rotationAngle) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale, float rotationAngle) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset) {
                return this.Transform;
            }

            /// <inheritdoc />
            public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset, float rotationAngle) {
                return this.Transform;
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
            public bool RemoveChild(IGameEntity entity) {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public bool RemoveComponent(IGameComponent component) {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public void SetWorldPosition(Vector2 position) {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public void SetWorldScale(Vector2 scale) {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public void SetWorldTransform(Vector2 position, Vector2 scale) {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public bool TryGetComponent<T>(out T? component) where T : class {
                component = null;
                return false;
            }
        }
    }
}