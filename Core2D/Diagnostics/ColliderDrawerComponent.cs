namespace Macabresoft.MonoGame.Core2D {

    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Draws a collider.
    /// </summary>
    public sealed class ColliderDrawerComponent : BaseDrawerComponent, IGameUpdateableComponent {
        private readonly List<IPhysicsBody> _bodies = new List<IPhysicsBody>();
        private BoundingArea _boundingArea;

        /// <inheritdoc />
        public override BoundingArea BoundingArea => this._boundingArea;

        /// <inheritdoc />
        public int UpdateOrder => 0;

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer != null && this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch && this._bodies.Any()) {
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);

                foreach (var body in this._bodies) {
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

        public void Update(FrameTime frameTime, InputState inputState) {
            this._bodies.Clear();
            this._bodies.AddRange(this.Entity.Components.OfType<IPhysicsBody>().Where(x => !x.BoundingArea.IsEmpty).ToList());
            this._boundingArea = BoundingArea.Combine(this._bodies.Select(x => x.BoundingArea).ToArray());
        }
    }
}