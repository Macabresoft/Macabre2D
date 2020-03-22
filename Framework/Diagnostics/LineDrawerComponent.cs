namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Draws a line.
    /// </summary>
    public sealed class LineDrawerComponent : BaseDrawerComponent {

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
        public override void Draw(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer != null && this.StartPoint != this.EndPoint && MacabreGame.Instance.SpriteBatch is SpriteBatch spriteBatch) {
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                this.PrimitiveDrawer.DrawLine(spriteBatch, this.StartPoint, this.EndPoint, this.Color, lineThickness);
            }
        }
    }
}