﻿namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// An entity which covers an entire <see cref="IBoundable" /> with a single color.
/// </summary>
public class BoundableCover : RenderableEntity {
    private Color _color;
    private Texture2D? _texture;
    private Vector2 _padding = Vector2.Zero;
    private Vector2 _paddingForScale = Vector2.Zero;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this.Parent is IBoundable parent ? parent.BoundingArea : BoundingArea.Empty;

    /// <summary>
    /// Gets or sets the color of this cover.
    /// </summary>
    [DataMember]
    public Color Color {
        get => this._color;
        set {
            if (value != this._color) {
                this._color = value;
                this.ResetColor();
            }
        }
    }

    /// <summary>
    /// Gets or sets this padding. This applies additional cover beyond the bounds of the <see cref="IBoundable"/>.
    /// </summary>
    [DataMember]
    public Vector2 Padding {
        get => this._padding;
        set {
            this._padding = value;
            this._paddingForScale = value * 2f;
        }
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        if (this.Parent is IBoundable oldParent) {
            oldParent.BoundingAreaChanged -= this.Parent_BoundingAreaChanged;
        }

        base.Initialize(scene, parent);

        if (this.Parent is IBoundable newParent) {
            newParent.BoundingAreaChanged += this.Parent_BoundingAreaChanged;
        }

        this._texture?.Dispose();
        this._texture = new Texture2D(this.Game.GraphicsDevice, 1, 1);
        this.ResetColor();
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, Color.White);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this._texture != null && this.SpriteBatch is { } spriteBatch && this.Parent is IBoundable boundable) {
            
            var scale = new Vector2(boundable.BoundingArea.Width + this._paddingForScale.X, boundable.BoundingArea.Height + this._paddingForScale.Y) * this.Project.PixelsPerUnit;
            spriteBatch.Draw(
                this._texture,
                (boundable.BoundingArea.Minimum - this.Padding) * this.Project.PixelsPerUnit,
                null,
                colorOverride,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }
    }

    private void Parent_BoundingAreaChanged(object? sender, EventArgs e) {
        this.BoundingAreaChanged?.SafeInvoke(this);
    }

    private void ResetColor() {
        this._texture?.SetData(new[] { this._color });
    }
}