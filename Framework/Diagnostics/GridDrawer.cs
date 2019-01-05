namespace Macabre2D.Framework.Diagnostics {

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
        /// Gets or sets the width of the column.
        /// </summary>
        /// <value>The width of the column.</value>
        public float ColumnWidth {
            get {
                return this._columnWidth;
            }

            set {
                if (value < 0f) {
                    value = 0f;
                }

                this._columnWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of the row.
        /// </summary>
        /// <value>The height of the row.</value>
        public float RowHeight {
            get {
                return this._rowHeight;
            }

            set {
                if (value < 0f) {
                    value = 0f;
                }

                this._rowHeight = value;
            }
        }

        /// <inheritdoc/>
        public override void Draw(GameTime gameTime, float viewHeight) {
            if (this.Camera is Camera camera) {
                var spriteBatch = this._scene.Game.SpriteBatch;
                var lineThickness = this.GetLineThickness(viewHeight);
                var boundingArea = this.BoundingArea;

                var columns = GridDrawer.GetGridPositions(boundingArea.Minimum.X, boundingArea.Maximum.X, this.ColumnWidth);
                foreach (var column in columns) {
                    this.PrimitiveDrawer.DrawLine(
                        spriteBatch,
                        new Vector2(column, boundingArea.Minimum.Y),
                        new Vector2(column, boundingArea.Maximum.Y),
                        this.Color,
                        lineThickness);
                }

                var rows = GridDrawer.GetGridPositions(boundingArea.Minimum.Y, boundingArea.Maximum.Y, this.RowHeight);
                foreach (var row in rows) {
                    this.PrimitiveDrawer.DrawLine(
                        spriteBatch,
                        new Vector2(boundingArea.Minimum.X, row),
                        new Vector2(boundingArea.Maximum.X, row),
                        this.Color,
                        lineThickness);
                }
            }
        }

        private static List<float> GetGridPositions(float lowerLimit, float upperLimit, float stepSize) {
            var result = new List<float>();
            var currentPosition = 0f;

            if (currentPosition < lowerLimit) {
                while (currentPosition + stepSize < lowerLimit) {
                    currentPosition += stepSize;
                }
            }
            else if (currentPosition > lowerLimit) {
                while (currentPosition - stepSize > lowerLimit) {
                    currentPosition -= stepSize;
                }
            }

            while (currentPosition <= upperLimit) {
                result.Add(currentPosition);
                currentPosition += stepSize;
            }

            return result;
        }
    }
}