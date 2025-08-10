namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for a <see cref="ITransformable" /> descendent of <see cref="IScene" /> which
/// holds a collection of <see cref="IEntity" />.
/// </summary>
public interface IEntity : IEnableable, IIdentifiable, INameable, INotifyPropertyChanged, ITransformable {

    /// <summary>
    /// An event that is called when <see cref="IEnableable.IsEnabled" /> changes.
    /// </summary>
    event EventHandler? IsEnabledChanged;

    /// <summary>
    /// Gets the children.
    /// </summary>
    IReadOnlyCollection<IEntity> Children => Array.Empty<IEntity>();

    /// <summary>
    /// Gets the game.
    /// </summary>
    IGame Game { get; }

    /// <summary>
    /// Gets the layers.
    /// </summary>
    Layers Layers { get; set; }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    IEntity Parent => GridContainer.Empty;

    /// <summary>
    /// Gets the game project.
    /// </summary>
    IGameProject Project => GameProject.Empty;

    /// <summary>
    /// Gets the scene.
    /// </summary>
    IScene Scene => EmptyObject.Scene;

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
    /// Removes all children.
    /// </summary>
    void ClearChildren();

    /// <summary>
    /// Deinitializes the entity.
    /// </summary>
    void Deinitialize();

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
    /// Get all descendants of the specified type.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <returns>Descendants of the specified type.</returns>
    IEnumerable<T> GetDescendants<T>();

    /// <summary>
    /// Gets all descendants of the specified type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Descendants of the specified type.</returns>
    IEnumerable<IEntity> GetDescendants(Type type);

    /// <summary>
    /// Gets all descendants that contain the specified content.
    /// </summary>
    /// <param name="contentId">The content identifier.</param>
    /// <returns>The entities with that content.</returns>
    IEnumerable<IEntity> GetDescendantsWithContent(Guid contentId);

    /// <summary>
    /// Gets the child of the specified type if it exists; otherwise, adds a new child.
    /// </summary>
    /// <typeparam name="T">The type of child to find.</typeparam>
    /// <returns>The entity that was found or added.</returns>
    T GetOrAddChild<T>() where T : class, IEntity, new();

    /// <summary>
    /// Gets children of the specified type up to a certain amount.
    /// </summary>
    /// <remarks>
    /// If there are not enough children, more will be added automatically.  If this entity
    /// has more children than requested, it will not delete the additional children.
    /// </remarks>
    /// <param name="amountOfChildren">The amount of children to get.</param>
    /// <typeparam name="T">The type of child to find.</typeparam>
    /// <returns>The entity that was found or added.</returns>
    IReadOnlyCollection<T> GetOrAddChildren<T>(int amountOfChildren) where T : class, IEntity, new();

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
    /// Loads assets for an entity before initialization.
    /// </summary>
    /// <param name="assets">The assets.</param>
    /// <param name="game">The game.</param>
    void LoadAssets(IAssetManager assets, IGame game);

    /// <summary>
    /// Called when this instance is removed from the <see cref="IScene" /> tree.
    /// </summary>
    void OnRemovedFromSceneTree();

    /// <summary>
    /// Checks whether this instance references the specified content.
    /// </summary>
    /// <param name="contentId">The content identifier.</param>
    /// <returns>A value indicating whether or not this instance </returns>
    bool ReferencesContent(Guid contentId);

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
    /// Tries the get an ancestor of specified type. This entity could be a parent going all the way up to the scene.
    /// </summary>
    /// <typeparam name="T">The type of parent entity.</typeparam>
    /// <param name="entity">The parent entity.</param>
    /// <returns>A value indicating whether or not the entity was found.</returns>
    bool TryGetAncestor<T>([NotNullWhen(true)] out T? entity);

    /// <summary>
    /// Tries the get a child of the specific type. This is recursive.
    /// </summary>
    /// <typeparam name="T">The type of entity for which to search.</typeparam>
    /// <param name="entity">The child entity.</param>
    /// <returns>
    /// A value indicating whether a child of the specified type was found.
    /// </returns>
    bool TryGetChild<T>([NotNullWhen(true)] out T? entity) where T : class, IEntity;
}

/// <summary>
/// A <see cref="ITransformable" /> descendent of <see cref="IScene" /> which holds a
/// collection of <see cref="IEntity" />
/// </summary>
[Category("Entity")]
public class Entity : Transformable, IEntity {
    [DataMember]
    private readonly EntityCollection _children = new();

    private bool _isEnabled = true;
    private string _name = string.Empty;

    /// <inheritdoc />
    public event EventHandler? IsEnabledChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity" /> class.
    /// </summary>
    public Entity() : base() {
        this.PropertyChanged += this.OnPropertyChanged;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<IEntity> Children => this._children;

    /// <inheritdoc />
    public virtual IGame Game => this.Scene.Game;

    /// <inheritdoc />
    [DataMember]
    [Browsable(false)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    [DataMember]
    public bool IsEnabled {
        get => this._isEnabled;
        set {
            if (this.Set(ref this._isEnabled, value)) {
                this.OnIsEnableChanged();
            }
        }
    }

    /// <inheritdoc />
    [DataMember]
    public Layers Layers { get; set; } = Layers.Default;

    /// <inheritdoc />
    [DataMember]
    public string Name {
        get => this._name;
        set {
            this._name = value;

            if (BaseGame.IsDesignMode) {
                this.RaisePropertyChanged();
            }
        }
    }

    /// <inheritdoc />
    public IEntity Parent { get; private set; } = EmptyObject.Entity;

    /// <summary>
    /// Gets the project.
    /// </summary>
    public IGameProject Project => this.Game.Project;

    /// <inheritdoc />
    public IScene Scene { get; private set; } = EmptyObject.Scene;

    /// <summary>
    /// Gets a value indicating whether this is initialized.
    /// </summary>
    protected bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets the sprite batch.
    /// </summary>
    protected SpriteBatch? SpriteBatch => this.Game.SpriteBatch;

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
    public IEntity AddChild() => this.AddChild<Entity>();

    /// <inheritdoc />
    public void AddChild(IEntity entity) {
        if (this.CanAddChild(entity)) {
            entity.Parent.RemoveChild(entity);
            this._children.Add(entity);
            this.OnAddChild(entity);
        }
    }

    /// <inheritdoc />
    public void ClearChildren() {
        foreach (var child in this.Children.ToList()) {
            this.RemoveChild(child);
        }
    }

    /// <inheritdoc />
    public virtual void Deinitialize() {
        if (this.IsInitialized) {
            this.Parent.TransformChanged -= this.Parent_TransformChanged;

            foreach (var assetReference in this.GetAssetReferences()) {
                assetReference.Deinitialize();
            }

            foreach (var entityReference in this.GetEntityReferences()) {
                entityReference.Deinitialize();
            }

            foreach (var child in this.Children) {
                child.Deinitialize();
            }

            this.IsInitialized = false;
        }
    }

    /// <inheritdoc />
    public IEntity FindChild(Guid id) {
        var result = EmptyObject.Entity;
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
        var result = EmptyObject.Entity;
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
    public IEnumerable<T> GetDescendants<T>() {
        foreach (var child in this.Children.OfType<T>()) {
            yield return child;
        }

        foreach (var child in this.Children.SelectMany(x => x.GetDescendants<T>())) {
            yield return child;
        }
    }

    /// <inheritdoc />
    public IEnumerable<IEntity> GetDescendants(Type type) {
        foreach (var child in this.Children.Where(type.IsInstanceOfType)) {
            yield return child;
        }

        foreach (var child in this.Children.SelectMany(x => x.GetDescendants(type))) {
            yield return child;
        }
    }

    /// <inheritdoc />
    public IEnumerable<IEntity> GetDescendantsWithContent(Guid contentId) {
        foreach (var child in this.Children.Where(x => x.ReferencesContent(contentId))) {
            yield return child;
        }

        foreach (var child in this.Children.SelectMany(x => x.GetDescendantsWithContent(contentId))) {
            yield return child;
        }
    }

    /// <inheritdoc />
    public T GetOrAddChild<T>() where T : class, IEntity, new() => this.TryGetChild<T>(out var entity) ? entity : this.AddChild<T>();

    /// <inheritdoc />
    public IReadOnlyCollection<T> GetOrAddChildren<T>(int amountOfChildren) where T : class, IEntity, new() {
        if (amountOfChildren <= 0) {
            return [];
        }

        var children = this.Children.OfType<T>().ToList();

        if (children.Count > amountOfChildren) {
            return children.Take(amountOfChildren).ToList();
        }

        while (children.Count < amountOfChildren) {
            children.Add(this.AddChild<T>());
        }

        return children;
    }

    /// <inheritdoc />
    public virtual void Initialize(IScene scene, IEntity parent) {
        try {
            if (this.IsInitialized) {
                this.Deinitialize();
            }

            this.Scene = scene;
            this.Parent = parent;
            this.Scene.RegisterEntity(this);

            foreach (var entityReference in this.GetEntityReferences()) {
                entityReference.Initialize(this.Scene);
            }

            foreach (var child in this.Children) {
                child.Initialize(this.Scene, this);
            }

            this.HandleTransformed();
            this.Parent.TransformChanged += this.Parent_TransformChanged;
        }
        finally {
            this.IsInitialized = true;
        }
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
    public bool IsDescendentOf(IEntity entity) => entity == this.Parent || this.Parent != this.Parent.Parent && this.Parent.IsDescendentOf(entity);

    /// <summary>
    /// Gets a value indicating whether the entity is null or empty.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="notNullEntity">The entity if it is not null without its nullability.</param>
    /// <returns>A value indicating whether the entity is null or empty.</returns>
    public static bool IsNullOrEmpty(IEntity? entity, out IEntity notNullEntity) {
        notNullEntity = entity ?? EmptyObject.Entity;
        return notNullEntity == EmptyObject.Entity || notNullEntity == GridContainer.Empty;
    }

    /// <inheritdoc />
    public virtual void LoadAssets(IAssetManager assets, IGame game) {
        foreach (var assetReference in this.GetAssetReferences()) {
            assetReference.Initialize(assets, game);
        }

        foreach (var child in this.Children) {
            child.LoadAssets(assets, game);
        }
    }

    /// <inheritdoc />
    public virtual void OnRemovedFromSceneTree() {
        this.Deinitialize();
        this.Scene.UnregisterEntity(this);
        this.Scene = EmptyObject.Scene;
        this.Parent = EmptyObject.Entity;

        foreach (var child in this._children) {
            child.OnRemovedFromSceneTree();
        }
    }

    /// <summary>
    /// Gets a value indicating whether this references the specified content.
    /// </summary>
    /// <param name="contentId">The content identifier.</param>
    /// <returns>A value indicating whether this references the specified content.</returns>
    public bool ReferencesContent(Guid contentId) => this.GetAssetReferences().Any(x => x.GetContentIds().Contains(contentId));

    /// <inheritdoc />
    public void RemoveChild(IEntity entity) {
        if (!Framework.Scene.IsNullOrEmpty(this.Scene)) {
            this.Scene.Invoke(() => this.PerformChildRemoval(entity));
        }
        else {
            this._children.Remove(entity);
        }

        this.OnRemoveChild(entity);
    }

    /// <inheritdoc />
    public void ReorderChild(IEntity entity, int newIndex) {
        var originalIndex = this._children.IndexOf(entity);
        this._children.Move(originalIndex, newIndex);
    }

    /// <inheritdoc />
    public virtual bool TryGetAncestor<T>([NotNullWhen(true)] out T? entity) {
        if (this.Parent is T parent) {
            entity = parent;
        }
        else {
            this.Parent.TryGetAncestor(out entity);
        }

        return entity != null;
    }

    /// <inheritdoc />
    public bool TryGetChild<T>([NotNullWhen(true)] out T? entity) where T : class, IEntity {
        entity = this.Children.OfType<T>().FirstOrDefault();
        return entity != null;
    }

    /// <summary>
    /// Removes the entity without any regard to whether it is a child.
    /// </summary>
    /// <remarks>This should only be used in the editor.</remarks>
    /// <param name="entity">The entity to remove.</param>
    protected void ForceChildRemoval(IEntity entity) {
        entity.OnRemovedFromSceneTree();
        this._children.Remove(entity);
    }

    /// <summary>
    /// Gets the asset references for initalization and deinitialization.
    /// </summary>
    /// <returns>The asset references</returns>
    protected virtual IEnumerable<IAssetReference> GetAssetReferences() {
        yield break;
    }

    /// <summary>
    /// Gets the entity references for initalization and deinitialization.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerable<IEntityReference> GetEntityReferences() {
        yield break;
    }

    /// <inheritdoc />
    protected override ITransformable GetParentTransformable() => this.Parent;

    /// <summary>
    /// Occurs when a child is added.
    /// </summary>
    /// <param name="child">The child.</param>
    protected virtual void OnAddChild(IEntity child) {
        if (!Framework.Scene.IsNullOrEmpty(this.Scene)) {
            this.Scene.Invoke(() =>
            {
                child.LoadAssets(this.Scene.Assets, this.Game);
                child.Initialize(this.Scene, this);
            });
        }
    }

    /// <summary>
    /// Called when <see cref="IsEnabled" /> changes.
    /// </summary>
    protected virtual void OnIsEnableChanged() {
        this.IsEnabledChanged.SafeInvoke(this);
    }

    /// <summary>
    /// Called when a property on the current object changes.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">
    /// The <see cref="PropertyChangedEventArgs" /> instance containing the event data.
    /// </param>
    protected virtual void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (this.TransformInheritance != TransformInheritance.None && e.PropertyName == nameof(this.Parent)) {
            this.HandleTransformed();
        }
    }

    /// <summary>
    /// Occurs when a child is removed.
    /// </summary>
    /// <param name="child">The child.</param>
    protected virtual void OnRemoveChild(IEntity child) {
    }

    private bool CanAddChild(IEntity entity) {
        return entity != this && this.Children.All(x => x != entity) && !this.IsDescendentOf(entity);
    }

    private void Parent_TransformChanged(object? sender, EventArgs e) {
        if (this.TransformInheritance != TransformInheritance.None) {
            this.HandleTransformed();
        }
    }

    private void PerformChildRemoval(IEntity entity) {
        if (this._children.Contains(entity)) {
            this.ForceChildRemoval(entity);
        }
    }
}