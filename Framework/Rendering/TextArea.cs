namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A text area.
/// </summary>
public class TextArea : Entity, IRenderableEntity {
    private readonly List<(SpriteSheetFontCharacter character, Vector2 position)> _spriteCharacters = new();
    private BoundingArea _boundingArea;
    private bool _isVisible = true;
    private int _kerning;
    private int _renderOrder;
    private string _text = string.Empty;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Gets the font asset reference.
    /// </summary>
    [DataMember]
    public SpriteSheetFontReference FontReference { get; } = new();

    /// <inheritdoc />
    [DataMember]
    public BoundingArea BoundingArea {
        get => this._boundingArea;
        set {
            this._boundingArea = value;
            if (this.IsInitialized) {
                this.BoundingAreaChanged.SafeInvoke(this);
                this.RequestRefresh();
            }
        }
    }

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    [DataMember(Order = 1)]
    public Color Color { get; set; } = Color.White;

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public bool IsVisible {
        get => this._isVisible && this.IsEnabled;
        set => this.Set(ref this._isVisible, value);
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

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public PixelSnap PixelSnap { get; set; } = PixelSnap.Inherit;

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public int RenderOrder {
        get => this._renderOrder;
        set => this.Set(ref this._renderOrder, value);
    }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public bool RenderOutOfBounds { get; set; }

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
        this.FontReference.AssetLoaded += this.FontReference_AssetLoaded;
        this.ResetLines();
        this.FontReference.PropertyChanged += this.FontReference_PropertyChanged;
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (!this.BoundingArea.IsEmpty && this.FontReference is { Asset: { } spriteSheet } && this.SpriteBatch is { } spriteBatch) {
            foreach (var (character, position) in this._spriteCharacters) {
                spriteSheet.Draw(
                    spriteBatch,
                    this.Project.PixelsPerUnit,
                    character.SpriteIndex,
                    position,
                    this.Color,
                    SpriteEffects.FlipVertically);
            }
        }
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.RequestRefresh();
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

        if (this.FontReference.PackagedAsset is { SpriteSheet: { } spriteSheet } font) {
            var characterHeight = spriteSheet.SpriteSize.Y * this.Project.UnitsPerPixel;
            var position = new Vector2(this.BoundingArea.Minimum.X, this.BoundingArea.Maximum.Y - characterHeight);
            foreach (var character in this.Text) {
                if (position.Y < this.BoundingArea.Minimum.Y) {
                    return;
                }

                if (font.TryGetSpriteCharacter(character, out var spriteCharacter)) {
                    var characterWidth = font.GetCharacterWidth(spriteCharacter, this.Kerning, this.Project);
                    if (position.X + characterWidth > this.BoundingArea.Maximum.X) {
                        position = new Vector2(this.BoundingArea.Minimum.X, position.Y - characterHeight);

                        if (character == ' ') {
                            break;
                        }
                    }

                    this._spriteCharacters.Add((spriteCharacter, position));
                    position = new Vector2(position.X + characterWidth, position.Y);
                }
            }
        }
    }
}