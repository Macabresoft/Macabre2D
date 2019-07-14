namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Collections.Generic;

    /// <summary>
    /// Draws a grid for the specified camera.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.Diagnostics.BaseDrawer"/>
    public sealed class GridDrawer : BaseDrawer {
        private Camera _camera;
        private float _columnWidth = 1f;
        private float _rowHeight = 1f;

        /// <inheritdoc/>
        public override BoundingArea BoundingArea {
            get {
                if (this.Camera is Camera camera) {
                    return camera.BoundingArea;
                }

                return new BoundingArea();
            }
        }

        /// <summary>
        /// Gets or sets the camera.
        /// </summary>
        /// <value>The camera.</value>
        public Camera Camera {
            get {
                return this._camera ?? this.Parent as Camera;
            }

            set {
                this._camera = value;
            }
        }

        /// <summary>
        /// Gets or sets the grid.
        /// </summary>
        /// <value>The grid.</value>
        public TileGrid Grid { get; set; }

        /// <inheritdoc/>
        public override void Draw(GameTime gameTime, BoundingArea viewBoundingArea) {
            var spriteBatch = MacabreGame.Instance.SpriteBatch;
            var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
            var boundingArea = this.BoundingArea;

            var columns = GridDrawer.GetGridPositions(boundingArea.Minimum.X, boundingArea.Maximum.X, this.Grid.TileSize.X, this.Grid.Offset.X);
            foreach (var column in columns) {
                this.PrimitiveDrawer.DrawLine(
                    spriteBatch,
                    new Vector2(column, boundingArea.Minimum.Y),
                    new Vector2(column, boundingArea.Maximum.Y),
                    this.Color,
                    lineThickness);
            }

            var rows = GridDrawer.GetGridPositions(boundingArea.Minimum.Y, boundingArea.Maximum.Y, this.Grid.TileSize.Y, this.Grid.Offset.Y);
            foreach (var row in rows) {
                this.PrimitiveDrawer.DrawLine(
                    spriteBatch,
                    new Vector2(boundingArea.Minimum.X, row),
                    new Vector2(boundingArea.Maximum.X, row),
                    this.Color,
                    lineThickness);
            }
        }

        private static List<float> GetGridPositions(float lowerLimit, float upperLimit, float stepSize, float offset) {
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
    }
}