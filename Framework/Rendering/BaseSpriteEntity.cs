namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for an entity that handles rendering from a single sprite sheet.
/// </summary>
public interface ISpriteEntity : IRenderableEntity {

    /// <summary>
    /// Gets or sets the render options.
    /// </summary>
    public RenderOptions RenderOptions { get; }
}

/// <summary>
/// An abstract base entity that renders a single sprite, given a sprite sheet and a sprite index.
/// </summary>
[Category(CommonCategories.Rendering)]
public abstract class BaseSpriteEntity : RenderableEntity, ISpriteEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly ResettableLazy<Vector2> _offsetTransform;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSpriteEntity" /> class.
    /// </summary>
    protected BaseSpriteEntity() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
        this._offsetTransform = new ResettableLazy<Vector2>(this.CreateOffsetTransform);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <inheritdoc />
    [DataMember(Order = 4, Name = "Render Options")]
    public RenderOptions RenderOptions { get; private set; } = new();

    /// <summary>
    /// Gets the sprite index.
    /// </summary>
    public abstract byte? SpriteIndex { get; }

    /// <summary>
    /// Gets the sprite sheet.
    /// </summary>
    protected abstract SpriteSheet? SpriteSheet { get; }

    /// <inheritdoc />
    public override RenderPriority RenderPriority {
        get {
            if (this.RenderPriorityOverride.IsEnabled) {
                return this.RenderPriorityOverride.Value;
            }

            return this.SpriteSheet?.DefaultRenderPriority ?? default;
        }
    }

    /// <summary>
    /// Gets a render priority override.
    /// </summary>
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public RenderPriorityOverride RenderPriorityOverride { get; } = new();

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        this.RenderOptions.PropertyChanged -= this.RenderSettings_PropertyChanged;

        base.Initialize(scene, parent);

        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.RenderOptions.Initialize(this.CreateSize);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.RenderOptions.Color);
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
    /// Creates the size of this sprite in pixels for the <see cref="RenderOptions" />.
    /// </summary>
    /// <returns></returns>
    protected virtual Vector2 CreateSize() {
        var result = Vector2.Zero;
        if (this.SpriteSheet is { } spriteSheet) {
            return new Vector2(spriteSheet.SpriteSize.X, spriteSheet.SpriteSize.Y);
        }

        return result;
    }

    /// <summary>
    /// Gets the appropriate transform for rendering.
    /// </summary>
    /// <returns>The render transform.</returns>
    protected Vector2 GetRenderTransform() => this._offsetTransform.Value;

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

    private BoundingArea CreateBoundingArea() => this.RenderOptions.CreateBoundingArea(this);

    private Vector2 CreateOffsetTransform() => this.GetWorldPosition(this.RenderOptions.Offset * this.Project.UnitsPerPixel);

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderOptions.Offset)) {
            this.ResetTransforms();
        }
    }

    private void ResetTransforms() {
        this._boundingArea.Reset();
        this._offsetTransform.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}