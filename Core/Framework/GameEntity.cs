namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
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
        /// An object that implements <see cref="IGameComponent" /> and has an empty constructor.
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
    public class GameEntity : PropertyChangedNotifier, IGameEntity {

        [DataMember]
        private readonly ObservableCollection<IGameEntity> _children = new ObservableCollection<IGameEntity>();

        [DataMember]
        private readonly ObservableCollection<IGameComponent> _components = new ObservableCollection<IGameComponent>();

        [DataMember]
        private bool _isEnabled;

        private bool _isInitialized;

        [DataMember]
        private string _name;

        /// <inheritdoc />
        public IReadOnlyCollection<IGameEntity> Children => this._children;

        /// <inheritdoc />
        public IReadOnlyCollection<IGameComponent> Components => this._components;

        /// <inheritdoc />
        public bool IsEnabled {
            get {
                return this._isInitialized && this._isEnabled;
            }

            set {
                this.Set(ref this._isEnabled, value);
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
        public virtual Transform Transform => throw new NotImplementedException();

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

                if (this._isInitialized) {
                    component.Initialize(this);
                }
            }
        }

        /// <inheritdoc />
        public void Initialize(IGameScene scene, IGameEntity parent) {
            if (!this._isInitialized) {
                try {
                    this.Scene = scene;
                    this.Parent = parent;

                    foreach (var component in this.Components) {
                        component.Initialize(this);
                    }

                    foreach (var child in this.Children) {
                        child.Initialize(this.Scene, this);
                    }
                }
                finally {
                    this._isInitialized = true;
                }
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
            return this._children.Remove(entity);
        }

        /// <inheritdoc />
        public bool RemoveComponent(IGameComponent component) {
            return this._components.Remove(component);
        }

        private bool CanAddChild(IGameEntity entity) {
            return entity != null && entity != this && !this.Children.Any(x => x == entity) && !this.IsDescendentOf(entity);
        }

        private void OnAddChild(IGameEntity entity) {
            if (this._isInitialized) {
                entity.Initialize(this.Scene, this);
            }
        }
    }
}