namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework;

    /// <summary>
    /// Draws bounding areas from colliders for debugging purposes.
    /// </summary>
    public sealed class BoundingAreaDrawerComponent : BaseDrawerComponent {

        /// <summary>
        /// Gets or sets the boundable.
        /// </summary>
        /// <value>The boundable.</value>
        public IBoundable Boundable { get; set; }

        /// <inheritdoc/>
        public override BoundingArea BoundingArea {
            get {
                if (this.Boundable != null) {
                    return this.Boundable.BoundingArea;
                }
                else if (this.Parent is IBoundable boundable) {
                    return boundable.BoundingArea;
                }

                return new BoundingArea();
            }
        }

        /// <inheritdoc/>
        public override void Draw(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer == null || this.LineThickness <= 0f || this.Color == Color.Transparent || this.BoundingArea.Maximum == this.BoundingArea.Minimum) {
                return;
            }

            var boundingArea = this.BoundingArea;
            if (!boundingArea.IsEmpty) {
                var minimum = boundingArea.Minimum;
                var maximum = boundingArea.Maximum;
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);

                var points = new Vector2[] { minimum, new Vector2(minimum.X, maximum.Y), maximum, new Vector2(maximum.X, minimum.Y) };
                this.PrimitiveDrawer.DrawPolygon(MacabreGame.Instance.SpriteBatch, this.Color, lineThickness, points);
            }
        }
    }
}