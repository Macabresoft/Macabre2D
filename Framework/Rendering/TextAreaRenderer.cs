namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// Renders text over an area.
/// </summary>
public class TextAreaRenderer : RenderableEntity, ITextRenderer {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly List<TextLine> _textLines = new();
    private bool _constrainHeight = true;
    private SpriteSheetFont? _font;
    private FontCategory _fontCategory = FontCategory.None;
    private float _height;
    private TextAlignment _horizontalAlignment;
    private int _kerning;
    private Vector2 _renderStartPosition;
    private string _resourceName = string.Empty;
    private string _resourceText = string.Empty;
    private SpriteSheet? _spriteSheet;
    private string _stringFormat = string.Empty;
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
    /// Gets the character height of this font.
    /// </summary>
    public float CharacterHeight { get; private set; }



    /// <summary>
    /// Gets or sets a value indicating whether this should constrain height vertically.
    /// </summary>
    /// <remarks>
    /// If this is set to false, the <see cref="Height" /> will not be used at all.
    /// </remarks>
    [DataMember]
    public bool ConstrainHeight {
        get => this._constrainHeight;
        set {
            if (this.Set(ref this._constrainHeight, value)) {
                this.OnSizeChanged();
            }
        }
    }

    /// <inheritdoc />
    [DataMember]
    public FontCategory FontCategory {
        get => this._fontCategory;
        set {
            if (value != this._fontCategory) {
                this._fontCategory = value;
                this.ReloadFontFromCategory();
                this.RequestRefresh();
            }
        }
    }

    /// <inheritdoc />
    [DataMember]
    public string Format {
        get => this._stringFormat;
        set {
            if (value != this._stringFormat) {
                this._stringFormat = value;
                this.RequestRefresh();
            }
        }
    }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    [DataMember]
    public float Height {
        get => this._height;
        set {
            this._height = value;
            this.OnSizeChanged();
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

    /// <inheritdoc />
    [ResourceName]
    [DataMember]
    public string ResourceName {
        get => this._resourceName;
        set {
            this._resourceName = value;
            this.ResetResource();
            this.RequestRefresh();
        }
    }

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
    /// Gets the height of the total text, including text that is not displayed within the <see cref="BoundingArea" />.
    /// </summary>
    public float TextHeight { get; private set; }

    /// <summary>
    /// Gets or sets the vertical offset.
    /// </summary>
    public float VerticalOffset { get; set; }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    [DataMember]
    public float Width {
        get => this._width;
        set {
            this._width = value;
            this.OnSizeChanged();
        }
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.FontReference.AssetChanged -= this.FontReferenceAssetChanged;
        this.FontReference.PropertyChanged -= this.FontReference_PropertyChanged;
    }

    /// <inheritdoc />
    public string GetFullText() {
        var actualText = string.IsNullOrEmpty(this.ResourceName) ? this.Text : this._resourceText;
        return !string.IsNullOrEmpty(this._stringFormat) ? string.Format(actualText, this._stringFormat) : actualText;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.VerticalOffset = 0f;
        this.RenderOptions.Initialize(this.CreateSize);
        this.ReloadFontFromCategory();
        this.ResetResource();
        this.ResetLines();
        this.ResetStartPosition();

        if (!this._constrainHeight) {
            // Size will be based off of the lines added above, because height is not constrained.
            this.ResetSize();
        }

        this.FontReference.AssetChanged += this.FontReferenceAssetChanged;
        this.FontReference.PropertyChanged += this.FontReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.RenderOptions.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (!this.BoundingArea.IsEmpty && this._spriteSheet != null && this.SpriteBatch is { } spriteBatch) {
            var currentPosition = new Vector2(this._renderStartPosition.X, this._renderStartPosition.Y + this.VerticalOffset);
            var topPosition = this.BoundingArea.Maximum.Y + Defaults.FloatComparisonTolerance;
            foreach (var line in this._textLines) {
                if (currentPosition.Y >= this.BoundingArea.Minimum.Y && currentPosition.Y + this.CharacterHeight <= topPosition) {
                    line.Render(
                        spriteBatch,
                        this._spriteSheet,
                        colorOverride,
                        currentPosition,
                        this.Project.PixelsPerUnit,
                        this.RenderOptions.Orientation);
                }

                currentPosition = new Vector2(currentPosition.X, currentPosition.Y - this.CharacterHeight);
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
        this._width > 0f &&
        (this._height > 0f || !this._constrainHeight) &&
        this._font != null &&
        !string.IsNullOrEmpty(this.GetFullText());

    private BoundingArea CreateBoundingArea() => this.CouldBeVisible() ? this.RenderOptions.CreateBoundingArea(this) : BoundingArea.Empty;

    private Vector2 CreateSize() {
        if (this._constrainHeight) {
            return new Vector2(this.Width * this.Project.PixelsPerUnit, this.Height * this.Project.PixelsPerUnit);
        }

        if (this._spriteSheet is { } spriteSheet) {
            return new Vector2(this.Width * this.Project.PixelsPerUnit, this._textLines.Count * spriteSheet.SpriteSize.Y);
        }

        return Vector2.Zero;
    }

    private void FontReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.RequestRefresh();
    }

    private void FontReferenceAssetChanged(object? sender, bool hasAsset) {
        this.RequestRefresh();
    }

    private void OnSizeChanged() {
        if (this.IsInitialized) {
            if (this._constrainHeight) {
                this.ResetSize();
                this.ResetLines();
            }
            else {
                this.ResetLines();
                this.ResetSize();
            }

            this.ResetStartPosition();
        }
    }

    private void ReloadFontFromCategory() {
        if (this.Project.Fonts.TryGetFont(this.FontCategory, this.Game.DisplaySettings.Culture, out var fontDefinition)) {
            this.FontReference.LoadAsset(fontDefinition.SpriteSheetId, fontDefinition.FontId);
        }
    }

    private void RequestRefresh() {
        if (this.IsInitialized) {
            this.ResetLines();

            if (!this._constrainHeight) {
                this.ResetSize();
                this.ResetStartPosition();
            }
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
            this.CharacterHeight = this._spriteSheet.SpriteSize.Y * this.Project.UnitsPerPixel;
            this._textLines.AddRange(TextLine.CreateTextLines(this.Project, this.GetFullText(), this.Width, this._font, this.Kerning, this.HorizontalAlignment));
            this.TextHeight = this.CharacterHeight * this._textLines.Count;
        }
    }

    private void ResetResource() {
        if (!string.IsNullOrEmpty(this.ResourceName)) {
            if (Resources.ResourceManager.TryGetString(this.ResourceName, out var resource)) {
                this._resourceText = resource;
            }
        }
    }

    private void ResetSize() {
        this.RenderOptions.InvalidateSize();
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    private void ResetStartPosition() {
        this._renderStartPosition = new Vector2(this.BoundingArea.Minimum.X, this.BoundingArea.Maximum.Y - this.CharacterHeight);
    }
}