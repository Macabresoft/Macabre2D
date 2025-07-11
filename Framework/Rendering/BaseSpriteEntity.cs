namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

    /// <inheritdoc />
    public override RenderPriority RenderPriority {
        get {
            if (this.RenderPriorityOverride.IsEnabled) {
                return this.RenderPriorityOverride.Value;
            }

            return this.SpriteSheet?.DefaultRenderPriority ?? default;
        }

        set {
            this.RenderPriorityOverride.IsEnabled = true;
            this.RenderPriorityOverride.Value = value;
        }
    }

    /// <summary>
    /// Gets a render priority override.
    /// </summary>
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public RenderPriorityOverride RenderPriorityOverride { get; } = new();

    /// <summary>
    /// Gets the sprite index.
    /// </summary>
    public abstract byte? SpriteIndex { get; }

    /// <summary>
    /// Gets the sprite sheet.
    /// </summary>
    protected abstract SpriteSheet? SpriteSheet { get; }

    /// <inheritdoc />
    public override void Deinitialize() {
        this.RenderOptions.PropertyChanged -= this.RenderSettings_PropertyChanged;
        base.Deinitialize();
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
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
            this.RenderAtPosition(spriteBatch, spriteSheet, this.SpriteIndex.Value, this.GetRenderTransform(), colorOverride);
        }
    }

    /// <summary>
    /// Creates the size of this sprite in pixels for the <see cref="RenderOptions" />.
    /// </summary>
    /// <returns>Creates the size.</returns>
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
    /// Renders the specified sprite at the specified location.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="position">The position.</param>
    /// <param name="colorOverride">The color override.</param>
    protected void RenderAtPosition(SpriteBatch spriteBatch, SpriteSheet spriteSheet, byte spriteIndex, Vector2 position, Color colorOverride) {
        spriteSheet.Draw(
            spriteBatch,
            this.Project.PixelsPerUnit,
            spriteIndex,
            position,
            colorOverride,
            this.RenderOptions.Orientation);
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