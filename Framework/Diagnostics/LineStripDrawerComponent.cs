namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Draws a line strip.
    /// </summary>
    public sealed class LineStripDrawerComponent : BaseDrawerComponent {
        private readonly List<Vector2> _points = new List<Vector2>();
        private BoundingArea _boundingArea;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStripDrawerComponent"/> class.
        /// </summary>
        public LineStripDrawerComponent() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineStripDrawerComponent"/> class.
        /// </summary>
        /// <param name="points">The points.</param>
        public LineStripDrawerComponent(IEnumerable<Vector2> points) {
            this.ResetPoints(points);
        }

        /// <inheritdoc/>
        public override BoundingArea BoundingArea {
            get {
                return this._boundingArea;
            }
        }

        /// <summary>
        /// Gets the points to draw. These are represented in world coordinates.
        /// </summary>
        /// <value>The points.</value>
        public IReadOnlyCollection<Vector2> Points {
            get {
                return this._points;
            }
        }

        /// <inheritdoc/>
        public override void Draw(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer != null && this._points.Any()) {
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                this.PrimitiveDrawer.DrawLineStrip(MacabreGame.Instance.SpriteBatch, this.Color, lineThickness, this.Points.ToArray());
            }
        }

        /// <summary>
        /// Clears the collection of points and replaces it with the newly provided points.
        /// </summary>
        /// <param name="points">The points.</param>
        public void ResetPoints(IEnumerable<Vector2> points) {
            this._points.Clear();
            this._points.AddRange(points);
            this._boundingArea = this.CreateBoundingArea();
        }

        private BoundingArea CreateBoundingArea() {
            return new BoundingArea(
                this.Points.Min(p => p.X),
                this.Points.Max(p => p.X),
                this.Points.Min(p => p.Y),
                this.Points.Max(p => p.Y));
        }
    }
}