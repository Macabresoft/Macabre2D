namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A renderer for <see cref="SpriteSheetFont" />.
/// </summary>
public class SpriteSheetTextRenderer : RenderableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly List<TextLine> _lines = new();
    private float _characterHeight;
    private float _characterWidth;
    private Color _color = Color.White;
    private byte _lineLength;
    private byte _maxLines;
    private string _text = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetTextRenderer" /> class.
    /// </summary>
    public SpriteSheetTextRenderer() {
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
    /// Gets or sets the line length.
    /// </summary>
    [DataMember]
    public byte LineLength {
        get => this._lineLength;
        set {
            if (this.Set(ref this._lineLength, value)) {
                this.Refresh();
            }
        }
    }

    /// <summary>
    /// Gets or sets the line length.
    /// </summary>
    [DataMember]
    public byte MaxLines {
        get => this._maxLines;
        set {
            if (this.Set(ref this._maxLines, value)) {
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

        this.RenderSettings.Initialize(this.CreateSize);
        this.FontReference.Initialize(this.Scene.Assets);
        this.Refresh();
        this.ResetCharacterSizes();
        this.RenderSettings.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.FontReference.PropertyChanged += this.FontReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        if (this.CouldBeVisible(out var spriteSheet) && this.SpriteBatch is { } spriteBatch) {
            var lineNumber = 1;

            foreach (var line in this._lines) {
                var verticalPosition = this.BoundingArea.Maximum.Y - lineNumber * this._characterHeight;
                var rowNumber = 0;

                foreach (var spriteIndex in line.SpriteIndexes) {
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

                if (lineNumber >= this.MaxLines) {
                    break;
                }

                lineNumber++;
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
               this.LineLength > 0 &&
               this.MaxLines > 0 &&
               this._characterHeight > 0f &&
               this._characterWidth > 0f &&
               this.LocalScale.X != 0f &&
               this.LocalScale.Y != 0f &&
               spriteSheet != null;
    }

    private BoundingArea CreateBoundingArea() {
        BoundingArea result;
        if (this.CouldBeVisible(out _) && this.RenderSettings.Size.X != 0f && this.RenderSettings.Size.Y != 0f) {
            var inversePixelsPerUnit = this.Settings.UnitsPerPixel;
            var (x, y) = this.RenderSettings.Size;
            var width = x * inversePixelsPerUnit;
            var height = y * inversePixelsPerUnit;
            var offset = this.RenderSettings.Offset * inversePixelsPerUnit;
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
        var result = Vector2.Zero;
        if (this.CouldBeVisible(out var spriteSheet)) {
            var width = this.LineLength * spriteSheet.SpriteSize.X;
            var height = this.MaxLines * spriteSheet.SpriteSize.Y;
            result = new Vector2(width, height);
        }

        return result;
    }

    private void FontReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.ResetCharacterSizes();
    }

    private void Refresh() {
        this.ResetLines();
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
            this._characterWidth = spriteSheet.SpriteSize.X * this.Settings.UnitsPerPixel;
            this._characterHeight = spriteSheet.SpriteSize.Y * this.Settings.UnitsPerPixel;
        }
    }

    private void ResetLines() {
        this._lines.Clear();

        if (this.MaxLines == 0 || this.LineLength == 0 || string.IsNullOrEmpty(this._text)) {
            return;
        }

        if (this.FontReference.PackagedAsset is { } font) {
            var numberOfLines = (byte)Math.Ceiling(this._text.Length / (float)this.LineLength);
            var lineLength = (int)this.LineLength;
            for (var i = 0; i < numberOfLines; i++) {
                var start = i * this.LineLength;

                if (start >= this.Text.Length) {
                    break;
                }

                if (start + lineLength >= this.Text.Length) {
                    lineLength = this.Text.Length - start;
                }

                this._lines.Add(new TextLine(this.Text.Substring(start, lineLength), font));
            }
        }
    }

    private class TextLine {
        private readonly byte[] _spriteIndexes;

        public TextLine(string text, SpriteSheetFont font) {
            this._spriteIndexes = new byte[text.Length];

            for (var i = 0; i < text.Length; i++) {
                if (font.TryGetSpriteIndex(text[i], out var spriteIndex)) {
                    this._spriteIndexes[i] = spriteIndex;
                }
                else {
                    this._spriteIndexes[i] = 0;
                }
            }
        }

        public IReadOnlyCollection<byte> SpriteIndexes => this._spriteIndexes;
    }
}