namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;

    /// <summary>
    /// Draws a circle.
    /// </summary>
    public sealed class CircleDrawer : BaseDrawer {
        private int _complexity;

        /// <inheritdoc/>
        public override BoundingArea BoundingArea {
            get {
                return new BoundingArea(-this.Radius, this.Radius);
            }
        }

        /// <summary>
        /// Gets or sets the complexity. This value determines the smoothness of the circle's edges.
        /// A larger value will look better, but perform worse.
        /// </summary>
        /// <remarks>
        /// Complexity is code for 'number of edges'. In reality, we can't make a perfect circle with
        /// pixels or polygons, so this is us faking it as usual. This value must be at least 3.
        /// </remarks>
        /// <value>The complexity.</value>
        public int Complexity {
            get {
                return this._complexity;
            }

            set {
                if (value < 3) {
                    value = 3;
                }

                this._complexity = value;
            }
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        public float Radius { get; set; }

        /// <inheritdoc/>
        public override void Draw(GameTime gameTime, BoundingArea viewBoundingArea) {
            if (this.Radius > 0f) {
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                this.PrimitiveDrawer.DrawCircle(MacabreGame.Instance.SpriteBatch, this.Radius, this.WorldTransform.Position, this.Complexity, this.Color, lineThickness);
            }
        }
    }
}