namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A module which allows simple raycasting through colliders, which are sorted into a quad tree.
    /// </summary>
    /// <seealso cref="FixedTimeStepService" />
    public class SimplePhysicsModule : FixedTimeStepService {

        /// <summary>
        /// Gets the collider tree.
        /// </summary>
        /// <value>The collider tree.</value>
        protected QuadTree<Collider> ColliderTree { get; } = new QuadTree<Collider>(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

        /// <inheritdoc />
        public override void Initialize(IGameScene scene) {
            base.Initialize(scene);
            this.InsertColliders();
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

        /// <inheritdoc />
        protected override void FixedUpdate(FrameTime frameTime, InputState inputState) {
            this.InsertColliders();
        }

        private void InsertColliders() {
            this.ColliderTree.Clear();
            foreach (var body in this.Scene.PhysicsBodies) {
                this.ColliderTree.InsertMany(body.GetColliders());
            }
        }
    }
}