namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// An abstract base entity that renders a single sprite, given a sprite sheet and a sprite index.
/// </summary>
[Category(CommonCategories.Rendering)]
public abstract class BaseSpriteEntity : RenderableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly ResettableLazy<Vector2> _pixelTransform;
    private Color _color = Color.White;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseSpriteEntity" /> class.
    /// </summary>
    protected BaseSpriteEntity() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
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
    public Color Color {
        get => this._color;
        set => this.Set(ref this._color, value);
    }

    /// <summary>
    /// Gets or sets the render options.
    /// </summary>
    /// <value>The render options.</value>
    [DataMember(Order = 4, Name = "Render Options")]
    public RenderOptions RenderOptions { get; private set; } = new();

    /// <summary>
    /// Gets the sprite sheet.
    /// </summary>
    protected abstract SpriteSheetAsset? SpriteSheet { get; }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.RenderOptions.Initialize(this.CreateSize);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.SpriteIndex.HasValue && this.SpriteBatch is { } spriteBatch && this.SpriteSheet is { } spriteSheet) {
            spriteSheet.Draw(
                spriteBatch,
                this.Settings.PixelsPerUnit,
                this.SpriteIndex.Value,
                this.GetRenderTransform(),
                this.Color,
                this.RenderOptions.Orientation);
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(this.WorldPosition)) {
            this.Reset();
        }
        else if (e.PropertyName == nameof(IEntity.IsEnabled) && this.IsEnabled) {
            this.RaisePropertyChanged(nameof(this.IsVisible));
        }
    }

    /// <summary>
    /// Resets the render options, bounding area, and render transform.
    /// </summary>
    protected void Reset() {
        this.RenderOptions.InvalidateSize();
        this._boundingArea.Reset();
        this._pixelTransform.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private BoundingArea CreateBoundingArea() {
        BoundingArea result;
        if (this.SpriteSheet is { } spriteSheet) {
            var inversePixelsPerUnit = this.Settings.UnitsPerPixel;
            var width = spriteSheet.SpriteSize.X * inversePixelsPerUnit;
            var height = spriteSheet.SpriteSize.Y * inversePixelsPerUnit;
            var offset = this.RenderOptions.Offset * inversePixelsPerUnit;

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

            result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
        }
        else {
            result = new BoundingArea();
        }

        return result;
    }

    private Vector2 CreatePixelPosition() {
        return this.GetWorldPosition(this.RenderOptions.Offset * this.Settings.UnitsPerPixel).ToPixelSnappedValue(this.Settings);
    }

    private Vector2 CreateSize() {
        var result = Vector2.Zero;
        if (this.SpriteSheet is { } spriteSheet) {
            return new Vector2(spriteSheet.SpriteSize.X, spriteSheet.SpriteSize.Y);
        }

        return result;
    }

    /// <summary>
    /// Gets the appropriate transform for rendering.
    /// </summary>
    /// <returns></returns>
    protected Vector2 GetRenderTransform() {
        return this.ShouldSnapToPixels(this.Settings) ? this._pixelTransform.Value : this.WorldPosition;
    }

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderOptions.Offset)) {
            this._pixelTransform.Reset();
            this._boundingArea.Reset();
        }
    }
}