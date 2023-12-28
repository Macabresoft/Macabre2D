﻿namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A renderer for <see cref="SpriteSheetFont" /> which renders a single line of text.
/// </summary>
public class TextLine : RenderableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly List<SpriteSheetFontCharacter> _spriteCharacters = new();
    private float _characterHeight;
    private SpriteSheetFont? _font;
    private SpriteSheetFontReference? _fontReference;
    private int _kerning;
    private SpriteSheet? _spriteSheet;
    private string _text = string.Empty;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextLine" /> class.
    /// </summary>
    public TextLine() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea {
        get => this._boundingArea.Value;
    }

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

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        this.FontReference.AssetLoaded -= this.FontReference_AssetLoaded;
        this.RenderOptions.PropertyChanged -= this.RenderSettings_PropertyChanged;
        this.FontReference.PropertyChanged -= this.FontReference_PropertyChanged;

        base.Initialize(scene, parent);

        this.FontReference.Initialize(this.Scene.Assets);
        this.FontReference.AssetLoaded += this.FontReference_AssetLoaded;
        this.ResetIndexes();
        this.RenderOptions.Initialize(this.CreateSize);
        this._boundingArea.Reset();
        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.FontReference.PropertyChanged += this.FontReference_PropertyChanged;
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

    /// <summary>
    /// Gets the kerning for rendering.
    /// </summary>
    /// <remarks>
    /// Allows dynamic kerning by being overridable.
    /// </remarks>
    /// <returns>The kerning.</returns>
    protected virtual int GetKerning() {
        return this.Kerning;
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.RequestRefresh();
    }

    /// <summary>
    /// Renders the text with the specified font.
    /// </summary>
    /// <param name="fontReference">The font reference.</param>
    /// <param name="color">The color.</param>
    protected void RenderWithFont(SpriteSheetFontReference fontReference, Color color) {
        if (!this.BoundingArea.IsEmpty && fontReference is { PackagedAsset: { } font, Asset: { } spriteSheet } && this.SpriteBatch is { } spriteBatch) {
            var position = this.BoundingArea.Minimum;
            var kerning = this.GetKerning();

            foreach (var character in this._spriteCharacters) {
                spriteSheet.Draw(
                    spriteBatch,
                    this.Project.PixelsPerUnit,
                    character.SpriteIndex,
                    position,
                    color,
                    this.RenderOptions.Orientation);

                position = new Vector2(position.X + font.GetCharacterWidth(character, kerning, this.Project), position.Y);
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
    }

    private bool CouldBeVisible() {
        return !string.IsNullOrEmpty(this.Text) &&
               this._characterHeight > 0f &&
               this._spriteCharacters.Any() &&
               this._font != null;
    }

    private BoundingArea CreateBoundingArea() {
        return this.CouldBeVisible() ? this.RenderOptions.CreateBoundingArea(this) : BoundingArea.Empty;
    }

    private Vector2 CreateSize() {
        if (this._font != null && this._spriteSheet != null) {
            var kerning = this.GetKerning();
            var unitWidth = this._spriteCharacters.Sum(character => this._font.GetCharacterWidth(character, kerning, this.Project));
            this._characterHeight = this._spriteSheet.SpriteSize.Y * this.Project.UnitsPerPixel;
            return new Vector2(unitWidth * this.Project.PixelsPerUnit, this._spriteSheet.SpriteSize.Y);
        }

        return Vector2.Zero;
    }

    private void FontReference_AssetLoaded(object? sender, EventArgs e) {
        this.RequestRefresh();
    }

    private void FontReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.RenderOptions.InvalidateSize();
    }

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderOptions.Offset)) {
            this._boundingArea.Reset();
        }
    }

    private void RequestRefresh() {
        if (this.IsInitialized) {
            this.ResetIndexes();
            this.ResetSize();
        }
    }

    private void ResetIndexes() {
        this._spriteCharacters.Clear();

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
            foreach (var character in this.Text) {
                if (this._font.TryGetSpriteCharacter(character, out var spriteCharacter)) {
                    this._spriteCharacters.Add(spriteCharacter);
                }
            }
        }
    }
}