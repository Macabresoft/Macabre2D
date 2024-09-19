namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// An abstract base entity that renders a single sprite, given a sprite sheet and a sprite index.
/// </summary>
[Category(CommonCategories.Rendering)]
public abstract class BaseSpriteEntity : RenderableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly ResettableLazy<Vector2> _offsetTransform;
    private readonly ResettableLazy<Vector2> _pixelTransform;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSpriteEntity" /> class.
    /// </summary>
    protected BaseSpriteEntity() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
        this._offsetTransform = new ResettableLazy<Vector2>(this.CreateOffsetTransform);
        this._pixelTransform = new ResettableLazy<Vector2>(this.CreatePixelPosition);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets the sprite index.
    /// </summary>
    public abstract byte? SpriteIndex { get; }

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    [DataMember(Order = 1)]
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets the render options.
    /// </summary>
    /// <value>The render options.</value>
    [DataMember(Order = 4, Name = "Render Options")]
    public RenderOptions RenderOptions { get; private set; } = new();

    /// <summary>
    /// Gets the sprite sheet.
    /// </summary>
    protected abstract SpriteSheet? SpriteSheet { get; }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        this.RenderOptions.PropertyChanged -= this.RenderSettings_PropertyChanged;

        base.Initialize(scene, parent);

        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.RenderOptions.Initialize(this.CreateSize);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this.SpriteIndex.HasValue && this.SpriteBatch is { } spriteBatch && this.SpriteSheet is { } spriteSheet) {
            spriteSheet.Draw(
                spriteBatch,
                this.Project.PixelsPerUnit,
                this.SpriteIndex.Value,
                this.GetRenderTransform(),
                colorOverride,
                this.RenderOptions.Orientation);
        }
    }

    /// <summary>
    /// Gets the appropriate transform for rendering.
    /// </summary>
    /// <returns></returns>
    protected Vector2 GetRenderTransform() {
        return this.ShouldSnapToPixels(this.Project) ? this._pixelTransform.Value : this._offsetTransform.Value;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(IEntity.IsEnabled) && this.IsEnabled) {
            this.RaisePropertyChanged(nameof(this.ShouldRender));
        }
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.Reset();
    }

    /// <summary>
    /// Resets the render options, bounding area, and render transform.
    /// </summary>
    protected void Reset() {
        this.RenderOptions.InvalidateSize();
        this.ResetTransforms();
    }

    private BoundingArea CreateBoundingArea() {
        return this.RenderOptions.CreateBoundingArea(this);
    }

    private Vector2 CreateOffsetTransform() {
        return this.GetWorldPosition(this.RenderOptions.Offset * this.Project.UnitsPerPixel);
    }

    private Vector2 CreatePixelPosition() {
        return this._offsetTransform.Value.ToPixelSnappedValue(this.Project);
    }

    private Vector2 CreateSize() {
        var result = Vector2.Zero;
        if (this.SpriteSheet is { } spriteSheet) {
            return new Vector2(spriteSheet.SpriteSize.X, spriteSheet.SpriteSize.Y);
        }

        return result;
    }

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderOptions.Offset)) {
            this.ResetTransforms();
        }
    }

    private void ResetTransforms() {
        this._boundingArea.Reset();
        this._offsetTransform.Reset();
        this._pixelTransform.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}