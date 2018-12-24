namespace Macabre2D.Framework.Diagnostics {

    using Microsoft.Xna.Framework;
    using Physics;
    using System.Linq;

    /// <summary>
    /// Draws colliders (any game ob
    /// </summary>
    /// <seealso cref="BaseComponent"/>
    /// <seealso cref="IDrawableComponent"/>
    public sealed class ColliderDrawer : BaseDrawer {

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        public Body Body { get; set; }

        /// <inheritdoc/>
        public override BoundingArea BoundingArea {
            get {
                if (this.Body != null) {
                    return this.Body.BoundingArea;
                }
                if (this.Parent is Body body) {
                    return body.BoundingArea;
                }

                return new BoundingArea();
            }
        }

        /// <inheritdoc/>
        public override void Draw(GameTime gameTime, float viewHeight) {
            var body = this.Body ?? this.Parent as Body;
            if (body != null) {
                var spriteBatch = this._scene.Game.SpriteBatch;
                var lineThickness = this.GetLineThickness(viewHeight);

                if (body.Collider is CircleCollider circle) {
                    this.PrimitiveDrawer.DrawCircle(spriteBatch, circle.ScaledRadius, circle.Center, 50, this.Color, lineThickness);
                }
                else if (body.Collider is LineCollider line) {
                    this.PrimitiveDrawer.DrawLine(spriteBatch, line.WorldPoints.First(), line.WorldPoints.Last(), this.Color, lineThickness);
                }
                else if (body.Collider is PolygonCollider polygon) {
                    this.PrimitiveDrawer.DrawPolygon(spriteBatch, this.Color, lineThickness, polygon.WorldPoints);
                }
            }
        }
    }
}