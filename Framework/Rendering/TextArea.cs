namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A text area.
/// </summary>
public class TextArea : RenderableEntity, IRenderableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly List<(SpriteSheetFontCharacter character, Vector2 position)> _spriteCharacters = new();
    private float _height;
    private int _kerning;
    private string _text = string.Empty;
    private float _width;
    private SpriteSheetFont? _font;
    private SpriteSheet? _spriteSheet;
    
    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextLine" /> class.
    /// </summary>
    public TextArea() {
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
    public override void Initialize(IScene scene, IEntity parent) {
        this.FontReference.PropertyChanged -= this.FontReference_PropertyChanged;
        base.Initialize(scene, parent);

        this.FontReference.Initialize(this.Scene.Assets);
        this.FontReference.AssetLoaded += this.FontReference_AssetLoaded;
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
            foreach (var (character, position) in this._spriteCharacters) {
                this._spriteSheet.Draw(
                    spriteBatch,
                    this.Project.PixelsPerUnit,
                    character.SpriteIndex,
                    position,
                    colorOverride,
                    this.RenderOptions.Orientation);
            }
        }
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();

        if (this.IsInitialized) {
            this.ResetSize();
            this.ResetLines();
        }
    }

    private bool CouldBeVisible() {
        return !string.IsNullOrEmpty(this.Text) &&
               this._height > 0f &&
               this._width > 0f &&
               this.FontReference.Asset != null;
    }

    private BoundingArea CreateBoundingArea() {
        return this.CouldBeVisible() ? this.RenderOptions.CreateBoundingArea(this) : BoundingArea.Empty;
    }

    private Vector2 CreateSize() {
        return new Vector2(this.Width * this.Project.PixelsPerUnit, this.Height * this.Project.PixelsPerUnit);
    }

    private void FontReference_AssetLoaded(object? sender, EventArgs e) {
        this.RequestRefresh();
    }

    private void FontReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.RequestRefresh();
    }

    private void RequestRefresh() {
        if (this.IsInitialized) {
            this.ResetLines();
        }
    }

    private void ResetLines() {
        this._spriteCharacters.Clear();
        
        if (this.FontReference is { PackagedAsset: not null, Asset: not null }) {
            this._font = this.FontReference.PackagedAsset;
            this._spriteSheet = this.FontReference.Asset;
        }
        else if (this.Project.Fallbacks.Font is { PackagedAsset: not null, Asset: not null }) {
            this._font = this.Project.Fallbacks.Font.PackagedAsset;
            this._spriteSheet = this.Project.Fallbacks.Font.Asset;
        }

        if (this._font != null && this._spriteSheet != null) {
            var characterHeight = this._spriteSheet.SpriteSize.Y * this.Project.UnitsPerPixel;
            var position = new Vector2(this.BoundingArea.Minimum.X, this.BoundingArea.Maximum.Y - characterHeight);
            foreach (var character in this.Text) {
                if (position.Y < this.BoundingArea.Minimum.Y) {
                    return;
                }

                if (this._font.TryGetSpriteCharacter(character, out var spriteCharacter)) {
                    var characterWidth = this._font.GetCharacterWidth(spriteCharacter, this.Kerning, this.Project);
                    if (position.X + characterWidth > this.BoundingArea.Maximum.X) {
                        position = new Vector2(this.BoundingArea.Minimum.X, position.Y - characterHeight);

                        if (character == ' ') {
                            break;
                        }

                        if (position.Y < this.BoundingArea.Minimum.Y) {
                            return;
                        }
                    }

                    this._spriteCharacters.Add((spriteCharacter, position));
                    position = new Vector2(position.X + characterWidth, position.Y);
                }
            }
        }
    }

    private void ResetSize() {
        this.RenderOptions.InvalidateSize();
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}