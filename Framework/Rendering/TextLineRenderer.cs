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
    private FontCategory _fontCategory = FontCategory.None;
    private SpriteSheetFontReference? _fontReference;
    private bool _isScrollingRight = true;
    private int _kerning;
    private float _offset;
    private string _resourceName = string.Empty;
    private string _resourceText = string.Empty;
    private float _scrollVelocity = 3f;
    private bool _shouldScroll;
    private string _stringFormat = string.Empty;
    private string _text = string.Empty;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged;

    /// <inheritdoc />
    public event EventHandler? UpdateOrderChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextLineRenderer" /> class.
    /// </summary>
    public TextLineRenderer() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

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
    /// Gets or sets the velocity of the text scrolling.
    /// </summary>
    public float ScrollVelocity {
        get => this._scrollVelocity;
        set => this._scrollVelocity = Math.Abs(value);
    }

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
                this.ShouldUpdateChanged.SafeInvoke(this);

                if (this.ShouldScroll) {
                    this.ScrollWaitTime.Restart();
                }
            }
        }
    }

    /// <inheritdoc />
    public bool ShouldUpdate => this.ShouldScroll && this.WidthOverride.IsEnabled && this.TextLine.Width > this.WidthOverride.Value;

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
    /// Gets an override for the width of this instance. If the override is enabled, the text line will not render past the defined width.
    /// </summary>
    [DataMember]
    public FloatOverride WidthOverride { get; } = new();

    /// <summary>
    /// Gets the font.
    /// </summary>
    protected SpriteSheetFont? Font { get; private set; }

    /// <summary>
    /// Gets the sprite sheet.
    /// </summary>
    protected SpriteSheet? SpriteSheet { get; private set; }

    /// <summary>
    /// Gets the text line this is rendering.
    /// </summary>
    protected TextLine TextLine { get; private set; } = TextLine.Empty;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();

        this.FontReference.AssetChanged -= this.FontReference_AssetChanged;
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
            this.ScrollWaitTime.Restart();
        }

        this.FontReference.AssetChanged += this.FontReference_AssetChanged;
        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.FontReference.PropertyChanged += this.FontReference_PropertyChanged;
        this.Game.CultureChanged += this.Game_CultureChanged;
        this.WidthOverride.PropertyChanged += this.WidthOverride_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.RenderOptions.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (!this.BoundingArea.IsEmpty && this.SpriteBatch is { } spriteBatch) {
            if (this.ShouldScroll && this.WidthOverride.IsEnabled) {
                this.TextLine.Render(
                    spriteBatch,
                    colorOverride,
                    this.GetTextStartPosition(),
                    this.Project.PixelsPerUnit,
                    this.RenderOptions.Orientation,
                    this.WidthOverride.Value,
                    this._offset);
            }
            else {
                this.TextLine.Render(
                    spriteBatch,
                    colorOverride,
                    this.GetTextStartPosition(),
                    this.Project.PixelsPerUnit,
                    this.RenderOptions.Orientation);
            }
        }
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        if (this.ScrollWaitTime.State == TimerState.Running) {
            this.ScrollWaitTime.Increment(frameTime);
        }
        else {
            this.HandleScrolling(frameTime);
        }
    }

    /// <summary>
    /// Creates the bounding area for this renderer.
    /// </summary>
    /// <returns>The bounding area.</returns>
    protected virtual BoundingArea CreateBoundingArea() {
        var boundingArea = BoundingArea.Empty;
        if (this.CouldBeVisible()) {
            boundingArea = this.RenderOptions.CreateBoundingArea(this);

            if (this.WidthOverride.IsEnabled) {
                boundingArea = new BoundingArea(boundingArea.Minimum, this.WidthOverride.Value, boundingArea.Height);
            }
        }

        return boundingArea;
    }

    /// <summary>
    /// Creates the pixel size for <see cref="RenderOptions" />.
    /// </summary>
    /// <returns>The size.</returns>
    protected virtual Vector2 CreateSize() {
        if (this.Font != null && this.SpriteSheet != null) {
            var unitWidth = this.WidthOverride.IsEnabled ? this.WidthOverride.Value : this.TextLine.Width;
            this._characterHeight = this.SpriteSheet.SpriteSize.Y * this.Project.UnitsPerPixel;
            return new Vector2(unitWidth * this.Project.PixelsPerUnit, this.SpriteSheet.SpriteSize.Y);
        }

        return Vector2.Zero;
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.FontReference;
    }

    /// <summary>
    /// Gets the position to start rendering the text.
    /// </summary>
    /// <returns>The position to start rendering text.</returns>
    protected virtual Vector2 GetTextStartPosition() => this.BoundingArea.Minimum;

    /// <summary>
    /// Called when the font changes.
    /// </summary>
    protected virtual void OnFontChanged() {
        this.RequestRefresh();
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.RequestRefresh();
    }

    /// <summary>
    /// Requests a reset that will be performed if this has been initialized.
    /// </summary>
    protected void RequestRefresh() {
        if (this.IsInitialized) {
            this.ResetResource();
            this.ResetIndexes();
            this.ResetSize();

            if (this.ShouldScroll) {
                this.ScrollWaitTime.Restart();
            }
        }
    }

    /// <summary>
    /// Resets the size and bounding area.
    /// </summary>
    protected void ResetSize() {
        this.RenderOptions.InvalidateSize();
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);

        if (this.ShouldScroll) {
            this.ShouldUpdateChanged.SafeInvoke(this);
        }
    }

    private bool CouldBeVisible() =>
        this._characterHeight > 0f &&
        this.Font != null &&
        !string.IsNullOrEmpty(this.GetFullText());

    private void FontReference_AssetChanged(object? sender, bool hasAsset) {
        this.OnFontChanged();
    }

    private void FontReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.RenderOptions.InvalidateSize();
    }

    private void Game_CultureChanged(object? sender, ResourceCulture e) {
        this.RequestRefresh();
    }

    private void HandleScrolling(FrameTime frameTime) {
        if (this._isScrollingRight) {
            var distance = this.TextLine.Width - this.WidthOverride.Value;
            this._offset += (float)frameTime.SecondsPassed * this.ScrollVelocity;
            if (this._offset >= distance) {
                this.ScrollWaitTime.Restart();
                this._isScrollingRight = false;
            }
        }
        else {
            this._offset -= (float)frameTime.SecondsPassed * this.ScrollVelocity;
            if (this._offset <= 0f) {
                this.ScrollWaitTime.Restart();
                this._isScrollingRight = true;
            }
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

    private void ResetIndexes() {
        this.TextLine = TextLine.Empty;

        if (this.FontReference is { PackagedAsset: not null, Asset: not null }) {
            this._fontReference = this.FontReference;
            this.Font = this.FontReference.PackagedAsset;
            this.SpriteSheet = this.FontReference.Asset;
        }
        else if (this.Project.Fallbacks.Font is { PackagedAsset: not null, Asset: not null }) {
            this._fontReference = this.Project.Fallbacks.Font;
            this.Font = this.Project.Fallbacks.Font.PackagedAsset;
            this.SpriteSheet = this.Project.Fallbacks.Font.Asset;
        }

        if (this.Font != null) {
            var actualText = this.GetFullText();
            this.TextLine = TextLine.CreateTextLine(this.Project, actualText, this.Font, this.Kerning);
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