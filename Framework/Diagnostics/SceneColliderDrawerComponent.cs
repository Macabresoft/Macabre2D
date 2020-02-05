namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class SceneColliderDrawerComponent : BaseDrawerComponent {

        private readonly FilterSortCollection<IPhysicsBody> _bodies = new FilterSortCollection<IPhysicsBody>(
            r => r.IsEnabled,
            (r, handler) => r.IsEnabledChanged += handler,
            (r, handler) => r.IsEnabledChanged -= handler,
            (r1, r2) => Comparer<int>.Default.Compare(r1.UpdateOrder, r2.UpdateOrder),
            (r, handler) => r.UpdateOrderChanged += handler,
            (r, handler) => r.UpdateOrderChanged -= handler);

        public override BoundingArea BoundingArea { get; } = new BoundingArea(0.5f * float.MinValue, 0.5f * float.MaxValue);

        public override void Draw(GameTime gameTime, BoundingArea viewBoundingArea) {
            this._bodies.ForEachFilteredItem(body => {
                if (body.BoundingArea.Overlaps(viewBoundingArea)) {
                    var spriteBatch = MacabreGame.Instance.SpriteBatch;
                    var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                    var colliders = body.GetColliders();

                    foreach (var collider in colliders) {
                        if (collider is CircleCollider circle) {
                            this.PrimitiveDrawer.DrawCircle(spriteBatch, circle.ScaledRadius, circle.Center, 50, this.Color, lineThickness);
                        }
                        else if (collider is LineCollider line) {
                            this.PrimitiveDrawer.DrawLine(spriteBatch, line.WorldPoints.First(), point2: line.WorldPoints.Last(), color: this.Color, thickness: lineThickness);
                        }
                        else if (collider is PolygonCollider polygon) {
                            this.PrimitiveDrawer.DrawPolygon(spriteBatch, this.Color, lineThickness, polygon.WorldPoints);
                        }
                    }
                }
            });
        }

        protected override void Initialize() {
            base.Initialize();
            this._bodies.AddRange(this.Scene.GetAllComponentsOfType<IPhysicsBody>());
            this.Scene.ComponentCreated += this.Scene_ComponentAdded;
            this.Scene.ComponentDestroyed += this.Scene_ComponentRemoved;
        }

        private void Scene_ComponentAdded(object sender, BaseComponent e) {
            if (e is IPhysicsBody body) {
                this._bodies.Add(body);
            }
        }

        private void Scene_ComponentRemoved(object sender, BaseComponent e) {
            if (e is IPhysicsBody body) {
                this._bodies.Remove(body);
            }
        }
    }
}