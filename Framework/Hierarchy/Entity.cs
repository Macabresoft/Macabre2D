namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for a <see cref="ITransformable" /> descendent of <see cref="IScene" /> which
/// holds a collection of <see cref="IEntity" />.
/// </summary>
public interface IEntity : IEnableable, IIdentifiable, INameable, INotifyPropertyChanged, ITransformable {
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
    /// Finds the first child with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The child if it exists.</returns>
    IEntity FindChild(Guid id);

    /// <summary>
    /// Finds the first child with the specified name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The child if it exists.</returns>
    IEntity FindChild(string name);

    /// <summary>
    /// Get all descendents of the specified type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>Descendents of the specified type.</returns>
    IEnumerable<T> GetDescendents<T>();

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
    /// Inserts the child at the provided index.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="entity">The entity.</param>
    void InsertChild(int index, IEntity entity);

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
    /// Reorders the children so the specified entity is moved to the specified index.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="newIndex">The new index.</param>
    void ReorderChild(IEntity entity, int newIndex);

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
    bool TryGetParentEntity<T>(out T? entity);
}

/// <summary>
/// A <see cref="ITransformable" /> descendent of <see cref="IScene" /> which holds a
/// collection of <see cref="IEntity" />
/// </summary>
[Category("Entity")]
public class Entity : Transformable, IEntity {
    [DataMember]
    private readonly EntityCollection _children = new();

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

    /// <summary>
    /// Gets the default empty <see cref="IEntity" /> that is present before initialization.
    /// </summary>
    public static IEntity Empty => EmptyObject.Instance;

    /// <summary>
    /// Gets the game.
    /// </summary>
    public virtual IGame Game => this.Scene.Game;

    /// <inheritdoc />
    [DataMember]
    [EditorExclude]
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

    /// <summary>
    /// Gets the settings.
    /// </summary>
    protected IGameSettings Settings => this.Game.Project.Settings;

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
    public IEntity FindChild(Guid id) {
        var result = Empty;
        foreach (var child in this.Children) {
            if (child.Id == id) {
                result = child;
                break;
            }

            if (!IsNullOrEmpty(child.FindChild(id), out result)) {
                break;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public IEntity FindChild(string name) {
        var result = Empty;
        foreach (var child in this.Children) {
            if (child.Name == name) {
                result = child;
                break;
            }

            if (!IsNullOrEmpty(child.FindChild(name), out result)) {
                break;
            }
        }

        return result;
    }

    /// <inheritdoc />
    public IEnumerable<T> GetDescendents<T>() {
        var descendents = new List<T>(this.Children.OfType<T>());
        descendents.AddRange(this.Children.SelectMany(x => x.GetDescendents<T>()));
        return descendents;
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
    public void InsertChild(int index, IEntity entity) {
        if (this.CanAddChild(entity)) {
            entity.Parent.RemoveChild(entity);
            this._children.InsertOrAdd(index, entity);
            this.OnAddChild(entity);
        }
    }

    /// <inheritdoc />
    public bool IsDescendentOf(IEntity entity) {
        return entity == this.Parent || this.Parent != this.Parent.Parent && this.Parent.IsDescendentOf(entity);
    }

    /// <summary>
    /// Gets a value indicating whether or not the entity is null or empty.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="notNullEntity">The entity if it is not null without its nullability.</param>
    /// <returns>A value indicating whether or not the entity is null or empty.</returns>
    public static bool IsNullOrEmpty(IEntity? entity, out IEntity notNullEntity) {
        notNullEntity = entity ?? Empty;
        return notNullEntity == Empty || notNullEntity == Framework.Scene.Empty;
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
    public void ReorderChild(IEntity entity, int newIndex) {
        var originalIndex = this._children.IndexOf(entity);
        this._children.Move(originalIndex, newIndex);
    }

    /// <inheritdoc />
    public bool TryGetChild<T>(out T? entity) where T : class, IEntity {
        entity = this.Children.OfType<T>().FirstOrDefault();
        return entity != null;
    }

    /// <inheritdoc />
    public virtual bool TryGetParentEntity<T>(out T? entity) {
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
        if (this.IsTransformRelativeToParent && e.PropertyName == nameof(this.Parent)) {
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
        if (this.IsTransformRelativeToParent && e.PropertyName == nameof(ITransformable.Transform)) {
            this.HandleMatrixOrTransformChanged();
        }
    }

    private void PerformChildRemoval(IEntity entity) {
        if (this._children.Contains(entity)) {
            entity.OnRemovedFromSceneTree();
            this._children.Remove(entity);
        }
    }
}