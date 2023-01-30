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
    private float _characterHeight;
    private float _characterWidth;
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
                this.Refresh();
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
                this.Refresh();
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
            var rowNumber = 0;
            var verticalPosition = this.BoundingArea.Minimum.Y;

            foreach (var spriteIndex in this._spriteIndexes) {
                var horizontalPosition = this.BoundingArea.Minimum.X + rowNumber * this._characterWidth;

                spriteSheet.Draw(
                    spriteBatch,
                    this.Settings.PixelsPerUnit,
                    spriteIndex,
                    new Vector2(horizontalPosition, verticalPosition),
                    Vector2.One,
                    0f,
                    this.Color,
                    this.RenderSettings.Orientation);

                rowNumber++;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(this.Transform)) {
            this.Refresh();
        }
    }

    private bool CouldBeVisible([NotNullWhen(true)] out SpriteSheetAsset? spriteSheet) {
        spriteSheet = this.FontReference.Asset;
        return !string.IsNullOrEmpty(this.Text) &&
               this._characterHeight > 0f &&
               this._characterWidth > 0f &&
               this.LocalScale.X != 0f &&
               this.LocalScale.Y != 0f &&
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
                this.GetWorldTransform(offset).Position,
                this.GetWorldTransform(offset + new Vector2(width, 0f)).Position,
                this.GetWorldTransform(offset + new Vector2(width, height)).Position,
                this.GetWorldTransform(offset + new Vector2(0f, height)).Position
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

    private void Refresh() {
        this.ResetIndexes();
        this.RenderSettings.InvalidateSize();
        this._boundingArea.Reset();
    }

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderSettings.Offset)) {
            this._boundingArea.Reset();
        }
    }

    private void ResetCharacterSizes() {
        if (this.FontReference.Asset is { } spriteSheet) {
            this._characterWidth = (spriteSheet.SpriteSize.X + this.Kerning) * this.Settings.UnitsPerPixel;
            this._characterHeight = spriteSheet.SpriteSize.Y * this.Settings.UnitsPerPixel;
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