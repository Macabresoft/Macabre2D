namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for a <see cref="ITransformable" /> descendent of <see cref="IScene" /> which
    /// holds a collection of <see cref="IEntity" />.
    /// </summary>
    public interface IEntity : IEnableable, IIdentifiable, ITransformable, INotifyPropertyChanged {
        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        IReadOnlyCollection<IEntity> Children => Array.Empty<IEntity>();

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        IEntity Parent => Framework.Scene.Empty;

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        IScene Scene => Framework.Scene.Empty;

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
        /// Adds a child of the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// An object that implements <see cref="IEntity" /> and has an empty constructor.
        /// </typeparam>
        /// <returns>The added child.</returns>
        T AddChild<T>() where T : IEntity, new();

        /// <summary>
        /// Adds a child of this entity's default child type.
        /// </summary>
        /// <returns>The added child.</returns>
        IEntity AddChild();

        /// <summary>
        /// Adds an existing <see cref="IEntity" /> as a child.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void AddChild(IEntity entity);

        /// <summary>
        /// Gets the child of the specified type if it exists; otherwise, adds a new child.
        /// </summary>
        /// <typeparam name="T">The type of child to find.</typeparam>
        /// <returns>The entity that was found or added.</returns>
        T GetOrAddChild<T>() where T : class, IEntity, new();

        /// <summary>
        /// Initializes this entity as a descendent of <paramref name="scene" /> and
        /// <paramref name="parent" />.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <param name="parent">The parent.</param>
        void Initialize(IScene scene, IEntity parent);

        /// <summary>
        /// Determines whether this instance is a descendent of <paramref name="entity" />.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// <c>true</c> if this instance is a descendent of <paramref name="entity" />; otherwise, <c>false</c>.
        /// </returns>
        bool IsDescendentOf(IEntity entity);

        /// <summary>
        /// Called when this instance is removed from the <see cref="IScene" /> tree.
        /// </summary>
        void OnRemovedFromSceneTree();

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void RemoveChild(IEntity entity);

        /// <summary>
        /// Tries the get a child of the specific type. This is recursive.
        /// </summary>
        /// <typeparam name="T">The type of entity for which to search.</typeparam>
        /// <param name="entity">The child entity.</param>
        /// <returns>
        /// A value indicating whether or not a child of the specified type was found.
        /// </returns>
        bool TryGetChild<T>(out T? entity) where T : class, IEntity;

        /// <summary>
        /// Tries the get the parent. This entity could be a parent going all the way up to the scene.
        /// </summary>
        /// <typeparam name="T">The type of parent entity.</typeparam>
        /// <param name="entity">The parent entity.</param>
        /// <returns>A value indicating whether or not the entity was found.</returns>
        bool TryGetParentEntity<T>(out T? entity) where T : class, IEntity;
    }

    /// <summary>
    /// A <see cref="ITransformable" /> descendent of <see cref="IScene" /> which holds a
    /// collection of <see cref="IEntity" />
    /// </summary>
    [Category("Entity")]
    public class Entity : Transformable, IEntity {
        /// <summary>
        /// The default empty <see cref="IEntity" /> that is present before initialization.
        /// </summary>
        public static readonly IEntity Empty = new EmptyEntity();

        [DataMember]
        private readonly ObservableCollection<IEntity> _children = new();

        private Guid _id = Guid.NewGuid();
        private bool _isEnabled = true;
        private Layers _layers = Layers.Default;
        private string _name = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity" /> class.
        /// </summary>
        public Entity() : base() {
            this.PropertyChanged += this.OnPropertyChanged;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IEntity> Children => this._children;

        /// <inheritdoc />
        [DataMember]
        public Guid Id {
            get => this._id;
            set => this.Set(ref this._id, value);
        }

        /// <inheritdoc />
        [DataMember]
        public bool IsEnabled {
            get => this._isEnabled;
            set => this.Set(ref this._isEnabled, value);
        }

        /// <inheritdoc />
        [DataMember]
        public Layers Layers {
            get => this._layers;
            set => this.Set(ref this._layers, value);
        }

        /// <inheritdoc />
        [DataMember]
        public string Name {
            get => this._name;
            set => this.Set(ref this._name, value);
        }

        /// <inheritdoc />
        public IEntity Parent { get; private set; } = Empty;

        /// <inheritdoc />
        public IScene Scene { get; private set; } = Framework.Scene.Empty;

        /// <inheritdoc />
        public T AddChild<T>() where T : IEntity, new() {
            var entity = new T {
                Name = typeof(T).Name
            };
            
            this._children.Add(entity);
            this.OnAddChild(entity);
            return entity;
        }

        /// <inheritdoc />
        public IEntity AddChild() {
            return this.AddChild<Entity>();
        }

        /// <inheritdoc />
        public void AddChild(IEntity entity) {
            if (this.CanAddChild(entity)) {
                entity.Parent.RemoveChild(entity);
                this._children.Add(entity);
                this.OnAddChild(entity);
            }
        }

        /// <inheritdoc />
        public T GetOrAddChild<T>() where T : class, IEntity, new() {
            if (this.TryGetChild<T>(out var entity) && entity != null) {
                return entity;
            }

            return this.AddChild<T>();
        }

        /// <inheritdoc />
        public virtual void Initialize(IScene scene, IEntity parent) {
            this.Parent.PropertyChanged -= this.Parent_PropertyChanged;
            this.Scene = scene;
            this.Parent = parent;
            this.Scene.RegisterEntity(this);

            foreach (var child in this.Children) {
                child.Initialize(this.Scene, this);
            }

            this.HandleMatrixOrTransformChanged();
            this.Parent.PropertyChanged += this.Parent_PropertyChanged;
        }

        /// <inheritdoc />
        public bool IsDescendentOf(IEntity entity) {
            return entity == this.Parent || this.Parent != this.Parent.Parent && this.Parent.IsDescendentOf(entity);
        }

        /// <inheritdoc />
        public virtual void OnRemovedFromSceneTree() {
            this.Scene.UnregisterEntity(this);
            this.Scene = Framework.Scene.Empty;
            this.Parent = Empty;

            foreach (var child in this._children) {
                child.OnRemovedFromSceneTree();
            }
        }

        /// <inheritdoc />
        public void RemoveChild(IEntity entity) {
            if (!Framework.Scene.IsNullOrEmpty(this.Scene)) {
                this.Scene.Invoke(() => this.PerformChildRemoval(entity));
            }
            else {
                this._children.Remove(entity);
            }
        }

        /// <inheritdoc />
        public bool TryGetChild<T>(out T? entity) where T : class, IEntity {
            entity = this.Children.OfType<T>().FirstOrDefault();
            return entity != null;
        }

        /// <inheritdoc />
        public virtual bool TryGetParentEntity<T>(out T? entity) where T : class, IEntity {
            if (this.Parent is T parent) {
                entity = parent;
            }
            else {
                this.Parent.TryGetParentEntity(out entity);
            }

            return entity != null;
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
        protected virtual void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.Parent)) {
                this.HandleMatrixOrTransformChanged();
            }
        }

        private bool CanAddChild(IEntity entity) {
            return entity != this && this.Children.All(x => x != entity) && !this.IsDescendentOf(entity);
        }

        private void OnAddChild(IEntity child) {
            if (!Framework.Scene.IsNullOrEmpty(this.Scene)) {
                this.Scene.Invoke(() => child.Initialize(this.Scene, this));
            }
        }

        private void Parent_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ITransformable.Transform)) {
                this.HandleMatrixOrTransformChanged();
            }
        }

        private void PerformChildRemoval(IEntity entity) {
            if (this._children.Contains(entity)) {
                entity.OnRemovedFromSceneTree();
                this._children.Remove(entity);
            }
        }

        internal class EmptyEntity : EmptyTransformable, IEntity {
            /// <inheritdoc />
            public event PropertyChangedEventHandler? PropertyChanged;

            /// <inheritdoc />
            public Guid Id {
                get => Guid.Empty;
                set { }
            }

            /// <inheritdoc />
            public bool IsEnabled {
                get => false;
                set { }
            }

            /// <inheritdoc />
            public Layers Layers {
                get => Layers.None;
                set { }
            }

            /// <inheritdoc />
            public string Name {
                get => "Empty";
                set { }
            }

            /// <inheritdoc />
            public T AddChild<T>() where T : IEntity, new() {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public IEntity AddChild() {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public void AddChild(IEntity entity) {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public T GetOrAddChild<T>() where T : class, IEntity, new() {
                throw new NotSupportedException("Initialization has not occured.");
            }

            /// <inheritdoc />
            public void Initialize(IScene scene, IEntity parent) {
                throw new NotSupportedException("An empty entity cannot be initialized.");
            }

            /// <inheritdoc />
            public bool IsDescendentOf(IEntity entity) {
                return false;
            }

            /// <inheritdoc />
            public void OnRemovedFromSceneTree() {
                throw new NotSupportedException("An empty entity should never be added to a scene tree, much less removed.");
            }

            /// <inheritdoc />
            public void RemoveChild(IEntity entity) {
            }

            /// <inheritdoc />
            public bool TryGetChild<T>(out T? entity) where T : class, IEntity {
                entity = null;
                return false;
            }

            /// <inheritdoc />
            public bool TryGetParentEntity<T>(out T? entity) where T : class, IEntity {
                entity = default;
                return false;
            }
        }
    }
}