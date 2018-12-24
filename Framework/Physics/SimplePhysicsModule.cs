namespace Macabre2D.Framework.Physics {

    using Microsoft.Xna.Framework;
    using System.Collections.Generic;

    /// <summary>
    /// A module which allows simple raycasting through colliders, which are sorted into a quad tree.
    /// </summary>
    /// <seealso cref="FixedTimeStepModule"/>
    public class SimplePhysicsModule : FixedTimeStepModule {

        /// <summary>
        /// Gets the bodies.
        /// </summary>
        /// <value>The bodies.</value>
        protected FilterSortCollection<Body> Bodies { get; } = new FilterSortCollection<Body>(
            r => r.IsEnabled,
            (r, handler) => r.IsEnabledChanged += handler,
            (r, handler) => r.IsEnabledChanged -= handler,
            (r1, r2) => Comparer<int>.Default.Compare(r1.UpdateOrder, r2.UpdateOrder),
            (r, handler) => r.UpdateOrderChanged += handler,
            (r, handler) => r.UpdateOrderChanged -= handler);

        /// <summary>
        /// Gets the collider tree.
        /// </summary>
        /// <value>The collider tree.</value>
        protected QuadTree<Collider> ColliderTree { get; } = new QuadTree<Collider>(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

        /// <inheritdoc/>
        public override void FixedPostUpdate() {
            this.ColliderTree.Clear();
            this.Bodies.ForEachFilteredItem(r => this.ColliderTree.Insert(r.Collider));
        }

        /// <inheritdoc/>
        public override void PostInitialize() {
            this.Bodies.AddRange(this.Scene.GetAllComponentsOfType<Body>());

            this.Scene.ComponentAdded += this.Scene_ComponentAdded;
            this.Scene.ComponentRemoved += this.Scene_ComponentRemoved;
        }

        /// <summary>
        /// Performs a raycast across colliders in the scene, returning all collisions in its path.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="layers">The layers.</param>
        /// <returns>Any hits that occurred during the raycast.</returns>
        public List<RaycastHit> RaycastAll(Vector2 start, Vector2 direction, float distance, Layers layers) {
            var ray = new LineSegment(start, direction, distance);
            var potentialColliders = this.ColliderTree.RetrievePotentialCollisions(ray.BoundingArea);
            var hits = new List<RaycastHit>();

            foreach (var collider in potentialColliders) {
                if (collider.IsHitBy(ray, out var hit)) {
                    hits.Add(hit);
                }
            }

            return hits;
        }

        /// <summary>
        /// Performs a raycast across colliders in the scene, but stops after the first collision.
        /// Performs a raycast across colliders in the scene.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="layers">The layers.</param>
        /// <param name="hit">The hit.</param>
        /// <returns>A value indicating whether or not anything was hit.</returns>
        public bool TryRaycast(Vector2 start, Vector2 direction, float distance, Layers layers, out RaycastHit hit) {
            var hits = RaycastAll(start, direction, distance, layers);

            if (hits.Count == 0) {
                hit = null;
                return false;
            }

            hit = hits[0];
            var shortestDistance = Vector2.Distance(start, hit.ContactPoint);

            for (var i = 1; i < hits.Count; i++) {
                var newHit = hits[i];
                var currentDistance = Vector2.Distance(start, newHit.ContactPoint);
                if (currentDistance < shortestDistance) {
                    hit = newHit;
                    shortestDistance = currentDistance;
                }
            }

            return true;
        }

        private void Scene_ComponentAdded(object sender, BaseComponent e) {
            if (e is Body body) {
                this.Bodies.Add(body);
            }
        }

        private void Scene_ComponentRemoved(object sender, BaseComponent e) {
            if (e is Body body) {
                this.Bodies.Remove(body);
            }
        }
    }
}