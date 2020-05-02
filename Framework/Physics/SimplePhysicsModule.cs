namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A module which allows simple raycasting through colliders, which are sorted into a quad tree.
    /// </summary>
    /// <seealso cref="FixedTimeStepModule"/>
    public class SimplePhysicsModule : FixedTimeStepModule {

        /// <summary>
        /// Gets the bodies.
        /// </summary>
        /// <value>The bodies.</value>
        protected FilterSortCollection<IPhysicsBody> Bodies { get; } = new FilterSortCollection<IPhysicsBody>(
            r => r.IsEnabled,
            nameof(IPhysicsBody.IsEnabled),
            (r1, r2) => Comparer<int>.Default.Compare(r1.UpdateOrder, r2.UpdateOrder),
            nameof(IPhysicsBody.UpdateOrder));

        /// <summary>
        /// Gets the collider tree.
        /// </summary>
        /// <value>The collider tree.</value>
        protected QuadTree<Collider> ColliderTree { get; } = new QuadTree<Collider>(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

        /// <inheritdoc/>
        public override void FixedPostUpdate() {
            this.ColliderTree.Clear();
            this.Bodies.ForEachFilteredItem(r => this.ColliderTree.InsertMany(r.GetColliders()));
        }

        /// <inheritdoc/>
        public override void PostInitialize() {
            this.Bodies.Clear();
            this.Bodies.AddRange(this.Scene.GetAllComponentsOfType<IPhysicsBody>());
            this.Bodies.ForEachFilteredItem(r => this.ColliderTree.InsertMany(r.GetColliders()));
            this.Scene.ComponentCreated += this.Scene_ComponentAdded;
            this.Scene.ComponentDestroyed += this.Scene_ComponentRemoved;
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

            foreach (var collider in potentialColliders.Where(x => (x.Layers & layers) != Layers.None)) {
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
            var hits = this.RaycastAll(start, direction, distance, layers);

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
            if (e is IPhysicsBody body) {
                this.Bodies.Add(body);
            }
        }

        private void Scene_ComponentRemoved(object sender, BaseComponent e) {
            if (e is IPhysicsBody body) {
                this.Bodies.Remove(body);
            }
        }
    }
}