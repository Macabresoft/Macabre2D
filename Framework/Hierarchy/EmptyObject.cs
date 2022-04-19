namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// An empty object in the hierarchy.
/// </summary>
internal class EmptyObject : IScene {
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static readonly EmptyObject Instance = new();

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public Transform Transform => Transform.Origin;

    /// <inheritdoc />
    public Matrix TransformMatrix => Matrix.Identity;

    /// <inheritdoc />
    public Vector2 WorldTileSize => this.LocalTileSize;

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
    public Vector2 LocalScale {
        get => Vector2.One;
        set { }
    }

    /// <inheritdoc />
    public Vector2 LocalTileSize {
        get => Vector2.One;
        set { }
    }

    /// <inheritdoc />
    public string Name {
        get => "Empty";
        set { }
    }

    /// <inheritdoc />
    public Version Version { get; set; } = new();

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
    public T AddLoop<T>() where T : ILoop, new() {
        return new T();
    }

    /// <inheritdoc />
    public void AddLoop(ILoop loop) {
    }

    /// <inheritdoc />
    public IEntity FindChild(Guid id) {
        return this;
    }

    /// <inheritdoc />
    public IEntity FindChild(string name) {
        return this;
    }

    /// <inheritdoc />
    public IEnumerable<T> GetDescendents<T>() {
        return Enumerable.Empty<T>();
    }

    /// <inheritdoc />
    public T? GetLoop<T>() where T : class, ILoop {
        return null;
    }

    /// <inheritdoc />
    public Vector2 GetNearestTilePosition(Vector2 position) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public T GetOrAddChild<T>() where T : class, IEntity, new() {
        throw new NotSupportedException("Initialization has not occured.");
    }

    /// <inheritdoc />
    public Vector2 GetTilePosition(Point tile) {
        throw new NotImplementedException();
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
    public Transform GetWorldTransform(Vector2 originOffset, Rotation rotation) {
        return this.Transform;
    }

    /// <inheritdoc />
    public Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale, Rotation rotation) {
        return this.Transform;
    }

    /// <inheritdoc />
    public Transform GetWorldTransform(Vector2 originOffset, Vector2 overrideScale) {
        return this.Transform;
    }

    /// <inheritdoc />
    public void Initialize(IScene scene, IEntity parent) {
        throw new NotSupportedException("An empty entity cannot be initialized.");
    }

    /// <inheritdoc />
    public void Initialize(IGame gameLoop, IAssetManager assetManager) {
    }

    /// <inheritdoc />
    public void InsertChild(int index, IEntity entity) {
        throw new NotSupportedException("Initialization has not occured.");
    }

    /// <inheritdoc />
    public void InsertLoop(int index, ILoop loop) {
    }

    /// <inheritdoc />
    public void Invoke(Action action) {
    }

    /// <inheritdoc />
    public bool IsDescendentOf(IEntity entity) {
        return false;
    }

    /// <inheritdoc />
    public void Move(Vector2 amount) {
    }

    /// <inheritdoc />
    public void OnRemovedFromSceneTree() {
        throw new NotSupportedException("An empty entity should never be added to a scene tree, much less removed.");
    }

    /// <inheritdoc />
    public void RegisterEntity(IEntity entity) {
    }

    /// <inheritdoc />
    public void RemoveChild(IEntity entity) {
    }

    /// <inheritdoc />
    public bool RemoveLoop(ILoop loop) {
        return false;
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, InputState inputState) {
    }

    /// <inheritdoc />
    public void ReorderChild(IEntity entity, int newIndex) {
    }

    /// <inheritdoc />
    public void ReorderLoop(ILoop loop, int newIndex) {
    }

    /// <inheritdoc />
    public T ResolveDependency<T>() where T : new() {
        return new T();
    }

    /// <inheritdoc />
    public T ResolveDependency<T>(Func<T> objectFactory) where T : class {
        return objectFactory.SafeInvoke();
    }

    /// <inheritdoc />
    public void SetWorldPosition(Vector2 position) {
    }

    /// <inheritdoc />
    public void SetWorldScale(Vector2 scale) {
    }

    /// <inheritdoc />
    public void SetWorldTransform(Vector2 position, Vector2 scale) {
    }

    /// <inheritdoc />
    public void SetWorldTransform(Transform transform) {
    }

    /// <inheritdoc />
    public bool TryGetChild<T>(out T? entity) where T : class, IEntity {
        entity = null;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetParentEntity<T>(out T? entity) {
        entity = default;
        return false;
    }

    /// <inheritdoc />
    public void UnregisterEntity(IEntity entity) {
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
    }
}