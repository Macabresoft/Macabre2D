namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for a camera which tells the engine where to render any <see cref="IRenderableEntity" />.
/// </summary>
public interface ICamera : IBoundableEntity {
    /// <summary>
    /// Called when the <see cref="RenderOrder" /> changes.
    /// </summary>
    event EventHandler? RenderOrderChanged;

    /// <summary>
    /// Gets the actual view height.
    /// </summary>
    float ActualViewHeight { get; }

    /// <summary>
    /// Gets the layers to exclude from renders.
    /// </summary>
    /// <remarks>
    /// An entity will be excluded from the render if it includes any layer specified here,
    /// regardless of whether one of the other layers it has it meant to be rendered.
    /// </remarks>
    Layers LayersToExcludeFromRender { get; }

    /// <summary>
    /// Gets the layers to render.
    /// </summary>
    /// <value>The layers to render.</value>
    Layers LayersToRender { get; set; }

    /// <summary>
    /// Gets the offset options.
    /// </summary>
    /// <value>The offset options.</value>
    OffsetOptions OffsetOptions { get; }

    /// <summary>
    /// Gets the render order.
    /// </summary>
    /// <value>The render order.</value>
    int RenderOrder => 0;

    /// <summary>
    /// Gets the safe area to render objects in this camera. By default, it does not differ from the <see cref="BoundingArea" />.
    /// </summary>
    BoundingArea SafeArea { get; }

    /// <summary>
    /// Gets or sets the view height of the camera in world units (not screen pixels).
    /// </summary>
    float ViewHeight { get; set; }

    /// <summary>
    /// Gets the view width.
    /// </summary>
    float ViewWidth { get; }

    /// <summary>
    /// Converts the point from screen space to world space.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The world space location of the point.</returns>
    Vector2 ConvertPointFromScreenSpaceToWorldSpace(Point point);

    /// <summary>
    /// Renders the provided entities.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="spriteBatch">The sprite batch to use while rendering.</param>
    /// <param name="renderTree">The render tree.</param>
    void Render(FrameTime frameTime, SpriteBatch? spriteBatch, IReadonlyQuadTree<IRenderableEntity> renderTree);
}

/// <summary>
/// Represents a camera into the game world.
/// </summary>
public class Camera : Entity, ICamera {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly ResettableLazy<float> _viewWidth;
    private bool _overrideCommonViewHeight = true;
    private int _renderOrder;
    private float _viewHeight = 10f;
    private float _zoom;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? RenderOrderChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="Camera" /> class.
    /// </summary>
    public Camera() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
        this._viewWidth = new ResettableLazy<float>(this.CreateViewWidth);
    }

    /// <summary>
    /// Gets the actual view height for this camera. Will only be different from <see cref="ViewHeight" /> if pixel snapping is enabled.
    /// </summary>
    public float ActualViewHeight { get; private set; }

    /// <inheritdoc />
    public BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets the color override.
    /// </summary>
    [DataMember]
    public ColorOverride ColorOverride { get; } = new();

    /// <inheritdoc />
    [DataMember(Name = "Layers to Exclude")]
    public Layers LayersToExcludeFromRender { get; set; } = Layers.None;

    /// <inheritdoc />
    [DataMember(Name = "Layers to Render")]
    public Layers LayersToRender { get; set; } = LayersHelpers.GetAll();

    /// <inheritdoc />
    [DataMember(Name = "Offset Options")]
    public OffsetOptions OffsetOptions { get; } = new(Vector2.Zero, PixelOffsetType.Center);

    /// <summary>
    /// Gets or sets a value indicating whether to override the common view height from <see cref="IGameProject" />.
    /// </summary>
    [DataMember(Name = "Override Common View Height", Order = 10)]
    public bool OverrideCommonViewHeight {
        get => this._overrideCommonViewHeight;
        set {
            if (value != this._overrideCommonViewHeight) {
                this._overrideCommonViewHeight = value;
                this.CalculateActualViewHeight();
                this.OnScreenAreaChanged();
            }
        }
    }

    /// <inheritdoc />
    [DataMember]
    public int RenderOrder {
        get => this._renderOrder;
        set {
            if (this.Set(ref this._renderOrder, value)) {
                this.RenderOrderChanged.SafeInvoke(this);
            }
        }
    }

    /// <inheritdoc />
    public virtual BoundingArea SafeArea => this.BoundingArea;

    /// <summary>
    /// Gets or sets the type of the sampler state.
    /// </summary>
    /// <value>The type of the sampler state.</value>
    [DataMember(Name = "Sampler State")]
    public SamplerStateType Sampler { get; set; } = SamplerStateType.PointClamp;

    /// <summary>
    /// Gets the shader reference.
    /// </summary>
    [DataMember(Name = CommonCategories.Shader)]
    [Category(CommonCategories.Shader)]
    public ShaderReference ShaderReference { get; } = new();

    /// <summary>
    /// Gets or sets the height of the view.
    /// </summary>
    [DataMember(Name = "View Height", Order = 11)]
    public float ViewHeight {
        get => this._viewHeight;
        set {
            // This is kind of an arbitrary value, but imagine the chaos if we allowed the
            // camera to reach 0.
            if (value <= 0.1f) {
                value = 0.1f;
            }

            this._viewHeight = value;
            this.CalculateActualViewHeight();
            this.OnScreenAreaChanged();
            this.ResetZoom();
        }
    }

    /// <inheritdoc />
    public float ViewWidth => this._viewWidth.Value;

    /// <inheritdoc />
    public Vector2 ConvertPointFromScreenSpaceToWorldSpace(Point point) {
        var result = Vector2.Zero;

        if (this.OffsetOptions.Size.Y > 0f && this.Game.ViewportSize.Y > 0f) {
            var cameraRatio = this.ActualViewHeight / this.OffsetOptions.Size.Y;
            var screenSpaceRatio = this.ActualViewHeight / this.Game.ViewportSize.Y;
            var pointVector = point.ToVector2() * screenSpaceRatio;
            var vectorPosition = new Vector2(pointVector.X + this.OffsetOptions.Offset.X * cameraRatio, -pointVector.Y + (this.OffsetOptions.Size.Y + this.OffsetOptions.Offset.Y) * cameraRatio);
            result = this.GetWorldPosition(vectorPosition);
        }

        return result;
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.Game.ViewportSizeChanged -= this.Game_ViewportSizeChanged;
        this.OffsetOptions.PropertyChanged -= this.OffsetSettings_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.OffsetOptions.Initialize(this.CreateSize);
        this.OnScreenAreaChanged();
        this.ResetZoom();

        this.Game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
        this.OffsetOptions.PropertyChanged += this.OffsetSettings_PropertyChanged;
        this.ResetZoom();
    }

    /// <inheritdoc />
    public virtual void Render(FrameTime frameTime, SpriteBatch? spriteBatch, IReadonlyQuadTree<IRenderableEntity> renderTree) {
        this.Render(frameTime, spriteBatch, renderTree, this.BoundingArea, this.GetViewMatrix(), this.LayersToRender, this.LayersToExcludeFromRender, this.ColorOverride, this.ShaderReference.PrepareAndGetShader(this.Game.ViewportSize.ToVector2(), this.Game, this.Scene));
    }

    /// <summary>
    /// Zooms to a world position.
    /// </summary>
    /// <param name="worldPosition">The world position.</param>
    /// <param name="zoomAmount">The zoom amount.</param>
    public void ZoomTo(Vector2 worldPosition, float zoomAmount) {
        if (this.OverrideCommonViewHeight) {
            var originalCameraPosition = this.WorldPosition;
            var originalDistanceFromCamera = worldPosition - originalCameraPosition;
            var originalViewHeight = this.ActualViewHeight;
            this.ViewHeight -= zoomAmount;
            var viewHeightRatio = this.ActualViewHeight / originalViewHeight;
            this.SetWorldPosition(worldPosition - originalDistanceFromCamera * viewHeightRatio);
        }
        else {
            this.SetWorldPosition(worldPosition);
        }
    }

    /// <summary>
    /// Zooms to a screen position.
    /// </summary>
    /// <param name="screenPosition">The screen position.</param>
    /// <param name="zoomAmount">The zoom amount.</param>
    public void ZoomTo(Point screenPosition, float zoomAmount) {
        var worldPosition = this.ConvertPointFromScreenSpaceToWorldSpace(screenPosition);
        this.ZoomTo(worldPosition, zoomAmount);
    }

    /// <summary>
    /// Zooms to a boundable entity, fitting it into frame.
    /// </summary>
    /// <param name="boundable">The boundable.</param>
    public void ZoomTo(IBoundableEntity boundable) {
        var area = boundable.BoundingArea;
        var difference = area.Maximum - area.Minimum;
        var origin = area.Minimum + difference * 0.5f;

        this.SetWorldPosition(origin);

        this.ActualViewHeight = difference.Y;
        if (this.ActualViewHeight < difference.X && this.ViewWidth != 0f) {
            this.ActualViewHeight *= difference.X / this.ViewWidth;
        }
    }

    /// <summary>
    /// Creates the pixel size of this camera's viewing area.
    /// </summary>
    /// <returns>The pixel size as a <see cref="Vector2" />.</returns>
    protected virtual Vector2 CreateSize() => new(this.Project.InternalRenderResolution.X, this.Project.InternalRenderResolution.Y);

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.ShaderReference;
    }

    /// <summary>
    /// Gets the view matrix.
    /// </summary>
    /// <returns>The view matrix.</returns>
    protected Matrix GetViewMatrix() => this.GetViewMatrix(this.WorldPosition);

    /// <summary>
    /// Gets the view matrix.
    /// </summary>
    /// <param name="position">The position to use.</param>
    /// <returns>The view matrix.</returns>
    protected Matrix GetViewMatrix(Vector2 position) {
        position = this.ToPixelSnappedValue(position);
        var offsetX = this.OffsetOptions.Offset.X;
        var offsetY = this.OffsetOptions.Size.Y + this.OffsetOptions.Offset.Y;

        var matrix =
            Matrix.CreateTranslation(new Vector3(-position * this.Project.PixelsPerUnit, 0f)) *
            Matrix.CreateScale(this._zoom, -this._zoom, 0f) *
            Matrix.CreateTranslation(new Vector3(-offsetX, offsetY, 0f));

        return matrix;
    }

    /// <summary>
    /// Called when the screen area changes.
    /// This could be the resolution changing, the transform of this entity changing, the offset options changing, or snap to pixels changing.
    /// This will also be called during initialization.
    /// </summary>
    protected virtual void OnScreenAreaChanged() {
        this.CalculateActualViewHeight();
        this._boundingArea.Reset();
        this._viewWidth.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.OnScreenAreaChanged();
    }

    /// <summary>
    /// Renders entities in the specified bounding area.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="renderTree">The render tree.</param>
    /// <param name="viewBoundingArea">The view's bounding area.</param>
    /// <param name="viewMatrix">The view matrix.</param>
    /// <param name="layersToRender">The layers to render.</param>
    /// <param name="layersToExclude">The layers to exclude from render.</param>
    /// <param name="colorOverride">The color override.</param>
    /// <param name="shader">The shader.</param>
    protected virtual void Render(
        FrameTime frameTime,
        SpriteBatch? spriteBatch,
        IReadonlyQuadTree<IRenderableEntity> renderTree,
        BoundingArea viewBoundingArea,
        Matrix viewMatrix,
        Layers layersToRender,
        Layers layersToExclude,
        ColorOverride colorOverride,
        Effect? shader) {
        spriteBatch?.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            this.Sampler.ToSamplerState(),
            null,
            RasterizerState.CullNone,
            shader,
            viewMatrix);

        if (colorOverride.IsEnabled) {
            var entities = renderTree
                .RetrievePotentialCollisions(viewBoundingArea)
                .Where(x => x.ShouldRender && (x.Layers & layersToExclude) == Layers.None && (x.Layers & layersToRender) != Layers.None)
                .OrderBy(x => x.RenderPriority)
                .ThenBy(x => x.RenderOrder);

            foreach (var entity in entities) {
                entity.Render(frameTime, viewBoundingArea, colorOverride.Value);
            }
        }
        else {
            var groupings = renderTree
                .RetrievePotentialCollisions(viewBoundingArea)
                .Where(x => x.ShouldRender && (x.Layers & layersToExclude) == Layers.None && (x.Layers & layersToRender) != Layers.None)
                .GroupBy(x => x.RenderPriority)
                .OrderBy(x => x.Key);

            foreach (var group in groupings) {
                var entities = group.OrderBy(x => x.RenderOrder);
                if (this.Game.UserSettings.Colors.TryGetColorForRenderPriority(group.Key, out var color)) {
                    foreach (var entity in entities) {
                        entity.Render(frameTime, viewBoundingArea, color);
                    }
                }
                else {
                    foreach (var entity in entities) {
                        entity.Render(frameTime, viewBoundingArea);
                    }
                }
            }
        }

        spriteBatch?.End();
    }

    private void CalculateActualViewHeight() {
        this.ActualViewHeight = this.OverrideCommonViewHeight ? this.ViewHeight : this.Project.ViewHeight;
        this.RaisePropertyChanged(nameof(this.ActualViewHeight));
    }

    private BoundingArea CreateBoundingArea() {
        var ratio = this.ActualViewHeight / this.OffsetOptions.Size.Y;
        var width = this.OffsetOptions.Size.X * ratio;
        var height = this.OffsetOptions.Size.Y * ratio;
        var offset = this.OffsetOptions.Offset * ratio;

        var points = new List<Vector2> {
            this.GetWorldPosition(offset),
            this.GetWorldPosition(offset + new Vector2(width, 0f)),
            this.GetWorldPosition(offset + new Vector2(width, height)),
            this.GetWorldPosition(offset + new Vector2(0f, height))
        };

        var minimumX = points.Min(x => x.X);
        var minimumY = points.Min(x => x.Y);
        var maximumX = points.Max(x => x.X);
        var maximumY = points.Max(x => x.Y);

        return new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
    }

    private float CreateViewWidth() {
        var (x, y) = this.OffsetOptions.Size;
        var ratio = y != 0 ? this.ActualViewHeight / y : 0f;
        return x * ratio;
    }

    private void Game_ViewportSizeChanged(object? sender, Point e) {
        this.OffsetOptions.InvalidateSize();
        this.OnScreenAreaChanged();
        this.ResetZoom();
    }

    private void OffsetSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.OffsetOptions.Offset)) {
            this.OnScreenAreaChanged();
        }
    }

    private void ResetZoom() {
        if (this.OffsetOptions.Size.Y > 0f) {
            this._zoom = 1f / this.Project.GetPixelAgnosticRatio(this.ActualViewHeight, (int)this.OffsetOptions.Size.Y);
        }
    }

    private Vector2 ToPixelSnappedValue(Vector2 value) =>
        new(
            this.ToPixelSnappedValue(value.X),
            this.ToPixelSnappedValue(value.Y));

    private float ToPixelSnappedValue(float value) => (int)Math.Round(value * this.Project.PixelsPerUnit, 0, MidpointRounding.AwayFromZero) * this.Project.UnitsPerPixel;
}