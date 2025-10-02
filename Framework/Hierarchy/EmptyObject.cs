namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// An empty object in the hierarchy.
/// </summary>
public class EmptyObject : ICamera, IInputSystem, IPhysicsBody, IQueueableSpriteAnimator, ITextRenderer, IScene, ISpriteRenderer, IRenderableBlinker, ITileableEntity {
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static readonly EmptyObject Instance = new();

    /// <inheritdoc />
    public event EventHandler? Activated {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler<CollisionEventArgs>? CollisionOccured {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler? Deactivated {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler? IsEnabledChanged {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler<SpriteAnimation?>? OnAnimationFinished {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler? RenderOrderChanged {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler? ShouldAnimateChanged {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler? ShouldRenderChanged {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler? TilesChanged {
        add { }
        remove { }
    }

    /// <inheritdoc />
    public event EventHandler? TransformChanged {
        add { }
        remove { }
    }

    /// <inheritdoc />
    event EventHandler? IPhysicsBody.UpdateOrderChanged {
        add { }
        remove { }
    }

    /// <inheritdoc />
    event EventHandler? IUpdateableEntity.UpdateOrderChanged {
        add { }
        remove { }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptyObject" /> class.
    /// </summary>
    protected EmptyObject() {
    }

    /// <inheritdoc />
    public IReadOnlyCollection<Point> ActiveTiles { get; } = [];

    /// <inheritdoc />
    public float ActualViewHeight => 1f;

    /// <inheritdoc />
    public Color BackgroundColor {
        get => Color.HotPink;
        set { }
    }

    /// <summary>
    /// Gets the singleton instance as <see cref="IBoundableEntity" />.
    /// </summary>
    public static IBoundableEntity Boundable => Instance;

    /// <inheritdoc />
    public BoundingArea BoundingArea => BoundingArea.Empty;

    /// <inheritdoc />
    public SpriteAnimation? CurrentAnimation => null;

    /// <inheritdoc />
    public IGridContainer CurrentGrid => this;

    /// <inheritdoc />
    public GameTimer DelayTimer { get; } = new();

    /// <inheritdoc />
    public GameTimer DisappearTimer { get; } = new();

    /// <inheritdoc />
    public bool EndImmediately {
        get => true;
        set { }
    }

    /// <summary>
    /// Gets the singleton instance as <see cref="IEntity" />.
    /// </summary>
    public static IEntity Entity => Instance;

    /// <inheritdoc />
    public FontCategory FontCategory { get; set; } = FontCategory.None;

    /// <inheritdoc />
    public SpriteSheetFontReference FontReference { get; } = new();

    /// <inheritdoc />
    public string Format {
        get => string.Empty;
        set { }
    }

    /// <inheritdoc />
    public ByteOverride FrameRateOverride { get; } = new();

    /// <inheritdoc />
    public IGame Game => BaseGame.Empty;

    /// <inheritdoc />
    public bool HasCollider => false;

    /// <inheritdoc />
    public float HorizontalAxis => 0f;

    /// <inheritdoc />
    public Guid Id {
        get => Guid.Empty;
        set { }
    }

    /// <inheritdoc />
    public bool IsActive => false;

    /// <inheritdoc />
    public bool IsEnabled {
        get => false;
        set { }
    }

    /// <inheritdoc />
    public bool IsLooping => false;

    /// <inheritdoc />
    public bool IsPlaying => false;

    /// <inheritdoc />
    public bool IsTrigger => false;

    /// <inheritdoc />
    public int Kerning {
        get => 0;
        set { }
    }

    /// <inheritdoc />
    public Layers Layers {
        get => Layers.None;
        set { }
    }

    /// <inheritdoc />
    public Layers LayersToExcludeFromRender => default;

    /// <inheritdoc />
    public Layers LayersToRender {
        get => default;
        set { }
    }

    /// <inheritdoc />
    public Vector2 LocalPosition {
        get => Vector2.Zero;
        set { }
    }

    /// <inheritdoc />
    public Point MaximumTile => Point.Zero;

    /// <inheritdoc />
    public Point MinimumTile => Point.Zero;

    /// <inheritdoc />
    public string Name {
        get => "Empty";
        set { }
    }

    /// <inheritdoc />
    public OffsetOptions OffsetOptions { get; } = new();

    /// <inheritdoc />
    public PhysicsMaterial PhysicsMaterial => PhysicsMaterial.Empty;

    /// <inheritdoc />
    public QueueableSpriteAnimation? QueuedAnimation => null;

    /// <summary>
    /// Gets the singleton instance as <see cref="IRenderableEntity" />.
    /// </summary>
    public static IRenderableEntity Renderer => Instance;

    /// <inheritdoc />
    public RenderOptions RenderOptions { get; } = new();

    /// <inheritdoc cref="IRenderableEntity.RenderOrder" />
    public int RenderOrder {
        get => 0;
        set { }
    }

    /// <inheritdoc />
    public bool RenderOutOfBounds {
        get => false;
        set { }
    }

    /// <inheritdoc />
    public RenderPriority RenderPriority { get; set; } = default;

    /// <inheritdoc />
    public string ResourceName {
        get => string.Empty;
        set { }
    }

    /// <inheritdoc />
    public BoundingArea SafeArea => BoundingArea.Empty;

    /// <summary>
    /// Gets the singleton instance as <see cref="IScene" />.
    /// </summary>
    public static IScene Scene => Instance;

    /// <inheritdoc />
    public bool ShouldAnimate => false;

    /// <inheritdoc />
    public bool ShouldRender {
        get => false;
        set { }
    }

    /// <inheritdoc />
    public bool ShouldUpdate => false;

    /// <inheritdoc />
    public GameTimer ShowTimer { get; } = new();

    /// <inheritdoc />
    public float SpeedMultiplier {
        get => 1f;
        set { }
    }

    /// <inheritdoc />
    public SpriteReference SpriteReference { get; } = new();

    /// <inheritdoc />
    public SceneState State { get; } = new();

    /// <inheritdoc />
    public string Text {
        get => string.Empty;
        set { }
    }

    /// <summary>
    /// Gets the singleton instance as <see cref="ITextRenderer" />.
    /// </summary>
    public static ITextRenderer TextRenderer => Instance;

    /// <inheritdoc />
    public Vector2 TileSize {
        get => Vector2.One;
        set { }
    }

    /// <inheritdoc />
    public TransformInheritance TransformInheritance {
        get => TransformInheritance.None;
        set { }
    }

    /// <inheritdoc cref="IUpdateableEntity.UpdateOrder" />
    public int UpdateOrder => 0;

    /// <inheritdoc />
    public float VerticalAxis => 0f;

    /// <inheritdoc />
    public float ViewHeight {
        get => 1f;
        set { }
    }

    /// <inheritdoc />
    public float ViewWidth => 1f;

    /// <inheritdoc />
    public Vector2 WorldPosition => Vector2.Zero;

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
    public bool AddTile(Point tile) => false;

    /// <inheritdoc />
    public void BeginBlink(byte numberOfBlinks, Action? finishedCallback) {
    }

    /// <inheritdoc />
    public void ClearChildren() {
    }

    /// <inheritdoc />
    public void ClearTiles() {
    }

    /// <inheritdoc />
    public bool ContainsEntity(Guid id) => false;

    /// <inheritdoc />
    public Vector2 ConvertPointFromScreenSpaceToWorldSpace(Point point) => Vector2.Zero;

    /// <inheritdoc />
    public void Deinitialize() {
    }

    /// <inheritdoc />
    public void Enqueue(SpriteAnimation animation, bool shouldLoopIndefinitely) {
    }

    /// <inheritdoc />
    public void Enqueue(SpriteAnimationReference animationReference, bool shouldLoopIndefinitely) {
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
    public IEnumerable<Collider> GetColliders() => [];

    /// <inheritdoc />
    public IEnumerable<T> GetDescendants<T>() => [];

    /// <inheritdoc />
    public IEnumerable<IEntity> GetDescendants(Type type) => [];

    /// <inheritdoc />
    public IEnumerable<IEntity> GetDescendantsWithContent(Guid contentId) => [];

    /// <inheritdoc />
    public string GetFullText() => string.Empty;

    /// <inheritdoc />
    public InputActionState GetInputActionState(InputAction action) => InputActionState.None;

    /// <inheritdoc />
    public InputActionState GetInputActionState(InputAction action, InputKind inputKind) => InputActionState.None;

    /// <inheritdoc />
    public Vector2 GetNearestTilePosition(Vector2 position) => throw new NotSupportedException();

    /// <inheritdoc />
    public T GetOrAddChild<T>() where T : class, IEntity, new() => throw new NotSupportedException("Initialization has not occured.");

    /// <inheritdoc />
    public IReadOnlyCollection<T> GetOrAddChildren<T>(int amountOfChildren) where T : class, IEntity, new() => [];

    /// <inheritdoc />
    public T GetOrAddSystem<T>() where T : class, IGameSystem, new() => throw new NotSupportedException("Initialization has not occured.");

    /// <inheritdoc />
    public float GetPercentageComplete() => 0f;

    /// <inheritdoc />
    public T? GetSystem<T>() where T : class, IGameSystem => null;

    /// <inheritdoc />
    public BoundingArea GetTileBoundingArea(Point tile) => BoundingArea.Empty;

    /// <inheritdoc />
    public Vector2 GetTilePosition(Point tile) => Vector2.Zero;

    /// <inheritdoc />
    public Point GetTileThatContains(Vector2 worldPosition) => Point.Zero;

    /// <inheritdoc />
    public Vector2 GetWorldPosition(Vector2 originOffset) => this.WorldPosition;

    /// <inheritdoc />
    public bool HasActiveTileAt(Point tilePosition) => false;

    /// <inheritdoc />
    public bool HasActiveTileAt(Vector2 worldPosition) => false;

    /// <inheritdoc />
    public void IncrementTime(FrameTime frameTime) {
    }

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
    public bool IsHeld(InputAction action) => false;

    /// <inheritdoc />
    public bool IsPressed(InputAction action, InputKind inputKind) => false;

    /// <inheritdoc />
    public bool IsPressed(InputAction action) => false;

    /// <inheritdoc />
    public bool IsReleased(InputAction action) => false;

    /// <inheritdoc />
    public void LoadAssets(IAssetManager assets, IGame game) {
    }

    /// <inheritdoc />
    public void Move(Vector2 amount) {
    }

    /// <inheritdoc />
    public void NextFrame() {
    }

    /// <inheritdoc />
    public void NotifyCollisionOccured(CollisionEventArgs eventArgs) {
    }

    /// <inheritdoc />
    public void OnRemovedFromSceneTree() {
        throw new NotSupportedException("An empty entity should never be added to a scene tree, much less removed.");
    }

    /// <inheritdoc />
    public void OnSceneTreeLoaded() {
    }

    /// <inheritdoc />
    public void Pause() {
    }

    /// <inheritdoc />
    public void Play() {
    }

    /// <inheritdoc />
    public void Play(SpriteAnimation animation, bool shouldLoop) {
    }

    /// <inheritdoc />
    public void Play(SpriteAnimationReference animationReference, bool shouldLoop) {
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
    public bool RemoveTile(Point tile) => false;

    /// <inheritdoc />
    public void Render(FrameTime frameTime, InputState inputState) {
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, SpriteBatch? spriteBatch, IReadonlyQuadTree<IRenderableEntity> renderTree) {
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
    }

    /// <inheritdoc />
    public void ReorderChild(IEntity entity, int newIndex) {
    }

    /// <inheritdoc />
    public void ReorderSystem(IGameSystem system, int newIndex) {
    }

    /// <inheritdoc />
    public void SetPercentageComplete(float amount) {
    }

    /// <inheritdoc />
    public void SetWorldPosition(Vector2 position) {
    }

    /// <inheritdoc />
    public void Stop() {
    }

    /// <inheritdoc cref="IQueueableSpriteAnimator.Stop(bool)" />
    public void Stop(bool eraseQueue) {
    }

    /// <inheritdoc />
    public void Swap(SpriteAnimation animation) {
    }

    /// <inheritdoc />
    public void Swap(SpriteAnimationReference animationReference) {
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