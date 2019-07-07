namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;

    /// <summary>
    /// The base class for all components.
    /// </summary>
    [DataContract]
    public abstract class BaseComponent : IBaseComponent, IDisposable {
        protected bool _disposedValue;

        [DataMember]
        private readonly ObservableCollection<BaseComponent> _children = new ObservableCollection<BaseComponent>();

        private readonly List<Func<BaseComponent, bool>> _resolveChildActions = new List<Func<BaseComponent, bool>>();

        private readonly Transform _transform = new Transform();

        private readonly ResettableLazy<Matrix> _transformMatrix;

        [DataMember]
        private int _drawOrder;

        [DataMember]
        private bool _isEnabled = true;

        private bool _isTransformUpToDate;

        [DataMember]
        private bool _isVisible = true;

        private Vector2 _localPosition;

        private Vector2 _localScale = Vector2.One;

        [DataMember]
        private BaseComponent _parent;

        [DataMember]
        private int _updateOrder;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseComponent"/> class.
        /// </summary>
        public BaseComponent() {
            this.IsEnabledChanged += this.Self_EnabledChanged;
            this._children.CollectionChanged += this.Children_CollectionChanged;
            this._transformMatrix = new ResettableLazy<Matrix>(this.GetMatrix);
        }

        /// <inheritdoc/>
        public event EventHandler DrawOrderChanged;

        /// <inheritdoc/>
        public event EventHandler IsEnabledChanged;

        /// <inheritdoc/>
        public event EventHandler IsVisibleChanged;

        /// <inheritdoc/>
        public event EventHandler<BaseComponent> ParentChanged;

        /// <summary>
        /// Occurs when this object's transform changes.
        /// </summary>
        public event EventHandler TransformChanged;

        /// <summary>
        /// Occurs when [update order changed].
        /// </summary>
        public event EventHandler UpdateOrderChanged;

        /// <summary>
        /// Gets the backward vector. This is the same as (-1, 0) when no rotation is applied.
        /// </summary>
        /// <value>The backward vector.</value>
        public Vector2 Backward {
            get {
                return this.TransformMatrix.Backward.ToVector2();
            }
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<BaseComponent> Children {
            get {
                return this._children;
            }
        }

        /// <summary>
        /// Gets the down vector. This is equal to (0, -1) when no rotation is applied.
        /// </summary>
        /// <value>The down vector.</value>
        public Vector2 Down {
            get {
                return this.TransformMatrix.Down.ToVector2();
            }
        }

        /// <inheritdoc/>
        public int DrawOrder {
            get {
                return this._drawOrder;
            }

            set {
                if (value != this._drawOrder) {
                    this._drawOrder = value;
                    this.DrawOrderChanged.SafeInvoke(this);
                }
            }
        }

        /// <inheritdoc/>
        public EngineObjectType EngineObjectType {
            get {
                return EngineObjectType.Component;
            }
        }

        /// <summary>
        /// Gets the forward vector. This the same as (1, 0) when no rotation is applied.
        /// </summary>
        /// <value>The forward vector.</value>
        public Vector2 Forward {
            get {
                return this.TransformMatrix.Forward.ToVector2();
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid Id { get; internal set; } = Guid.NewGuid();

        /// <inheritdoc/>
        public bool IsEnabled {
            get {
                return this._isEnabled && (this.Parent == null || this.Parent.IsEnabled);
            }
            set {
                if (this._isEnabled != value) {
                    this._isEnabled = value;

                    if (this.Parent == null || this.Parent.IsEnabled) {
                        this.IsEnabledChanged.SafeInvoke(this);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this instance has been initialized.
        /// </summary>
        /// <value>A value indicating whether or not this instance has been initialized.</value>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public bool IsVisible {
            get {
                return this.IsEnabled && this._isVisible;
            }
            set {
                if (value != this._isVisible) {
                    this._isVisible = value;

                    if (this.IsEnabled) {
                        this.IsVisibleChanged.SafeInvoke(this);
                    }
                }
            }
        }

        /// <inheritdoc/>
        [DataMember]
        public Layers Layers { get; set; } = Layers.Layer01;

        /// <summary>
        /// Gets or sets the local position.
        /// </summary>
        /// <value>The local position.</value>
        [DataMember]
        public Vector2 LocalPosition {
            get {
                return this._localPosition;
            }
            set {
                if (value != this._localPosition) {
                    this._localPosition = value;
                    this.HandleMatrixOrTransformChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the local scale.
        /// </summary>
        /// <value>The local scale.</value>
        [DataMember]
        public Vector2 LocalScale {
            get {
                return this._localScale;
            }
            set {
                if (value != this._localScale) {
                    this._localScale = value;
                    this.HandleMatrixOrTransformChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <inheritdoc/>
        public BaseComponent Parent {
            get {
                return this._parent;
            }

            set {
                var valueId = value?.Id;
                if (this._parent?.Id != valueId && this.Id != valueId) {
                    var originalParent = this._parent;
                    this._parent = value;

                    if (this._parent != null) {
                        this._parent.AddChild(this);

                        if (this.IsInitialized) {
                            this._parent.IsEnabledChanged += this.Parent_EnabledChanged;
                            this._parent.TransformChanged += this.Parent_TransformChanged;
                        }

                        if (originalParent != null) {
                            originalParent.IsEnabledChanged -= this.Parent_EnabledChanged;
                            originalParent.TransformChanged -= this.Parent_TransformChanged;
                        }
                    }
                    else if (originalParent != null) {
                        originalParent.RemoveChild(this);
                        originalParent.IsEnabledChanged -= this.Parent_EnabledChanged;
                        originalParent.TransformChanged -= this.Parent_TransformChanged;
                    }

                    this.ParentChanged.SafeInvoke(this, this._parent);
                }
            }
        }

        /// <summary>
        /// Gets the session identifier. This identifier is unique per session. It is not guaranteed
        /// to be different across sessions, but it will remain the same for the whole time a scene
        /// is running.
        /// </summary>
        /// <value>The session identifier.</value>
        public int SessionId { get; internal set; }

        /// <summary>
        /// Gets the transform matrix.
        /// </summary>
        /// <value>The transform matrix.</value>
        public Matrix TransformMatrix {
            get {
                return this._transformMatrix.Value;
            }
        }

        /// <summary>
        /// Gets the up vector. This is equal to (0, 1) when no rotation is applied.
        /// </summary>
        /// <value>The up vector.</value>
        public Vector2 Up {
            get {
                return this.TransformMatrix.Up.ToVector2();
            }
        }

        /// <inheritdoc/>
        public int UpdateOrder {
            get {
                return this._updateOrder;
            }

            set {
                if (value != this._updateOrder) {
                    this._updateOrder = value;
                    this.UpdateOrderChanged.SafeInvoke(this);
                }
            }
        }

        /// <inheritdoc/>
        public Transform WorldTransform {
            get {
                if (!this._isTransformUpToDate) {
                    this._transform.UpdateTransform(this.TransformMatrix);
                    this._isTransformUpToDate = true;
                }

                return this._transform;
            }
        }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        protected IScene Scene { get; private set; } = EmptyScene.Instance;

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="child">Child.</param>
        public bool AddChild(BaseComponent child) {
            var result = false;
            if (child != null && child.Id != this.Id && !this._children.Any(x => x.Id == child.Id) && !this.IsDescendentOf(child)) {
                this._children.Add(child);
                child.Parent = this;
                result = true;

                if (this.IsInitialized) {
                    child.Initialize(this.Scene);
                }
            }

            return result;
        }

        /// <summary>
        /// Adds a new child component to this component. The new component will not be completely
        /// added until next update.
        /// </summary>
        /// <typeparam name="T">A class of type <see cref="BaseComponent"/>.</typeparam>
        /// <returns>The added component.</returns>
        public T AddChild<T>() where T : BaseComponent, new() {
            var component = new T { IsEnabled = true };
            this.AddChild(component);
            return component;
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">
        /// Exception gets hit if you don't pass in a type that inherits from <see cref="BaseComponent"/>.
        /// </exception>
        public BaseComponent AddChild(Type type) {
            if (typeof(BaseComponent).IsAssignableFrom(type)) {
                var component = Activator.CreateInstance(type) as BaseComponent;
                component.IsEnabled = true;
                this.AddChild(component);
                return component;
            }
            else {
                throw new NotSupportedException($"{type.FullName} is does not inherit from {typeof(BaseComponent).FullName}");
            }
        }

        /// <inheritdoc/>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finds the component with the specified name in this component's children.
        /// </summary>
        /// <returns>The component in children.</returns>
        /// <param name="name">Name.</param>
        public BaseComponent FindComponentInChildren(string name) {
            foreach (var child in this.Children) {
                if (string.Equals(name, child.Name)) {
                    return child;
                }

                var found = child.FindComponentInChildren(name);

                if (found != null) {
                    return found;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all children, including children of children and so on.
        /// </summary>
        /// <returns>All child components.</returns>
        public IEnumerable<BaseComponent> GetAllChildren() {
            var children = new List<BaseComponent>();

            foreach (var component in this.Children) {
                children.Add(component);
                children.AddRange(component.GetAllChildren());
            }

            return children;
        }

        /// <summary>
        /// Gets the child that is the specified type.
        /// </summary>
        /// <returns>The component.</returns>
        /// <typeparam name="T">A class of type <see cref="BaseComponent"/>.</typeparam>
        public T GetChild<T>() where T : BaseComponent {
            return (T)this.Children.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        /// <summary>
        /// Gets a child of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A <see cref="BaseComponent"/> of the specified type in children, or null if it doesn't exist.
        /// </returns>
        public BaseComponent GetChild(Type type) {
            return this.Children.FirstOrDefault(x => x.GetType() == type || type.IsAssignableFrom(x.GetType()));
        }

        /// <summary>
        /// Gets a child of the specified type and with the specified name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A <see cref="BaseComponent"/> of the specified type in children, or null if it doesn't exist.
        /// </returns>
        public BaseComponent GetChild(Type type, string name) {
            return this.Children.FirstOrDefault(x => x.Name == name && (x.GetType() == type || type.IsAssignableFrom(x.GetType())));
        }

        /// <summary>
        /// Gets a child of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <returns>A child of the specified type.</returns>
        public T GetChildOfType<T>() {
            return this._children.OfType<T>().First();
        }

        /// <summary>
        /// Gets the children that are the specified type.
        /// </summary>
        /// <typeparam name="T">A type that a component could be.</typeparam>
        /// <returns>All components that match the type.</returns>
        public IEnumerable<T> GetChildren<T>() {
            return this.Children.OfType<T>();
        }

        /// <summary>
        /// Gets the children of a specified type.
        /// </summary>
        /// <typeparam name="T">The type of the component.</typeparam>
        /// <returns>All children that match the type.</returns>
        public IEnumerable<T> GetChildrenOfType<T>() {
            return this._children.OfType<T>();
        }

        /// <summary>
        /// Gets the component as this component's parent, or that component's parent, or that
        /// component's parent, and so on.
        /// </summary>
        /// <typeparam name="T">A class of type <see cref="BaseComponent"/>.</typeparam>
        /// <returns>A component of the specified type in this component's ancestors.</returns>
        public T GetComponentFromParent<T>() where T : BaseComponent {
            if (this.Parent != null) {
                if (this.Parent is T generic) {
                    return generic;
                }
                else {
                    return this.Parent.GetComponentFromParent<T>();
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a component of the specified type in this component's children.
        /// </summary>
        /// <typeparam name="T">The type of component.</typeparam>
        /// <param name="isShallowSearch">if set to <c>true</c> will only search one level deep.</param>
        /// <returns>The found component.</returns>
        public T GetComponentInChildren<T>(bool isShallowSearch) where T : BaseComponent {
            var component = (T)this._children.FirstOrDefault(x => x.GetType() == typeof(T));

            if (!isShallowSearch && component == null) {
                foreach (var child in this.Children) {
                    component = child.GetComponentInChildren<T>(isShallowSearch);

                    if (component != null) {
                        break;
                    }
                }
            }

            return component;
        }

        /// <summary>
        /// Gets the components of the specified type in this object and in this object's children.
        /// </summary>
        /// <typeparam name="T">A component.</typeparam>
        /// <returns>All components of the specified type in this object and this object's children.</returns>
        public List<T> GetComponentsInChildren<T>() {
            var components = new List<T>();
            components.AddRange(this._children.OfType<T>());

            foreach (var child in this.Children) {
                components.AddRange(child.GetComponentsInChildren<T>());
            }

            return components;
        }

        /// <inheritdoc/>
        public RotatableTransform GetWorldTransform(float rotationAngle) {
            var worldTransform = this.WorldTransform;
            var matrix =
                Matrix.CreateScale(worldTransform.Scale.X, worldTransform.Scale.Y, 1f) *
                Matrix.CreateRotationZ(rotationAngle) *
                Matrix.CreateTranslation(worldTransform.Position.X, worldTransform.Position.Y, 0f);

            return matrix.ToRotatableTransform();
        }

        /// <inheritdoc/>
        public Transform GetWorldTransform(Vector2 originOffset) {
            var matrix = Matrix.CreateTranslation(originOffset.X, originOffset.Y, 0f) * this.TransformMatrix;
            return matrix.ToTransform();
        }

        /// <inheritdoc/>
        public RotatableTransform GetWorldTransform(Vector2 originOffset, float rotationAngle) {
            var worldTransform = this.WorldTransform;

            var matrix =
                Matrix.CreateTranslation(originOffset.X, originOffset.Y, 0f) *
                Matrix.CreateScale(worldTransform.Scale.X, worldTransform.Scale.Y, 1f) *
                Matrix.CreateRotationZ(rotationAngle) *
                Matrix.CreateTranslation(worldTransform.Position.X, worldTransform.Position.Y, 0f);

            return matrix.ToRotatableTransform();
        }

        /// <inheritdoc/>
        public RotatableTransform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale, float rotationAngle) {
            var worldTransform = this.WorldTransform;

            var matrix =
                Matrix.CreateScale(overrideScale.X, overrideScale.Y, 1f) *
                Matrix.CreateTranslation(originOffset.X, originOffset.Y, 0f) *
                Matrix.CreateRotationZ(rotationAngle) *
                Matrix.CreateTranslation(worldTransform.Position.X, worldTransform.Position.Y, 0f);

            return matrix.ToRotatableTransform();
        }

        /// <inheritdoc/>
        public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation) {
            var position = new Vector2(gridTileLocation.X * grid.TileSize.X, gridTileLocation.Y * grid.TileSize.Y) + grid.Offset;
            return this.GetWorldTransform(position);
        }

        /// <inheritdoc/>
        public Transform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset) {
            var position = new Vector2(gridTileLocation.X * grid.TileSize.X, gridTileLocation.Y * grid.TileSize.Y) + grid.Offset + offset;
            return this.GetWorldTransform(position);
        }

        /// <inheritdoc/>
        public RotatableTransform GetWorldTransform(TileGrid grid, Point gridTileLocation, Vector2 offset, float rotationAngle) {
            var position = new Vector2(gridTileLocation.X * grid.TileSize.X, gridTileLocation.Y * grid.TileSize.Y) + grid.Offset + offset;
            return this.GetWorldTransform(position, rotationAngle);
        }

        /// <summary>
        /// Determines whether this instance is an ancestor of the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <returns>
        /// <c>true</c> if this instance is an ancestor of the specified component; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAncestorOf(BaseComponent component) {
            var result = false;
            if (component != null) {
                foreach (var child in this.Children) {
                    if (result) {
                        break;
                    }

                    result = child.Id == component.Id || child.IsAncestorOf(component);
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether this instance is a descendent of the specified component.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <returns>
        /// <c>true</c> if this instance is a descendent of the specified component; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDescendentOf(BaseComponent component) {
            var result = false;

            if (component != null) {
                result = component.Id == this.Parent?.Id || (this.Parent?.IsDescendentOf(component) == true);
            }

            return result;
        }

        /// <inheritdoc/>
        public virtual void LoadContent() {
            foreach (var child in this._children) {
                child.LoadContent();
            }
        }

        /// <summary>
        /// Removes the child.
        /// </summary>
        /// <param name="child">Child.</param>
        public bool RemoveChild(BaseComponent child) {
            var result = false;
            if (child != null && this._children.Remove(child)) {
                child.Parent = null;
                result = true;
            }

            return result;
        }

        /// <inheritdoc/>
        public void SetWorldPosition(Vector2 position) {
            this.SetWorldTransform(position, this.WorldTransform.Scale);
        }

        /// <inheritdoc/>
        public void SetWorldScale(Vector2 scale) {
            this.SetWorldTransform(this.WorldTransform.Position, scale);
        }

        /// <inheritdoc/>
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
        }

        /// <summary>
        /// Subscribes to the children changed event.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void SubscribeToChildrenChanged(NotifyCollectionChangedEventHandler handler) {
            this._children.CollectionChanged += handler;
        }

        /// <summary>
        /// Unsubscribes from the children changed event.
        /// </summary>
        /// <param name="handler">The handler.</param>
        public void UnsubscribeFromChildrenChanged(NotifyCollectionChangedEventHandler handler) {
            this._children.CollectionChanged -= handler;
        }

        /// <summary>
        /// Initialize this instance.
        /// </summary>
        internal void Initialize(IScene scene) {
            if (!this.IsInitialized || this.Scene == null || this.Scene != scene) {
                this.Scene = scene;
                this.ResolveChildren();

                try {
                    this.Initialize();
                }
                finally {
                    this.IsInitialized = true;
                }

                if (this._parent != null) {
                    this._parent.IsEnabledChanged += this.Parent_EnabledChanged;
                    this._parent.TransformChanged += this.Parent_TransformChanged;
                }

                foreach (var child in this._children) {
                    child.Initialize(scene);
                }
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        /// unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing) {
            if (!this._disposedValue) {
                if (disposing) {
                }

                foreach (var child in this._children) {
                    child.Dispose();
                }

                if (this._parent != null) {
                    this._parent.IsEnabledChanged -= this.Parent_EnabledChanged;
                    this._parent.TransformChanged -= this.Parent_TransformChanged;
                }

                this.IsEnabledChanged = null;
                this.ParentChanged = null;
                this.TransformChanged = null;
                this._children.Clear();
                this._parent = null;
                this._resolveChildActions.Clear();
                this._children.CollectionChanged -= this.Children_CollectionChanged;
                this._disposedValue = true;
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected abstract void Initialize();

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (this.IsInitialized && this._resolveChildActions.Any() && e.Action == NotifyCollectionChangedAction.Add && e.NewItems.OfType<BaseComponent>().FirstOrDefault() is BaseComponent newComponent) {
                var actions = this._resolveChildActions.ToList();
                foreach (var action in actions) {
                    if (action.Invoke(newComponent)) {
                        this._resolveChildActions.Remove(action);
                    }
                }
            }
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
            this.TransformChanged.SafeInvoke(this);
            this._isTransformUpToDate = false;
        }

        private void Parent_EnabledChanged(object sender, EventArgs e) {
            if (this._isEnabled) {
                this.IsEnabledChanged.SafeInvoke(this);
            }
        }

        private void Parent_TransformChanged(object sender, EventArgs e) {
            this.HandleMatrixOrTransformChanged();
        }

        private void ResolveChildren() {
            var type = this.GetType();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            var properties = type.GetProperties(flags);

            foreach (var property in properties) {
                if (property.CanWrite && property.PropertyType.IsSubclassOf(typeof(BaseComponent)) && property.GetSetMethod() != null) {
                    var attributes = property.GetCustomAttributes(typeof(ChildAttribute), true);

                    if (attributes.FirstOrDefault() is ChildAttribute childAttribute) {
                        BaseComponent component;

                        if (childAttribute.UseExisting) {
                            if (childAttribute.Name == null) {
                                component = this.GetChild(property.PropertyType);
                            }
                            else {
                                component = this.GetChild(property.PropertyType, childAttribute.Name);
                            }
                        }
                        else {
                            component = this.AddChild(property.PropertyType);
                        }

                        if (component != null) {
                            property.SetValue(this, component);
                        }
                        else {
                            this._resolveChildActions.Add(new Func<BaseComponent, bool>(newComponent => {
                                var result = false;
                                if (property.PropertyType.IsAssignableFrom(newComponent.GetType())) {
                                    property.SetValue(this, newComponent);
                                    result = true;
                                }

                                return result;
                            }));
                        }
                    }
                }
            }

            var fields = type.GetFields(flags);
            foreach (var field in fields) {
                if (field.FieldType.IsSubclassOf(typeof(BaseComponent))) {
                    var attributes = field.GetCustomAttributes(typeof(ChildAttribute), true);
                    if (attributes.FirstOrDefault() is ChildAttribute childAttribute) {
                        BaseComponent component;
                        if (childAttribute.Name == null) {
                            component = this.GetChild(field.FieldType);
                        }
                        else {
                            component = this.GetChild(field.FieldType, childAttribute.Name);
                        }

                        if (component != null) {
                            field.SetValue(this, component);
                        }
                        else {
                            this._resolveChildActions.Add(new Func<BaseComponent, bool>(newComponent => {
                                var result = false;
                                if (field.FieldType.IsAssignableFrom(newComponent.GetType())) {
                                    field.SetValue(this, newComponent);
                                    result = true;
                                }

                                return result;
                            }));
                        }
                    }
                }
            }
        }

        private void Self_EnabledChanged(object sender, EventArgs e) {
            if (this._isVisible) {
                this.IsVisibleChanged.SafeInvoke(this);
            }
        }
    }
}