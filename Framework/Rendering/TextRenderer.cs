namespace Macabresoft.Macabre2D.Framework;

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
public class TextRenderer : RenderableEntity, IRotatable {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly ResettableLazy<Transform> _pixelTransform;
    private readonly ResettableLazy<Transform> _rotatableTransform;
    private Color _color = Color.Black;
    private Rotation _rotation;
    private bool _snapToPixels;
    private string _text = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRenderer" /> class.
    /// </summary>
    public TextRenderer() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
        this._pixelTransform = new ResettableLazy<Transform>(this.CreatePixelTransform);
        this._rotatableTransform = new ResettableLazy<Transform>(this.CreateRotatableTransform);
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
    public Color Color {
        get => this._color;
        set => this.Set(ref this._color, value);
    }

    /// <summary>
    /// Gets the render settings.
    /// </summary>
    /// <value>The render settings.</value>
    [DataMember(Order = 4, Name = "Render Settings")]
    public RenderSettings RenderSettings { get; private set; } = new();

    /// <inheritdoc />
    [DataMember(Order = 5)]
    [Category(CommonCategories.Transform)]
    public Rotation Rotation {
        get => this.ShouldSnapToPixels(this.Settings) ? Rotation.Zero : this._rotation;
        set {
            if (this.Set(ref this._rotation, value) && !this.ShouldSnapToPixels(this.Settings)) {
                this._boundingArea.Reset();
                this._rotatableTransform.Reset();
            }
        }
    }

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
            if (this.Set(ref this._snapToPixels, value)) {
                if (!this._snapToPixels) {
                    this._rotatableTransform.Reset();
                }
                else {
                    this._pixelTransform.Reset();
                }

                this._boundingArea.Reset();
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
            if (this.Set(ref this._text, value)) {
                this._boundingArea.Reset();
                this.RenderSettings.InvalidateSize();
            }
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);

        this.FontReference.Initialize(this.Scene.Assets);
        this.RenderSettings.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.RenderSettings.Initialize(this.CreateSize);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (!string.IsNullOrEmpty(this.Text) && this.FontReference.Asset is { } font && this.SpriteBatch is { } spriteBatch) {
            spriteBatch.Draw(
                this.Settings.PixelsPerUnit,
                font,
                this.Text,
                this.SnapToPixels ? this._pixelTransform.Value : this._rotatableTransform.Value,
                this.Color,
                this.RenderSettings.Orientation);
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(IEntity.Transform)) {
            this.Reset();
        }
    }

    private BoundingArea CreateBoundingArea() {
        BoundingArea result;
        if (this.LocalScale.X != 0f && this.LocalScale.Y != 0f) {
            var inversePixelsPerUnit = this.Settings.InversePixelsPerUnit;
            var (x, y) = this.RenderSettings.Size;
            var width = x * inversePixelsPerUnit;
            var height = y * inversePixelsPerUnit;
            var offset = this.RenderSettings.Offset * inversePixelsPerUnit;
            var points = new List<Vector2> {
                this.GetWorldTransform(offset, this.Rotation).Position,
                this.GetWorldTransform(offset + new Vector2(width, 0f), this.Rotation).Position,
                this.GetWorldTransform(offset + new Vector2(width, height), this.Rotation).Position,
                this.GetWorldTransform(offset + new Vector2(0f, height), this.Rotation).Position
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

            result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
        }
        else {
            result = new BoundingArea();
        }

        return result;
    }

    private Transform CreatePixelTransform() {
        var worldTransform = this.GetWorldTransform(this.RenderSettings.Offset * this.Settings.InversePixelsPerUnit);
        return worldTransform.ToPixelSnappedValue(this.Settings);
    }

    private Transform CreateRotatableTransform() {
        return this.GetWorldTransform(this.RenderSettings.Offset * this.Settings.InversePixelsPerUnit, this.Rotation);
    }

    private Vector2 CreateSize() {
        return this.FontReference.Asset?.Content?.MeasureString(this.Text) ?? Vector2.Zero;
    }

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderSettings.Offset)) {
            this.Reset();
        }
    }

    private void Reset() {
        this._pixelTransform.Reset();
        this._rotatableTransform.Reset();
        this._boundingArea.Reset();
    }
}