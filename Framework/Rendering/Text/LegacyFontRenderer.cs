namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabre2D.Project.Common;
using Macabresoft.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// An entity which will render the specified text using a legacy <see cref="SpriteFont" /> from MonoGame.
/// </summary>
public class LegacyFontRenderer : Entity, ILegacyFontRenderer {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private float _actualHeight;
    private float _actualWidth;
    private float _scale;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? ShouldRenderLegacyFontChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyFontRenderer" /> class.
    /// </summary>
    public LegacyFontRenderer() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets the font reference.
    /// </summary>
    [DataMember(Order = 0)]
    public AssetReference<FontAsset, SpriteFont> FontReference { get; } = new();

    /// <summary>
    /// Gets the height override for this font renderer.
    /// </summary>
    [DataMember]
    public FloatOverride HeightOverride { get; } = new();

    /// <summary>
    /// Gets the render options.
    /// </summary>
    /// <value>The render options.</value>
    [DataMember(Order = 4, Name = "Render Options")]
    public RenderOptions RenderOptions { get; private set; } = new();

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    [PredefinedInteger(PredefinedIntegerKind.RenderOrder)]
    public int RenderOrder { get; set; }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public bool RenderOutOfBounds { get; set; }

    /// <inheritdoc cref="ILegacyFontRenderer" />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public RenderPriority RenderPriority { get; set; }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public bool ShouldRenderLegacyFont {
        get => field && this.IsEnabled;
        set {
            if (this.Set(ref field, value)) {
                this.ShouldRenderLegacyFontChanged.SafeInvoke(this);
            }
        }
    } = true;

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    [DataMember(Order = 2)]
    public string Text {
        get;
        set {
            if (value != field) {
                field = value;
                this.ResetBoundingArea();
                this.RenderOptions.InvalidateSize();
            }
        }
    } = string.Empty;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();

        this.RenderOptions.PropertyChanged -= this.RenderSettings_PropertyChanged;
        this.HeightOverride.PropertyChanged -= this.HeightOverride_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);

        this.RenderOptions.Initialize(this.CreateSize);
        this.Reset();

        this.HeightOverride.PropertyChanged += this.HeightOverride_PropertyChanged;
        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
    }

    /// <inheritdoc />
    public void RenderLegacyFont(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.RenderLegacyFont(frameTime, viewBoundingArea, this.RenderOptions.Color);
    }

    /// <inheritdoc />
    public void RenderLegacyFont(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (!string.IsNullOrEmpty(this.Text) && this.FontReference.Asset is { } font && this.SpriteBatch is { } spriteBatch) {
            if (!BaseGame.IsDesignMode) {
                spriteBatch.Draw(
                    this.Game.ScreenPixelsPerUnit,
                    font,
                    this.Text,
                    this.WorldPosition,
                    new Vector2(this._scale),
                    colorOverride,
                    this.RenderOptions.Orientation);
            }
            else {
                spriteBatch.Draw(
                    this.Project.PixelsPerUnit,
                    font,
                    this.Text,
                    this.WorldPosition,
                    colorOverride,
                    this.RenderOptions.Orientation);
            }
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.FontReference;
    }

    /// <inheritdoc />
    protected override void OnIsEnableChanged() {
        base.OnIsEnableChanged();

        if (this.ShouldRenderLegacyFont) {
            this.ShouldRenderLegacyFontChanged.SafeInvoke(this);
        }
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.Reset();
    }

    private BoundingArea CreateBoundingArea() {
        var unitsPerPixel = this.Game.UnitsPerScreenPixel;
        var (x, y) = this.RenderOptions.Size;
        var width = x * unitsPerPixel;
        var height = y * unitsPerPixel;
        var offset = this.RenderOptions.Offset * unitsPerPixel;
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

        return new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
    }

    private Vector2 CreateSize() => new(this._actualWidth, this._actualHeight);

    private void HeightOverride_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.Reset();
    }

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderOptions.Offset)) {
            this.Reset();
        }
    }

    private void Reset() {
        this.ResetScale();
        this.RenderOptions.InvalidateSize();
        this.ResetBoundingArea();
    }

    private void ResetBoundingArea() {
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private void ResetScale() {
        if (this.FontReference.Asset?.Content is { } font) {
            var initialSize = font.MeasureString(this.Text);

            if (this.HeightOverride.IsEnabled && initialSize.Y > 0) {
                this._actualHeight = this.HeightOverride.Value * this.Game.ScreenPixelsPerUnit;
                this._scale = this._actualHeight / initialSize.Y;
                this._actualWidth = initialSize.X * this._scale;
            }
            else {
                this._scale = 1f;
                this._actualWidth = initialSize.X;
                this._actualHeight = initialSize.Y;
            }
        }
        else {
            this._scale = 0f;
            this._actualWidth = 0f;
            this._actualHeight = 0f;
        }
    }
}