namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A renderer for <see cref="SpriteSheetFont" /> which renders a single line of text.
/// </summary>
public class TextLine : RenderableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly List<byte> _spriteIndexes = new();
    private Color _color = Color.White;
    private int _kerning;
    private string _text = string.Empty;

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
    /// Gets the character height.
    /// </summary>
    public float CharacterHeight { get; private set; }

    /// <summary>
    /// Gets the character width.
    /// </summary>
    public float CharacterWidth { get; private set; }

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    /// <value>The color.</value>
    [DataMember(Order = 1)]
    public Color Color {
        get => this._color;
        set => this.Set(ref this._color, value);
    }

    /// <summary>
    /// Gets or sets the kerning. This is the space between letters in pixels. Positive numbers will increase the space, negative numbers will decrease it.
    /// </summary>
    [DataMember]
    public int Kerning {
        get => this._kerning;
        set {
            if (this.Set(ref this._kerning, value)) {
                this.ResetCharacterSizes();
                this.RequestRefresh();
            }
        }
    }

    /// <summary>
    /// Gets or sets the render settings.
    /// </summary>
    /// <value>The render settings.</value>
    [DataMember(Order = 4, Name = "Render Settings")]
    public RenderSettings RenderSettings { get; private set; } = new();

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    [DataMember]
    public string Text {
        get => this._text;
        set {
            if (this.Set(ref this._text, value)) {
                this.RequestRefresh();
            }
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.FontReference.Initialize(this.Scene.Assets);
        this.ResetCharacterSizes();
        this.RenderSettings.Initialize(this.CreateSize);
        this.Refresh();
        this.RenderSettings.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.FontReference.PropertyChanged += this.FontReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.CouldBeVisible(out var spriteSheet) && this.SpriteBatch is { } spriteBatch) {
            for (var i = 0; i < this._spriteIndexes.Count; i++) {
                spriteSheet.Draw(
                    spriteBatch,
                    this.Settings.PixelsPerUnit,
                    this._spriteIndexes[i],
                    this.GetCharacterPosition(i),
                    this.Color,
                    this.RenderSettings.Orientation);
            }
        }
    }

    /// <summary>
    /// Gets the character position for the character that is <see cref="characterIndex" /> characters into <see cref="Text" />.
    /// </summary>
    /// <param name="characterIndex">The character index.</param>
    /// <returns>The position of the character for rendering purposes.</returns>
    protected virtual Vector2 GetCharacterPosition(int characterIndex) {
        return new Vector2(this.BoundingArea.Minimum.X + characterIndex * this.CharacterWidth, this.BoundingArea.Minimum.Y);
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(this.WorldPosition)) {
            this.RequestRefresh();
        }
    }

    /// <summary>
    /// Refreshes this instance.
    /// </summary>
    protected virtual void Refresh() {
        this.ResetIndexes();
        this.RenderSettings.InvalidateSize();
        this._boundingArea.Reset();
    }

    private bool CouldBeVisible([NotNullWhen(true)] out SpriteSheetAsset? spriteSheet) {
        spriteSheet = this.FontReference.Asset;
        return !string.IsNullOrEmpty(this.Text) &&
               this.CharacterHeight > 0f &&
               this.CharacterWidth > 0f &&
               spriteSheet != null;
    }

    private BoundingArea CreateBoundingArea() {
        BoundingArea result;
        if (this.CouldBeVisible(out _) && this.RenderSettings.Size.X != 0f && this.RenderSettings.Size.Y != 0f) {
            var unitsPerPixel = this.Settings.UnitsPerPixel;
            var (x, y) = this.RenderSettings.Size;
            var width = x * unitsPerPixel;
            var height = y * unitsPerPixel;
            var offset = this.RenderSettings.Offset * unitsPerPixel;
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
            return new Vector2(this._spriteIndexes.Count * (spriteSheet.SpriteSize.X + this.Kerning), spriteSheet.SpriteSize.Y);
        }

        return Vector2.Zero;
    }

    private void FontReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.ResetCharacterSizes();
    }

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderSettings.Offset)) {
            this._boundingArea.Reset();
        }
    }


    private void RequestRefresh() {
        if (this.IsInitialized) {
            this.Refresh();
        }
    }

    private void ResetCharacterSizes() {
        if (this.FontReference.Asset is { } spriteSheet) {
            this.CharacterWidth = (spriteSheet.SpriteSize.X + this.Kerning) * this.Settings.UnitsPerPixel;
            this.CharacterHeight = spriteSheet.SpriteSize.Y * this.Settings.UnitsPerPixel;
        }
    }

    private void ResetIndexes() {
        this._spriteIndexes.Clear();

        if (this.FontReference.PackagedAsset is { } font) {
            foreach (var character in this.Text) {
                if (font.TryGetSpriteIndex(character, out var spriteIndex)) {
                    this._spriteIndexes.Add(spriteIndex);
                }
                else {
                    this._spriteIndexes.Add(0);
                }
            }
        }
    }
}