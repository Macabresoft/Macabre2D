namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// An entity which covers a rectangular area in a single color.
/// </summary>
public class SingleColorBox : RenderableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private Texture2D? _texture;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleColorBox" /> class.
    /// </summary>
    public SingleColorBox() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets the render options.
    /// </summary>
    [DataMember]
    public RenderOptions RenderOptions { get; } = new();

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public override RenderPriority RenderPriority { get; set; }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    [DataMember]
    public Vector2 Size {
        get;
        set {
            field = value;
            this.Reset();
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        if (this._texture == null) {
            this._texture = new Texture2D(this.Game.GraphicsDevice, 1, 1);
            this._texture?.SetData([Color.White]);
        }

        this.RenderOptions.Initialize(this.CreateSize);
        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.RenderOptions.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this._texture != null && this.SpriteBatch is { } spriteBatch && !this.BoundingArea.IsEmpty) {
            var scale = this.RenderOptions.Size;
            spriteBatch.Draw(
                this._texture,
                this.BoundingArea.Minimum * this.Project.PixelsPerUnit,
                null,
                colorOverride,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }
    }

    /// <inheritdoc />
    protected override void OnDisposing() {
        base.OnDisposing();
        this._texture?.Dispose();
        this._texture = null;
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.Reset();
    }

    private BoundingArea CreateBoundingArea() => this.RenderOptions.CreateBoundingArea(this);

    private Vector2 CreateSize() => this.Size * this.Project.PixelsPerUnit;

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderOptions.Offset)) {
            this.ResetBoundingArea();
        }
    }

    private void Reset() {
        this.RenderOptions.InvalidateSize();
        this.ResetBoundingArea();
    }

    private void ResetBoundingArea() {
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}