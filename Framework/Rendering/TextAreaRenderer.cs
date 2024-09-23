namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Renders text over an area.
/// </summary>
public class TextAreaRenderer : RenderableEntity, IRenderableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly List<TextLine> _textLines = new();
    private float _characterHeight;
    private SpriteSheetFont? _font;
    private float _height;
    private TextAlignment _horizontalAlignment;
    private int _kerning;
    private Vector2 _renderStartPosition;
    private SpriteSheet? _spriteSheet;
    private string _text = string.Empty;
    private float _width;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextLineRenderer" /> class.
    /// </summary>
    public TextAreaRenderer() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets the font asset reference.
    /// </summary>
    [DataMember]
    public SpriteSheetFontReference FontReference { get; } = new();

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    [DataMember(Order = 1)]
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    [DataMember]
    public float Height {
        get => this._height;
        set {
            this._height = value;
            if (this.IsInitialized) {
                this.ResetSize();
                this.ResetLines();
            }
        }
    }

    /// <summary>
    /// Gets or sets the horizontal alignment of the text.
    /// </summary>
    [DataMember]
    public TextAlignment HorizontalAlignment {
        get => this._horizontalAlignment;
        set {
            if (value != this._horizontalAlignment) {
                this._horizontalAlignment = value;
                this.ResetLines();
            }
        }
    }

    /// <summary>
    /// Gets or sets the kerning. This is the space between letters in pixels. Positive numbers will increase the space, negative numbers will decrease it.
    /// </summary>
    [DataMember]
    public int Kerning {
        get => this._kerning;
        set {
            if (value != this._kerning) {
                this._kerning = value;
                this.RequestRefresh();
            }
        }
    }

    /// <summary>
    /// Gets or sets the render options.
    /// </summary>
    /// <value>The render options.</value>
    [DataMember(Order = 4)]
    public RenderOptions RenderOptions { get; private set; } = new();

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    [DataMember]
    public string Text {
        get => this._text;
        set {
            if (value != this._text) {
                this._text = value;
                this.RequestRefresh();
            }
        }
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    [DataMember]
    public float Width {
        get => this._width;
        set {
            this._width = value;
            if (this.IsInitialized) {
                this.ResetSize();
                this.ResetLines();
            }
        }
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.FontReference.PropertyChanged -= this.FontReference_PropertyChanged;
        this.FontReference.AssetChanged -= this.FontReferenceAssetChanged;
        this.FontReference.PropertyChanged -= this.FontReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.FontReference.AssetChanged += this.FontReferenceAssetChanged;
        this.RenderOptions.Initialize(this.CreateSize);
        this.ResetLines();
        this.FontReference.PropertyChanged += this.FontReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (!this.BoundingArea.IsEmpty && this._spriteSheet != null && this.SpriteBatch is { } spriteBatch) {
            var currentPosition = this._renderStartPosition;
            foreach (var line in this._textLines) {
                if (currentPosition.Y < this.BoundingArea.Minimum.Y) {
                    // Don't render out of bounds.
                    // TODO: render part of the text and cut it off?
                    break;
                }
                
                line.Render(
                    spriteBatch,
                    this._spriteSheet,
                    colorOverride,
                    currentPosition,
                    this.Project.PixelsPerUnit,
                    this.RenderOptions.Orientation);

                currentPosition = new Vector2(currentPosition.X, currentPosition.Y - this._characterHeight);
            }
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.FontReference;
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();

        if (this.IsInitialized) {
            this.ResetSize();
            this.ResetStartPosition();
        }
    }

    private bool CouldBeVisible() =>
        !string.IsNullOrEmpty(this.Text) &&
        this._height > 0f &&
        this._width > 0f &&
        this.FontReference.Asset != null;

    private BoundingArea CreateBoundingArea() => this.CouldBeVisible() ? this.RenderOptions.CreateBoundingArea(this) : BoundingArea.Empty;

    private Vector2 CreateSize() => new(this.Width * this.Project.PixelsPerUnit, this.Height * this.Project.PixelsPerUnit);

    private void FontReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.RequestRefresh();
    }

    private void FontReferenceAssetChanged(object? sender, bool hasAsset) {
        this.RequestRefresh();
    }

    private void RequestRefresh() {
        if (this.IsInitialized) {
            this.ResetLines();
        }
    }

    private void ResetLines() {
        this._textLines.Clear();

        if (this.FontReference is { PackagedAsset: not null, Asset: not null }) {
            this._font = this.FontReference.PackagedAsset;
            this._spriteSheet = this.FontReference.Asset;
        }
        else if (this.Project.Fallbacks.Font is { PackagedAsset: not null, Asset: not null }) {
            this._font = this.Project.Fallbacks.Font.PackagedAsset;
            this._spriteSheet = this.Project.Fallbacks.Font.Asset;
        }

        if (this._font != null && this._spriteSheet != null) {
            this._characterHeight = this._spriteSheet.SpriteSize.Y * this.Project.UnitsPerPixel;
            this.ResetStartPosition();
            this._textLines.AddRange(TextLine.CreateTextLines(this.Project, this.Text, this.BoundingArea.Width, this._font, this.Kerning, this.HorizontalAlignment));
        }
    }

    private void ResetSize() {
        this.RenderOptions.InvalidateSize();
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private void ResetStartPosition() {
        this._renderStartPosition = new Vector2(this.BoundingArea.Minimum.X, this.BoundingArea.Maximum.Y - this._characterHeight);
    }
}