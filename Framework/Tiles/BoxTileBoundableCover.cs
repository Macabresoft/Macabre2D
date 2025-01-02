namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="BoxTileMap" /> that automatically sizes to contain its contents.
/// </summary>
public class BoxTileBoundableCover : BoxTileMap {
    private Point _margin;
    private Point _minimumSize;

    /// <summary>
    /// Gets or sets the margin.
    /// </summary>
    [DataMember]
    public Point Margin {
        get => this._margin;
        set {
            if (this.Set(ref this._margin, value)) {
                this.ResetSize();
            }
        }
    }

    /// <summary>
    /// Gets or sets the minimum size.
    /// </summary>
    [DataMember]
    public Point MinimumSize {
        get => this._minimumSize;
        set {
            if (this.Set(ref this._minimumSize, value)) {
                this.ResetSize();
            }
        }
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        if (this.Parent is IBoundableEntity boundable) {
            boundable.BoundingAreaChanged -= this.Parent_BoundingAreaChanged;
        }

        base.Deinitialize();

        this.TileSet.AssetChanged -= this.TileSet_AssetChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.TransformInheritance = TransformInheritance.None;
        this.Scene.Invoke(this.ResetSize);

        this.TileSet.AssetChanged += this.TileSet_AssetChanged;
        if (this.Parent is IBoundableEntity boundable) {
            boundable.BoundingAreaChanged += this.Parent_BoundingAreaChanged;
        }
    }

    private void Parent_BoundingAreaChanged(object? sender, EventArgs e) {
        this.ResetSize();
    }

    private void ResetSize() {
        if (this.Parent is IBoundableEntity { BoundingArea.IsEmpty: false } boundable) {
            var width = this._margin.X;
            var height = this._margin.Y;
            var boundingArea = boundable.BoundingArea;
            var spriteSize = this.GetSpriteUnitSize();

            if (spriteSize is { X: > 0f, Y: > 0f }) {
                var boundingWidth = boundingArea.Width;
                var boundingHeight = boundingArea.Height;

                while (boundingWidth > 0f) {
                    width++;
                    boundingWidth -= spriteSize.X;
                }

                while (boundingHeight > 0f) {
                    height++;
                    boundingHeight -= spriteSize.Y;
                }

                this.SetSizeWithMinimum(width, height);
                var desiredMinimum = boundingArea.Minimum - new Vector2(this._margin.X * spriteSize.X * 0.5f, this._margin.Y * spriteSize.X * 0.5f);
                var difference = desiredMinimum - this.BoundingArea.Minimum;
                this.Move(difference);
            }
            else {
                this.SetSize(0, 0);
            }
        }
        else {
            this.SetSize(0, 0);
        }
    }

    private void SetSizeWithMinimum(int width, int height) {
        this.SetSize(Math.Max(width, this.MinimumSize.X), Math.Max(height, this.MinimumSize.Y));
    }

    private void TileSet_AssetChanged(object? sender, bool e) {
        this.ResetSize();
    }
}