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
[Obsolete("This engine is meant for pixel graphics and TextLineRenderer should be used instead. Use at your own annoyance!")]
public class LegacyTextLineRenderer : Entity, IScreenSpaceRenderer {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private float _actualHeight;
    private float _actualWidth;
    private LegacyTextLine _textLine = LegacyTextLine.Empty;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? ShouldRenderLegacyFontChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="LegacyTextLineRenderer" /> class.
    /// </summary>
    public LegacyTextLineRenderer() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets the font reference.
    /// </summary>
    [DataMember(Order = 0)]
    public AssetReference<LegacyFontAsset, SpriteFont> FontReference { get; } = new();

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

    /// <inheritdoc cref="IScreenSpaceRenderer" />
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
        this.Game.ViewportSizeChanged -= this.Game_ViewportSizeChanged;

        base.Deinitialize();

        this._textLine = LegacyTextLine.Empty;
        this.RenderOptions.PropertyChanged -= this.RenderSettings_PropertyChanged;
        this.HeightOverride.PropertyChanged -= this.HeightOverride_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);

        this.RenderOptions.Initialize(this.CreateSize);
        this.Reset();

        this.Game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
        this.HeightOverride.PropertyChanged += this.HeightOverride_PropertyChanged;
        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
    }

    /// <inheritdoc />
    public void RenderInScreenSpace(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.RenderInScreenSpace(frameTime, viewBoundingArea, this.RenderOptions.Color);
    }

    /// <inheritdoc />
    public void RenderInScreenSpace(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this.SpriteBatch is { } spriteBatch) {
            this._textLine.Render(
                spriteBatch,
                colorOverride,
                this.WorldPosition,
                this.Game.ScreenPixelsPerUnit,
                this.RenderOptions.Orientation);
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

    private void Game_ViewportSizeChanged(object? sender, Point e) {
        if (this.IsInitialized) {
            this.Reset();
        }
    }

    private void HeightOverride_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.Reset();
    }

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderOptions.Offset)) {
            this.Reset();
        }
    }

    private void Reset() {
        this.ResetTextLine();
        this.RenderOptions.InvalidateSize();
        this.ResetBoundingArea();
    }

    private void ResetBoundingArea() {
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private void ResetTextLine() {
        if (this.FontReference.Asset is { Content: { } font } fontAsset) {
            var initialSize = font.MeasureString(this.Text);
            var scale = 1f;

            if (this.HeightOverride.IsEnabled && initialSize.Y > 0) {
                this._actualHeight = this.HeightOverride.Value * this.Game.ScreenPixelsPerUnit;
                scale = this._actualHeight / initialSize.Y;
                this._actualWidth = initialSize.X * scale;
            }
            else {
                this._actualWidth = initialSize.X;
                this._actualHeight = initialSize.Y;
            }

            this._textLine = new LegacyTextLine(fontAsset, scale, this.Game.ScreenPixelsPerUnit, this.Text);
        }
        else {
            this._textLine = LegacyTextLine.Empty;
            this._actualWidth = 0f;
            this._actualHeight = 0f;
        }
    }
}