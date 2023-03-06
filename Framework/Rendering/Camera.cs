namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for a camera which tells the engine where to render any <see cref="IRenderableEntity" />.
/// </summary>
public interface ICamera : IEntity, IBoundable, IPixelSnappable {
    /// <summary>
    /// Gets the actual view height.
    /// </summary>
    float ActualViewHeight { get; }

    /// <summary>
    /// Gets the layers to exclude from renders.
    /// </summary>
    /// <remarks>
    /// An entity will be excluded from the render if it includes any layer specified here,
    /// regardless of whether or not one of the other layers it has it meant to be rendered.
    /// </remarks>
    Layers LayersToExcludeFromRender { get; }

    /// <summary>
    /// Gets the layers to render.
    /// </summary>
    /// <value>The layers to render.</value>
    Layers LayersToRender { get; }

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
    /// Gets the view width.
    /// </summary>
    float ViewWidth { get; }

    /// <summary>
    /// Gets or sets the view height of the camera in world units (not screen pixels).
    /// </summary>
    float ViewHeight { get; set; }

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
[Display(Name = "Camera")]
public class Camera : Entity, ICamera {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly ResettableLazy<float> _viewWidth;
    private bool _overrideCommonViewHeight = true;
    private int _renderOrder;
    private SamplerState _samplerState = SamplerState.PointClamp;
    private SamplerStateType _samplerStateType = SamplerStateType.PointClamp;
    private float _viewHeight = 10f;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="Camera" /> class.
    /// </summary>
    public Camera() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
        this._viewWidth = new ResettableLazy<float>(this.CreateViewWidth);
    }

    /// <inheritdoc />
    public BoundingArea BoundingArea => this._boundingArea.Value;

    /// <inheritdoc />
    [DataMember(Name = "Offset Options")]
    public OffsetOptions OffsetOptions { get; } = new(Vector2.Zero, PixelOffsetType.Center);

    /// <summary>
    /// Gets the shader reference.
    /// </summary>
    [DataMember(Name = CommonCategories.Shader)]
    [Category(CommonCategories.Shader)]
    public ShaderReference ShaderReference { get; } = new();

    /// <inheritdoc />
    public float ViewWidth => this._viewWidth.Value;

    /// <summary>
    /// Gets the actual view height for this camera. Will only be different than <see cref="ViewHeight" /> if pixel snapping is enabled.
    /// </summary>
    public float ActualViewHeight { get; private set; }

    /// <inheritdoc />
    [DataMember(Name = "Layers to Exclude")]
    public Layers LayersToExcludeFromRender { get; set; } = Layers.None;

    /// <inheritdoc />
    [DataMember(Name = "Layers to Render")]
    public Layers LayersToRender { get; set; } = ~Layers.None;

    /// <summary>
    /// Gets or sets a value indicating whether or not to override the common view height from <see cref="IGameSettings" />.
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
    [Category(CommonCategories.Rendering)]
    public PixelSnap PixelSnap { get; set; } = PixelSnap.Inherit;

    /// <inheritdoc />
    [DataMember]
    public int RenderOrder {
        get => this._renderOrder;
        set => this.Set(ref this._renderOrder, value);
    }

    /// <summary>
    /// Gets or sets the type of the sampler state.
    /// </summary>
    /// <value>The type of the sampler state.</value>
    [DataMember(Name = "Sampler State")]
    public SamplerStateType SamplerStateType {
        get => this._samplerStateType;
        set {
            this._samplerStateType = value;
            this._samplerState = this._samplerStateType.ToSamplerState();
            this.RaisePropertyChanged(nameof(this._samplerState));
        }
    }

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
        }
    }

    /// <summary>
    /// Gets the sampler state.
    /// </summary>
    protected SamplerState SamplerState => this._samplerState;

    /// <inheritdoc />
    public Vector2 ConvertPointFromScreenSpaceToWorldSpace(Point point) {
        var result = Vector2.Zero;

        if (this.OffsetOptions.Size.Y != 0f) {
            var ratio = this.ActualViewHeight / this.OffsetOptions.Size.Y;
            var pointVector = point.ToVector2();
            var vectorPosition = new Vector2(pointVector.X + this.OffsetOptions.Offset.X, -pointVector.Y + this.OffsetOptions.Size.Y + this.OffsetOptions.Offset.Y) * ratio;
            result = this.GetWorldPosition(vectorPosition);
        }

        return result;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.OffsetOptions.Initialize(this.CreateSize);
        this.OnScreenAreaChanged();
        this._samplerState = this._samplerStateType.ToSamplerState();

        this.Game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
        this.OffsetOptions.PropertyChanged += this.OffsetSettings_PropertyChanged;
        this.ShaderReference.Initialize(this.Scene.Assets);
    }

    /// <inheritdoc />
    public virtual void Render(FrameTime frameTime, SpriteBatch? spriteBatch, IReadonlyQuadTree<IRenderableEntity> renderTree) {
        this.Render(frameTime, spriteBatch, renderTree, this.BoundingArea, this.GetViewMatrix(), this.LayersToRender, this.LayersToExcludeFromRender);
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
    public void ZoomTo(IBoundable boundable) {
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
    /// Gets the view matrix.
    /// </summary>
    /// <returns>The view matrix.</returns>
    protected Matrix GetViewMatrix() {
        return this.GetViewMatrix(this.WorldPosition);
    }

    /// <summary>
    /// Gets the view matrix.
    /// </summary>
    /// <param name="position">The position to use.</param>
    /// <returns>The view matrix.</returns>
    protected Matrix GetViewMatrix(Vector2 position) {
        var settings = this.Settings;
        var pixelsPerUnit = settings.PixelsPerUnit;
        var zoom = 1f / settings.GetPixelAgnosticRatio(this.ActualViewHeight, (int)this.OffsetOptions.Size.Y);

        var matrix =
            Matrix.CreateTranslation(new Vector3(-position.ToPixelSnappedValue(this.Settings) * pixelsPerUnit, 0f)) *
            Matrix.CreateScale(zoom, -zoom, 0f) *
            Matrix.CreateTranslation(new Vector3(-this.OffsetOptions.Offset.X, this.OffsetOptions.Size.Y + this.OffsetOptions.Offset.Y, 0f));

        return matrix;
    }

    /// <summary>
    /// Called when the screen area changes.
    /// This could be the resolution changing, the transform of this entity changing, the offset options changing, or snap to pixels changing.
    /// This will also be called during initialization.
    /// </summary>
    protected virtual void OnScreenAreaChanged() {
        this._boundingArea.Reset();
        this._viewWidth.Reset();
        this.CalculateActualViewHeight();
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
    protected virtual void Render(
        FrameTime frameTime,
        SpriteBatch? spriteBatch,
        IReadonlyQuadTree<IRenderableEntity> renderTree,
        BoundingArea viewBoundingArea,
        Matrix viewMatrix,
        Layers layersToRender,
        Layers layersToExclude) {
        var enabledLayers = this.Settings.LayerSettings.EnabledLayers;
        var entities = renderTree
            .RetrievePotentialCollisions(viewBoundingArea)
            .Where(x => (x.Layers & layersToExclude) == Layers.None && (x.Layers & layersToRender & enabledLayers) != Layers.None)
            .OrderBy(x => x.RenderOrder)
            .ToList();

        if (entities.Any()) {
            spriteBatch?.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                this._samplerState,
                null,
                RasterizerState.CullNone,
                this.ShaderReference.Asset?.Content,
                viewMatrix);

            foreach (var entity in entities) {
                entity.Render(frameTime, viewBoundingArea);
            }

            spriteBatch?.End();
        }
    }

    private void CalculateActualViewHeight() {
        var viewHeight = this.OverrideCommonViewHeight ? this.ViewHeight : this.Settings.CommonViewHeight;

        if (this.ShouldSnapToPixels(this.Settings)) {
            var originalPixelHeight = (uint)Math.Round(viewHeight * this.Settings.PixelsPerUnit, MidpointRounding.AwayFromZero);
            var viewportHeight = this.Game.ViewportSize.Y;
            if (originalPixelHeight < viewportHeight) {
                var internalPixelHeight = originalPixelHeight;
                var count = 0;

                while (internalPixelHeight < viewportHeight) {
                    internalPixelHeight += originalPixelHeight;
                    count++;
                }

                this.ActualViewHeight = this.Settings.UnitsPerPixel * (viewportHeight / (float)count);
            }
            else {
                this.ActualViewHeight = viewHeight;
            }
        }
        else {
            this.ActualViewHeight = viewHeight;
        }

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

        if (this.ShouldSnapToPixels(this.Settings)) {
            minimumX = minimumX.ToPixelSnappedValue(this.Settings);
            minimumY = minimumY.ToPixelSnappedValue(this.Settings);
            maximumX = maximumX.ToPixelSnappedValue(this.Settings);
            maximumY = maximumY.ToPixelSnappedValue(this.Settings);
        }

        return new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
    }

    private Vector2 CreateSize() {
        return new Vector2(this.Game.ViewportSize.X, this.Game.ViewportSize.Y);
    }

    private float CreateViewWidth() {
        var (x, y) = this.OffsetOptions.Size;
        var ratio = y != 0 ? this.ActualViewHeight / y : 0f;
        return x * ratio;
    }

    private void Game_ViewportSizeChanged(object? sender, Point e) {
        this.OffsetOptions.InvalidateSize();
        this.CalculateActualViewHeight();
        this.OnScreenAreaChanged();
        this._viewWidth.Reset();
    }

    private void OffsetSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.OffsetOptions.Offset)) {
            this.OnScreenAreaChanged();
        }
    }
}