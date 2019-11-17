namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Linq;

    /// <summary>
    /// Draws a collider.
    /// </summary>
    public sealed class ColliderDrawerComponent : BaseDrawerComponent {

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        public IPhysicsBody Body { get; set; }

        /// <inheritdoc/>
        public override BoundingArea BoundingArea {
            get {
                if (this.Body != null) {
                    return this.Body.BoundingArea;
                }
                if (this.Parent is IPhysicsBody body) {
                    return body.BoundingArea;
                }

                return new BoundingArea();
            }
        }

        /// <inheritdoc/>
        public override void Draw(GameTime gameTime, BoundingArea viewBoundingArea) {
            var body = this.Body ?? this.Parent as IPhysicsBody;
            if (body != null && this.PrimitiveDrawer != null) {
                var spriteBatch = MacabreGame.Instance.SpriteBatch;
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                var colliders = body.GetColliders();

                foreach (var collider in colliders) {
                    if (collider is CircleCollider circle) {
                        this.PrimitiveDrawer.DrawCircle(spriteBatch, circle.ScaledRadius, circle.Center, 50, this.Color, lineThickness);
                    }
                    else if (collider is LineCollider line) {
                        this.PrimitiveDrawer.DrawLine(spriteBatch, line.WorldPoints.First(), line.WorldPoints.Last(), this.Color, lineThickness);
                    }
                    else if (collider is PolygonCollider polygon) {
                        this.PrimitiveDrawer.DrawPolygon(spriteBatch, this.Color, lineThickness, polygon.WorldPoints);
                    }
                }
            }
        }
    }
}