namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// An entity which will render the specified text.
/// </summary>
public class TextRenderer : RenderableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private string _text = string.Empty;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRenderer" /> class.
    /// </summary>
    public TextRenderer() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets the font reference.
    /// </summary>
    [DataMember(Order = 0)]
    public AssetReference<FontAsset, SpriteFont> FontReference { get; } = new();

    /// <summary>
    /// Gets the render options.
    /// </summary>
    /// <value>The render options.</value>
    [DataMember(Order = 4, Name = "Render Options")]
    public RenderOptions RenderOptions { get; private set; } = new();

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    [DataMember(Order = 2)]
    public string Text {
        get => this._text;
        set {
            if (value != this._text) {
                this._text = value;
                this.ResetBoundingArea();
                this.RenderOptions.InvalidateSize();
            }
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);

        this.FontReference.Initialize(this.Scene.Assets, this.Game);
        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.RenderOptions.Initialize(this.CreateSize);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.RenderOptions.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (!string.IsNullOrEmpty(this.Text) && this.FontReference.Asset is { } font && this.SpriteBatch is { } spriteBatch) {
            spriteBatch.Draw(
                this.Project.PixelsPerUnit,
                font,
                this.Text,
                this.WorldPosition,
                colorOverride,
                this.RenderOptions.Orientation);
        }
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.Reset();
    }

    private BoundingArea CreateBoundingArea() {
        var inversePixelsPerUnit = this.Project.UnitsPerPixel;
        var (x, y) = this.RenderOptions.Size;
        var width = x * inversePixelsPerUnit;
        var height = y * inversePixelsPerUnit;
        var offset = this.RenderOptions.Offset * inversePixelsPerUnit;
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

        return new BoundingArea(new Vector2(minimumX, minimumY), new Vector2(maximumX, maximumY));
    }

    private Vector2 CreateSize() => this.FontReference.Asset?.Content?.MeasureString(this.Text) ?? Vector2.Zero;

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderOptions.Offset)) {
            this.Reset();
        }
    }

    private void Reset() {
        this.ResetBoundingArea();
    }

    private void ResetBoundingArea() {
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}