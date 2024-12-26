namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A <see cref="BoxTileMap"/> that automatically sizes to contain its contents.
/// </summary>
public class BoxTileContainer : BoxTileMap {
    private readonly List<IBoundable> _boundablechildren = [];

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._boundablechildren.AddRange(this.GetDescendants<IBoundable>());
        this.ResetSize();
    }

    /// <inheritdoc />
    protected override void OnAddChild(IEntity child) {
        base.OnAddChild(child);

        if (child is IBoundable boundable) {
            this._boundablechildren.Add(boundable);
            this.ResetSize();
        }
    }

    /// <inheritdoc />
    protected override void OnRemoveChild(IEntity child) {
        base.OnRemoveChild(child);

        if (child is IBoundable boundable) {
            this._boundablechildren.Remove(boundable);
            this.ResetSize();
        }
    }

    private void ResetSize() {
        var width = 0;
        var height = 0;
        var totalBoundingArea = this._boundablechildren
            .Select(x => x.BoundingArea)
            .Aggregate(BoundingArea.Empty, (current, boundingArea) => current.Combine(boundingArea));

        if (!totalBoundingArea.IsEmpty) {
            var spriteSize = this.GetSpriteUnitSize();

            if (spriteSize is { X: > 0f, Y: > 0f }) {
                var boundingWidth = totalBoundingArea.Width;
                var boundingHeight = totalBoundingArea.Height;

                while (boundingWidth > 0f) {
                    width++;
                    boundingWidth -= spriteSize.X;
                }

                while (boundingHeight > 0f) {
                    height++;
                    boundingHeight -= spriteSize.Y;
                }
            }
        }

        this.SetSize(height, width);
    }
}