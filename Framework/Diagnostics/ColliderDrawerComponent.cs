namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Draws a collider.
    /// </summary>
    [Display(Name = "Collider Drawer (Diagnostics)")]
    public sealed class ColliderDrawerComponent : BaseDrawerComponent, IGameUpdateableComponent {
        private readonly List<IPhysicsBodyComponent> _bodies = new();

        /// <inheritdoc />
        public override BoundingArea BoundingArea => this._bodies.Any() ? BoundingArea.Combine(this._bodies.Select(x => x.BoundingArea).ToArray()) : BoundingArea.Empty;

        /// <inheritdoc />
        public int UpdateOrder => 0;

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);

            this.ResetBodies();
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer != null && this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch && this._bodies.Any()) {
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);

                foreach (var body in this._bodies) {
                    var colliders = body.GetColliders();

                    foreach (var collider in colliders) {
                        if (collider is CircleCollider circle) {
                            this.PrimitiveDrawer.DrawCircle(
                                spriteBatch,
                                this.Entity.Scene.Game.Project.Settings.PixelsPerUnit,
                                circle.ScaledRadius,
                                circle.Center,
                                50, 
                                this.Color,
                                lineThickness);
                        }
                        else if (collider is LineCollider line) {
                            this.PrimitiveDrawer.DrawLine(
                                spriteBatch, 
                                this.Entity.Scene.Game.Project.Settings.PixelsPerUnit,
                                line.WorldPoints.First(), 
                                line.WorldPoints.Last(), 
                                this.Color, 
                                lineThickness);
                        }
                        else if (collider is PolygonCollider polygon) {
                            this.PrimitiveDrawer.DrawPolygon(
                                spriteBatch, 
                                this.Entity.Scene.Game.Project.Settings.PixelsPerUnit, 
                                this.Color, 
                                lineThickness, 
                                polygon.WorldPoints);
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Update(FrameTime frameTime, InputState inputState) {
            this.ResetBodies();
        }

        private void ResetBodies() {
            this._bodies.Clear();
            this._bodies.AddRange(this.Entity.Components.OfType<IPhysicsBodyComponent>().Where(x => !x.BoundingArea.IsEmpty).ToList());
        }
    }
}