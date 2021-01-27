namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Draws a grid for the specified camera.
    /// </summary>
    [Display(Name = "Grid Drawer (Diagnostics)")]
    public sealed class GridDrawerComponent : BaseDrawerComponent {

        /// <inheritdoc />
        public override BoundingArea BoundingArea => BoundingArea.MaximumSize;

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public TileGrid Grid { get; set; }

        /// <summary>
        /// Gets the grid positions.
        /// </summary>
        /// <param name="lowerLimit">The lower limit.</param>
        /// <param name="upperLimit">The upper limit.</param>
        /// <param name="stepSize">Size of the step.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The grid positions.</returns>
        public static IEnumerable<float> GetGridPositions(float lowerLimit, float upperLimit, float stepSize, float offset) {
            var result = new List<float>();

            if (stepSize > 0f) {
                if (offset < lowerLimit) {
                    while (offset + stepSize < lowerLimit) {
                        offset += stepSize;
                    }
                }
                else if (offset > lowerLimit) {
                    while (offset - stepSize > lowerLimit) {
                        offset -= stepSize;
                    }
                }

                while (offset <= upperLimit) {
                    result.Add(offset);
                    offset += stepSize;
                }
            }

            return result;
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer != null && this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);

                var columns = GridDrawerComponent.GetGridPositions(viewBoundingArea.Minimum.X, viewBoundingArea.Maximum.X, this.Grid.TileSize.X, this.Grid.Offset.X);
                foreach (var column in columns) {
                    this.PrimitiveDrawer.DrawLine(
                        spriteBatch,
                        this.Entity.Scene.Game.Project.Settings.PixelsPerUnit,
                        new Vector2(column, viewBoundingArea.Minimum.Y),
                        new Vector2(column, viewBoundingArea.Maximum.Y),
                        this.Color,
                        lineThickness);
                }

                var rows = GridDrawerComponent.GetGridPositions(viewBoundingArea.Minimum.Y, viewBoundingArea.Maximum.Y, this.Grid.TileSize.Y, this.Grid.Offset.Y);
                foreach (var row in rows) {
                    this.PrimitiveDrawer.DrawLine(
                        spriteBatch,
                        this.Entity.Scene.Game.Project.Settings.PixelsPerUnit,
                        new Vector2(viewBoundingArea.Minimum.X, row),
                        new Vector2(viewBoundingArea.Maximum.X, row),
                        this.Color,
                        lineThickness);
                }
            }
        }
    }
}