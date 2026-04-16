namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A renderer for <see cref="SpriteSheetFont" /> which renders a single line of text.
/// </summary>
public class TextLineRenderer : BaseSpriteSheetFontRenderer, ILegacyFontRenderer, IUpdateableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly AssetReference<LegacyFontAsset, SpriteFont> _legacyFontReference = new();

    private float _characterHeight;
    private bool _isScrollingRight = true;
    private float _offset;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? ShouldRenderLegacyFontChanged;

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

    /// <summary>
    /// Gets a timer that determines how long this waits on each end of the scrolled text.
    /// </summary>
    [DataMember]
    public GameTimer ScrollWaitTime { get; } = new(1f);

    /// <inheritdoc />
    public virtual bool ShouldUpdate => this.ShouldScroll && this.WidthOverride.IsEnabled && this.TextLine.Width > this.WidthOverride.Value;

    /// <summary>
    /// Gets an override for the width of this instance. If the override is enabled, the text line will not render past the defined width.
    /// </summary>
    [DataMember]
    public FloatOverride WidthOverride { get; } = new();

    /// <summary>
    /// Gets or sets the velocity of the text scrolling.
    /// </summary>
    [DataMember]
    public float ScrollVelocity {
        get;
        set => field = Math.Abs(value);
    } = 3f;

    /// <inheritdoc />
    public bool ShouldRenderLegacyFont {
        get;
        private set {
            if (value != field) {
                field = value;
                this.ShouldRenderLegacyFontChanged.SafeInvoke(this);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this scrolls.
    /// </summary>
    [DataMember(Order = 1)]
    public bool ShouldScroll {
        get;
        set {
            if (field != value) {
                field = value;
                this.ShouldUpdateChanged.SafeInvoke(this);

                if (this.ShouldScroll) {
                    this.ScrollWaitTime.Restart();
                }
            }
        }
    }

    /// <summary>
    /// Gets the text line this is rendering.
    /// </summary>
    protected TextLine TextLine { get; private set; } = TextLine.Empty;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();

        this.RenderOptions.PropertyChanged -= this.RenderSettings_PropertyChanged;
    }


    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.ResetIndexes();
        this.RenderOptions.Initialize(this.CreateSize);
        this._boundingArea.Reset();

        if (this.ShouldScroll) {
            this.ScrollWaitTime.Restart();
        }

        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
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
                    this.IgnoreColorForIcons,
                    this.WidthOverride.Value,
                    this._offset);
            }
            else {
                this.TextLine.Render(
                    spriteBatch,
                    colorOverride,
                    this.GetTextStartPosition(),
                    this.Project.PixelsPerUnit,
                    this.RenderOptions.Orientation,
                    this.IgnoreColorForIcons);
            }
        }
    }

    /// <inheritdoc />
    public void RenderLegacyFont(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.RenderLegacyFont(frameTime, viewBoundingArea, this.RenderOptions.Color);
    }

    /// <inheritdoc />
    public void RenderLegacyFont(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        var text = this.GetFullText();
        if (!string.IsNullOrEmpty(text) && this._legacyFontReference.Asset is { } font && this.SpriteBatch is { } spriteBatch) {
            spriteBatch.Draw(
                this.Game.ScreenPixelsPerUnit, //TODO: swap this based on in editor or not
                font,
                text,
                this.GetTextStartPosition(),
                new Vector2(1f), //TODO: get proper scale
                colorOverride,
                this.RenderOptions.Orientation);
        }
    }

    /// <inheritdoc />
    public virtual void Update(FrameTime frameTime, InputState inputState) {
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
        foreach (var reference in base.GetAssetReferences()) {
            yield return reference;
        }

        yield return this._legacyFontReference;
    }

    /// <summary>
    /// Gets the position to start rendering the text.
    /// </summary>
    /// <returns>The position to start rendering text.</returns>
    protected virtual Vector2 GetTextStartPosition() => this.BoundingArea.Minimum;

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.RequestRefresh();
    }

    /// <inheritdoc />
    protected override void ReloadFontFromCategory() {
        var shouldRenderLegacyFont = this.ShouldRenderLegacyFont;
        var fontFound = this.Project.Fonts.TryGetFont(this.FontCategory, this.Game.DisplaySettings.Culture, out var fontDefinition);
        if (fontFound) {
            this.FontReference.LoadAsset(fontDefinition.SpriteSheetId, fontDefinition.FontId);
            this.ShouldRenderLegacyFont = false;
        }
        else {
            this._legacyFontReference.ContentId = fontDefinition.FontId;
            this.ShouldRenderLegacyFont = !this.FontReference.HasContent && this._legacyFontReference.HasContent;
        }

        if (shouldRenderLegacyFont != this.ShouldRenderLegacyFont) {
            this.ShouldRenderLegacyFontChanged.SafeInvoke(this);
        }
    }

    /// <inheritdoc />
    protected override void RequestRefresh() {
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

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderOptions.Offset)) {
            this._boundingArea.Reset();
        }
    }

    private void ResetIndexes() {
        this.TextLine = TextLine.Empty;

        this.ResetFontAndSpriteSheet();

        if (this.Font != null) {
            var actualText = this.GetFullText();
            this.TextLine = TextLine.CreateTextLine(this.Project, this.Game.InputActionIconResolver, actualText, this.Font, this.Kerning);
        }
    }

    private void WidthOverride_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.RequestRefresh();
    }
}