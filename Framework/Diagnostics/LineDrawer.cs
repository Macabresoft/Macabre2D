namespace Macabre2D.Framework.Diagnostics {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Draws a line.
    /// </summary>
    public sealed class LineDrawer : BaseDrawer {

        /// <inheritdoc/>
        public override BoundingArea BoundingArea {
            get {
                return new BoundingArea(Vector2.Min(this.StartPoint, this.EndPoint), Vector2.Max(this.StartPoint, this.EndPoint));
            }
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>The end point.</value>
        public Vector2 EndPoint { get; set; }

        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        /// <value>The start point.</value>
        public Vector2 StartPoint { get; set; }

        /// <inheritdoc/>
        public override void Draw(GameTime gameTime, float viewHeight) {
            if (this.StartPoint != this.EndPoint && this._scene?.Game?.SpriteBatch is SpriteBatch spriteBatch) {
                var lineThickness = this.GetLineThickness(viewHeight);
                this.PrimitiveDrawer.DrawLine(spriteBatch, this.StartPoint, this.EndPoint, this.Color, lineThickness);
            }
        }
    }
}