namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// A renderer for <see cref="SpriteSheetFont" /> which renders a single line of text.
/// </summary>
public class TextLineRenderer : RenderableEntity, ITextRenderer, IUpdateableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private float _characterHeight;
    private SpriteSheetFont? _font;
    private FontCategory _fontCategory = FontCategory.None;
    private SpriteSheetFontReference? _fontReference;
    private bool _isScrollingRight = true;
    private int _kerning;
    private float _offset;
    private string _resourceName = string.Empty;
    private string _resourceText = string.Empty;
    private bool _shouldScroll;
    private SpriteSheet? _spriteSheet;
    private string _stringFormat = string.Empty;
    private string _text = string.Empty;
    private TextLine _textLine = TextLine.Empty;
    private float _totalWidth;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextLineRenderer" /> class.
    /// </summary>
    public TextLineRenderer() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <inheritdoc />
    [DataMember(Order = 1)]
    public Color Color { get; set; } = Color.White;

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
    public SpriteSheetFontReference FontReference { get; } = new();

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

    /// <inheritdoc />
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

    /// <inheritdoc />
    [DataMember(Order = 4)]
    public RenderOptions RenderOptions { get; } = new();

    /// <inheritdoc />
    [ResourceName]
    [DataMember]
    public string ResourceName {
        get => this._resourceName;
        set {
            this._resourceName = value;
            this.RequestRefresh();
        }
    }

    /// <summary>
    /// Gets a timer that determines how long it takes to scroll the entire text.
    /// </summary>
    [DataMember]
    public GameTimer ScrollTime { get; } = new(1f);

    /// <summary>
    /// Gets a timer that determines how long this waits on each end of the scrolled text.
    /// </summary>
    [DataMember]
    public GameTimer ScrollWaitTime { get; } = new(1f);

    /// <summary>
    /// Gets or sets a value indicating whether this scrolls.
    /// </summary>
    [DataMember(Order = 1)]
    public bool ShouldScroll {
        get => this._shouldScroll;
        set {
            if (this._shouldScroll != value) {
                this._shouldScroll = value;
                this.RaisePropertyChanged(nameof(this.ShouldUpdate));

                if (this.ShouldScroll) {
                    this.ScrollWaitTime.Restart();
                    this.ScrollTime.Restart();
                }
            }
        }
    }

    /// <inheritdoc />
    public bool ShouldUpdate => this.ShouldScroll && this.WidthOverride.IsEnabled && this._totalWidth > this.WidthOverride.Value;

    /// <inheritdoc />
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
    /// Gets a an override for the width of this instance. If the override is enabled, the text line will not render past the defined width.
    /// </summary>
    [DataMember]
    public FloatOverride WidthOverride { get; } = new();

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();

        this.FontReference.AssetChanged -= this.FontReferenceAssetChanged;
        this.RenderOptions.PropertyChanged -= this.RenderSettings_PropertyChanged;
        this.FontReference.PropertyChanged -= this.FontReference_PropertyChanged;
        this.Game.CultureChanged -= this.Game_CultureChanged;
    }

    /// <inheritdoc />
    public virtual string GetFullText() {
        var actualText = string.IsNullOrEmpty(this.ResourceName) ? this.Text : this._resourceText;
        return !string.IsNullOrEmpty(this._stringFormat) ? string.Format(actualText, this._stringFormat) : actualText;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.ReloadFontFromCategory();
        this.ResetResource();
        this.ResetIndexes();
        this.RenderOptions.Initialize(this.CreateSize);
        this._boundingArea.Reset();

        if (this.ShouldScroll) {
            this.ScrollTime.Restart();
            this.ScrollWaitTime.Restart();
        }

        this.FontReference.AssetChanged += this.FontReferenceAssetChanged;
        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.FontReference.PropertyChanged += this.FontReference_PropertyChanged;
        this.Game.CultureChanged += this.Game_CultureChanged;
        this.WidthOverride.PropertyChanged += this.WidthOverride_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this._fontReference != null) {
            this.RenderWithFont(this._fontReference, colorOverride);
        }
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        if (this.ScrollWaitTime.State == TimerState.Running) {
            this.HandleWaiting(frameTime);
        }
        else {
            this.HandleScrolling(frameTime);
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.FontReference;
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.RequestRefresh();
    }

    /// <summary>
    /// Resets the size and bounding area.
    /// </summary>
    protected void ResetSize() {
        this.RenderOptions.InvalidateSize();
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
        this.RaisePropertyChanged(nameof(this.ShouldUpdate));
    }

    private bool CouldBeVisible() =>
        this._characterHeight > 0f &&
        this._font != null &&
        !string.IsNullOrEmpty(this.GetFullText());

    private BoundingArea CreateBoundingArea() {
        var boundingArea = BoundingArea.Empty;
        if (this.CouldBeVisible()) {
            boundingArea = this.RenderOptions.CreateBoundingArea(this);
            this._totalWidth = boundingArea.Width;

            if (this.WidthOverride.IsEnabled) {
                boundingArea = new BoundingArea(boundingArea.Minimum, this.WidthOverride.Value, boundingArea.Height);
            }
        }
        else {
            this._totalWidth = 0f;
        }

        return boundingArea;
    }

    private Vector2 CreateSize() {
        if (this._font != null && this._spriteSheet != null) {
            var unitWidth = this._textLine.Width;
            this._characterHeight = this._spriteSheet.SpriteSize.Y * this.Project.UnitsPerPixel;
            return new Vector2(unitWidth * this.Project.PixelsPerUnit, this._spriteSheet.SpriteSize.Y);
        }

        return Vector2.Zero;
    }

    private void FontReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.RenderOptions.InvalidateSize();
    }

    private void FontReferenceAssetChanged(object? sender, bool hasAsset) {
        this.RequestRefresh();
    }

    private void Game_CultureChanged(object? sender, ResourceCulture e) {
        this.RequestRefresh();
    }

    private void HandleScrolling(FrameTime frameTime) {
        this.ScrollTime.Increment(frameTime);

        var distance = this._totalWidth - this.WidthOverride.Value;
        if (this._isScrollingRight) {
            this._offset = this.ScrollTime.PercentComplete * distance;
        }
        else {
            this._offset = (1f - this.ScrollTime.PercentComplete) * distance;
        }

        if (this.ScrollTime.State != TimerState.Running) {
            this.ScrollWaitTime.Restart();
            this._isScrollingRight = !this._isScrollingRight;
        }
    }

    private void HandleWaiting(FrameTime frameTime) {
        this.ScrollWaitTime.Increment(frameTime);

        if (this.ScrollWaitTime.State == TimerState.Finished) {
            this.ScrollTime.Restart();
        }
    }

    private void ReloadFontFromCategory() {
        if (this.Project.Fonts.TryGetFont(this.FontCategory, this.Game.DisplaySettings.Culture, out var fontDefinition)) {
            this.FontReference.LoadAsset(fontDefinition.SpriteSheetId, fontDefinition.FontId);
        }
    }

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderOptions.Offset)) {
            this._boundingArea.Reset();
        }
    }

    private void RenderWithFont(SpriteSheetFontReference fontReference, Color color) {
        if (!this.BoundingArea.IsEmpty && fontReference is { Asset: { } spriteSheet } && this.SpriteBatch is { } spriteBatch) {
            if (this.ShouldScroll && this.WidthOverride.IsEnabled) {
                this._textLine.Render(
                    spriteBatch,
                    spriteSheet,
                    color,
                    this.BoundingArea.Minimum,
                    this.Project.PixelsPerUnit,
                    this.RenderOptions.Orientation,
                    this.WidthOverride.Value,
                    this._offset);
            }
            else {
                this._textLine.Render(
                    spriteBatch,
                    spriteSheet,
                    color,
                    this.BoundingArea.Minimum,
                    this.Project.PixelsPerUnit,
                    this.RenderOptions.Orientation);
            }
        }
    }

    private void RequestRefresh() {
        if (this.IsInitialized) {
            this.ResetResource();
            this.ResetIndexes();
            this.ResetSize();

            if (this.ShouldScroll) {
                this.ScrollTime.Restart();
                this.ScrollWaitTime.Restart();
            }
        }
    }

    private void ResetIndexes() {
        this._textLine = TextLine.Empty;

        if (this.FontReference is { PackagedAsset: not null, Asset: not null }) {
            this._fontReference = this.FontReference;
            this._font = this.FontReference.PackagedAsset;
            this._spriteSheet = this.FontReference.Asset;
        }
        else if (this.Project.Fallbacks.Font is { PackagedAsset: not null, Asset: not null }) {
            this._fontReference = this.Project.Fallbacks.Font;
            this._font = this.Project.Fallbacks.Font.PackagedAsset;
            this._spriteSheet = this.Project.Fallbacks.Font.Asset;
        }

        if (this._font != null) {
            var actualText = this.GetFullText();
            this._textLine = TextLine.CreateTextLine(this.Project, actualText, this._font, this.Kerning);
        }
    }

    private void ResetResource() {
        if (!string.IsNullOrEmpty(this.ResourceName)) {
            if (Resources.ResourceManager.TryGetString(this.ResourceName, out var resource)) {
                this._resourceText = resource;
            }
        }
    }

    private void WidthOverride_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.RequestRefresh();
    }
}