namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// An empty object in the hierarchy.
/// </summary>
public class EmptyObject : IScene {
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static readonly EmptyObject Instance = new();

    /// <inheritdoc />
    public event EventHandler? Activated;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? Deactivated;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public event EventHandler? TransformChanged;

    /// <inheritdoc />
    public BoundingArea BoundingArea => BoundingArea.Empty;

    /// <inheritdoc />
    public bool IsActive => false;

    /// <inheritdoc />
    public bool ShouldUpdate => false;

    /// <inheritdoc />
    public SceneState State { get; } = new();

    /// <inheritdoc />
    public Vector2 WorldPosition => Vector2.Zero;

    /// <inheritdoc />
    public Color BackgroundColor {
        get => Color.HotPink;
        set { }
    }

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
    public Vector2 LocalPosition {
        get => Vector2.Zero;
        set { }
    }

    /// <inheritdoc />
    public string Name {
        get => "Empty";
        set { }
    }

    /// <inheritdoc />
    public Vector2 TileSize {
        get => Vector2.One;
        set { }
    }

    /// <inheritdoc />
    public Version Version { get; set; } = new();

    /// <inheritdoc />
    public T AddChild<T>() where T : IEntity, new() => throw new NotSupportedException("Initialization has not occured.");

    /// <inheritdoc />
    public IEntity AddChild() => throw new NotSupportedException("Initialization has not occured.");

    /// <inheritdoc />
    public void AddChild(IEntity entity) {
        throw new NotSupportedException("Initialization has not occured.");
    }

    /// <inheritdoc />
    public T AddSystem<T>() where T : IGameSystem, new() => new();


    /// <inheritdoc />
    public void AddSystem(IGameSystem system) {
    }

    /// <inheritdoc />
    public void ClearChildren() {
    }

    /// <inheritdoc />
    public bool ContainsEntity(Guid id) => false;

    /// <inheritdoc />
    public void Deinitialize() {
    }

    /// <inheritdoc />
    public IEntity FindChild(Guid id) => this;

    /// <inheritdoc />
    public IEntity FindChild(string name) => this;

    /// <inheritdoc />
    public TEntity? FindEntity<TEntity>(Guid id) where TEntity : class, IEntity => null;

    /// <inheritdoc />
    public TSystem? FindSystem<TSystem>(Guid id) where TSystem : class, IGameSystem => null;

    /// <inheritdoc />
    public IEnumerable<T> GetDescendants<T>() => [];

    /// <inheritdoc />
    public IEnumerable<IEntity> GetDescendants(Type type) => [];

    /// <inheritdoc />
    public IEnumerable<IEntity> GetDescendentsWithContent(Guid contentId) => [];

    /// <inheritdoc />
    public Vector2 GetNearestTilePosition(Vector2 position) => throw new NotSupportedException();

    /// <inheritdoc />
    public T GetOrAddChild<T>() where T : class, IEntity, new() => throw new NotSupportedException("Initialization has not occured.");

    /// <inheritdoc />
    public T? GetSystem<T>() where T : class, IGameSystem => null;

    /// <inheritdoc />
    public Vector2 GetTilePosition(Point tile) => throw new NotSupportedException();

    /// <inheritdoc />
    public Vector2 GetWorldPosition(Vector2 originOffset) => this.WorldPosition;


    /// <inheritdoc />
    public void Initialize(IScene scene, IEntity parent) {
        throw new NotSupportedException("An empty entity cannot be initialized.");
    }

    /// <inheritdoc />
    public void Initialize(IGame game, IAssetManager assetManager) {
    }

    /// <inheritdoc />
    public void InsertChild(int index, IEntity entity) {
        throw new NotSupportedException("Initialization has not occured.");
    }

    /// <inheritdoc />
    public void InsertSystem(int index, IGameSystem system) {
    }

    /// <inheritdoc />
    public void Invoke(Action action) {
    }

    /// <inheritdoc />
    public bool IsDescendentOf(IEntity entity) => false;

    /// <inheritdoc />
    public void LoadAssets(IAssetManager assets, IGame game) {
    }

    /// <inheritdoc />
    public void Move(Vector2 amount) {
    }

    /// <inheritdoc />
    public void OnRemovedFromSceneTree() {
        throw new NotSupportedException("An empty entity should never be added to a scene tree, much less removed.");
    }

    /// <inheritdoc />
    public void RaiseActivated() {
    }

    /// <inheritdoc />
    public void RaiseDeactivated() {
    }

    /// <inheritdoc />
    public bool ReferencesContent(Guid contentId) => false;

    /// <inheritdoc />
    public void RegisterEntity(IEntity entity) {
    }

    /// <inheritdoc />
    public void RemoveChild(IEntity entity) {
    }

    /// <inheritdoc />
    public bool RemoveSystem(IGameSystem system) => false;

    /// <inheritdoc />
    public void Render(FrameTime frameTime, InputState inputState) {
    }

    /// <inheritdoc />
    public void ReorderChild(IEntity entity, int newIndex) {
    }

    /// <inheritdoc />
    public void ReorderSystem(IGameSystem system, int newIndex) {
    }

    /// <inheritdoc />
    public T ResolveDependency<T>() where T : new() => new();


    /// <inheritdoc />
    public T ResolveDependency<T>(Func<T> objectFactory) where T : class => objectFactory.SafeInvoke();

    /// <inheritdoc />
    public void SetWorldPosition(Vector2 position) {
    }

    /// <inheritdoc />
    public bool TryGetAncestor<T>([NotNullWhen(true)] out T? entity) {
        entity = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetChild<T>([NotNullWhen(true)] out T? entity) where T : class, IEntity {
        entity = null;
        return false;
    }

    /// <inheritdoc />
    public void UnregisterEntity(IEntity entity) {
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
    }
}