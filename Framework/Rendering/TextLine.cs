namespace Macabresoft.Macabre2D.Framework;

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
    private readonly Dictionary<char, float> _characterToWidth = new();
    private readonly Dictionary<char, float> _characterToOffset = new();
    private readonly List<SpriteSheetFontCharacter> _spriteCharacters = new();
    private float _characterHeight;
    private int _kerning;
    private string _text = string.Empty;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextLine" /> class.
    /// </summary>
    public TextLine() {
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
    [DataMember(Order = 4, Name = "Render Options")]
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
        base.Initialize(scene, parent);

        this.FontReference.Initialize(this.Scene.Assets);
        this.ResetIndexes();
        this.RenderOptions.Initialize(this.CreateSize);
        this._boundingArea.Reset();
        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.FontReference.PropertyChanged += this.FontReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (!this.BoundingArea.IsEmpty && this.FontReference.Asset is { } spriteSheet && this.SpriteBatch is { } spriteBatch) {
            var position = this.BoundingArea.Minimum;

            foreach (var character in this._spriteCharacters) {
                if (this._characterToOffset.TryGetValue(character.Character, out var offset)) {
                    position = new Vector2(position.X + offset, position.Y);
                }
                
                spriteSheet.Draw(
                    spriteBatch,
                    this.Settings.PixelsPerUnit,
                    character.SpriteIndex,
                    position,
                    this.Color,
                    this.RenderOptions.Orientation);

                if (this._characterToWidth.TryGetValue(character.Character, out var width)) {
                    position = new Vector2(position.X + width, position.Y);
                }
            }
        }
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.RequestRefresh();
    }

    private bool CouldBeVisible() {
        return !string.IsNullOrEmpty(this.Text) &&
               this._characterHeight > 0f &&
               this._spriteCharacters.Any() &&
               this.FontReference.Asset != null;
    }

    private BoundingArea CreateBoundingArea() {
        BoundingArea result;
        if (this.CouldBeVisible() && this.RenderOptions.Size != Vector2.Zero) {
            var unitsPerPixel = this.Settings.UnitsPerPixel;
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

            if (this.ShouldSnapToPixels(this.Settings)) {
                minimumX = minimumX.ToPixelSnappedValue(this.Settings);
                minimumY = minimumY.ToPixelSnappedValue(this.Settings);
                maximumX = maximumX.ToPixelSnappedValue(this.Settings);
                maximumY = maximumY.ToPixelSnappedValue(this.Settings);
            }

            result = new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
        }
        else {
            result = BoundingArea.Empty;
        }

        return result;
    }

    private Vector2 CreateSize() {
        if (this.FontReference.Asset is { } spriteSheet) {
            var unitWidth = 0f;

            foreach (var character in this._spriteCharacters) {
                if (this._characterToWidth.TryGetValue(character.Character, out var width)) {
                    unitWidth += width;
                }
            }

            this._characterHeight = spriteSheet.SpriteSize.Y * this.Settings.UnitsPerPixel;
            return new Vector2(unitWidth * this.Settings.PixelsPerUnit, spriteSheet.SpriteSize.Y);
        }

        return Vector2.Zero;
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
            this.RenderOptions.InvalidateSize();
            this._boundingArea.Reset();
            this.BoundingAreaChanged.SafeInvoke(this);
        }
    }

    private void ResetIndexes() {
        this._spriteCharacters.Clear();
        this._characterToWidth.Clear();
        this._characterToOffset.Clear();

        if (this.FontReference.PackagedAsset is { SpriteSheet: { } spriteSheet } font) {
            foreach (var character in this.Text) {
                if (font.TryGetSpriteCharacter(character, out var spriteCharacter)) {
                    this._spriteCharacters.Add(spriteCharacter);

                    if (spriteCharacter.Kerning != 0) {
                        this._characterToOffset[character] = 0.5f * spriteCharacter.Kerning * this.Settings.UnitsPerPixel;
                        this._characterToWidth[character] = (spriteSheet.SpriteSize.X + this.Kerning + spriteCharacter.Kerning) * this.Settings.UnitsPerPixel;
                    }
                    else {
                        this._characterToWidth[character] = (spriteSheet.SpriteSize.X + this.Kerning) * this.Settings.UnitsPerPixel;
                    }
                }
            }
        }
    }
}