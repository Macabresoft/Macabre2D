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
/// An entity which will render the specified text.
/// </summary>
[Display(Name = "Text Renderer")]
public class TextRenderer : RenderableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly ResettableLazy<Vector2> _pixelPosition;
    private bool _snapToPixels;
    private string _text = string.Empty;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRenderer" /> class.
    /// </summary>
    public TextRenderer() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
        this._pixelPosition = new ResettableLazy<Vector2>(this.CreatePixelPosition);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets the font reference.
    /// </summary>
    [DataMember(Order = 0)]
    public AssetReference<FontAsset, SpriteFont> FontReference { get; } = new();

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    [DataMember(Order = 1)]
    public Color Color { get; set; } = Color.Black;

    /// <summary>
    /// Gets the render options.
    /// </summary>
    /// <value>The render options.</value>
    [DataMember(Order = 4, Name = "Render Options")]
    public RenderOptions RenderOptions { get; private set; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether this text renderer should snap to the pixel
    /// ratio defined in <see cref="IGameSettings" />.
    /// </summary>
    /// <remarks>Snapping to pixels will disable rotations on this renderer.</remarks>
    /// <value><c>true</c> if this should snap to pixels; otherwise, <c>false</c>.</value>
    [DataMember(Order = 3)]
    public bool SnapToPixels {
        get => this._snapToPixels;
        set {
            if (value != this._snapToPixels) {
                this._snapToPixels = value;
                if (this._snapToPixels) {
                    this._pixelPosition.Reset();
                }

                this.ResetBoundingArea();
            }
        }
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    [DataMember(Order = 2)]
    public string Text {
        get => this._text;
        set {
            if (value != this._text) {
                this._text = value;
                this.ResetBoundingArea();
                this.RenderOptions.InvalidateSize();
            }
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);

        this.FontReference.Initialize(this.Scene.Assets);
        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.RenderOptions.Initialize(this.CreateSize);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (!string.IsNullOrEmpty(this.Text) && this.FontReference.Asset is { } font && this.SpriteBatch is { } spriteBatch) {
            spriteBatch.Draw(
                this.Settings.PixelsPerUnit,
                font,
                this.Text,
                this.SnapToPixels ? this._pixelPosition.Value : this.WorldPosition,
                this.Color,
                this.RenderOptions.Orientation);
        }
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.Reset();
    }

    private BoundingArea CreateBoundingArea() {
        var inversePixelsPerUnit = this.Settings.UnitsPerPixel;
        var (x, y) = this.RenderOptions.Size;
        var width = x * inversePixelsPerUnit;
        var height = y * inversePixelsPerUnit;
        var offset = this.RenderOptions.Offset * inversePixelsPerUnit;
        var points = new List<Vector2> {
            this.GetWorldPosition(offset),
            this.GetWorldPosition(offset + new Vector2(width, 0f)),
            this.GetWorldPosition(offset + new Vector2(width, height)),
            this.GetWorldPosition(offset + new Vector2(0f, height))
        };

        var minimumX = points.Min(point => point.X);
        var minimumY = points.Min(point => point.Y);
        var maximumX = points.Max(point => point.X);
        var maximumY = points.Max(point => point.Y);

        if (this.SnapToPixels) {
            minimumX = minimumX.ToPixelSnappedValue(this.Settings);
            minimumY = minimumY.ToPixelSnappedValue(this.Settings);
            maximumX = maximumX.ToPixelSnappedValue(this.Settings);
            maximumY = maximumY.ToPixelSnappedValue(this.Settings);
        }

        return new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
    }

    private Vector2 CreatePixelPosition() {
        return this.WorldPosition.ToPixelSnappedValue(this.Settings);
    }

    private Vector2 CreateSize() {
        return this.FontReference.Asset?.Content?.MeasureString(this.Text) ?? Vector2.Zero;
    }

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderOptions.Offset)) {
            this.Reset();
        }
    }

    private void Reset() {
        this._pixelPosition.Reset();
        this.ResetBoundingArea();
    }

    private void ResetBoundingArea() {
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}