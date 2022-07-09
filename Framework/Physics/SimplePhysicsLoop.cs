namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for a system which allows simple raycasting through colliders.
/// </summary>
public interface ISimplePhysicsLoop : ILoop {
    /// <summary>
    /// Performs an operation similar to a raycast, but with a bounding area instead of a ray. Returns all collision points as well as any vertices of the collider that reside within the <see cref="BoundingArea" /> provided.
    /// </summary>
    /// <param name="boundingArea">The <see cref="BoundingArea" /> to cast.</param>
    /// <param name="layers">The layers for which to cast.</param>
    /// <returns>Any hits that occurred.</returns>
    IReadOnlyCollection<RaycastHit> BoundingAreaCastAll(BoundingArea boundingArea, Layers layers);

    /// <summary>
    /// Performs a raycast across colliders in the scene, returning all collisions in its path.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="distance">The distance.</param>
    /// <param name="layers">The layers.</param>
    /// <returns>Any hits that occurred during the raycast.</returns>
    IReadOnlyList<RaycastHit> RaycastAll(Vector2 start, Vector2 direction, float distance, Layers layers);

    /// <summary>
    /// Performs a raycast across colliders in the scene, but stops after the first collision.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="distance">The distance.</param>
    /// <param name="layers">The layers.</param>
    /// <param name="hit">The hit.</param>
    /// <returns>A value indicating whether or not anything was hit.</returns>
    bool TryRaycast(Vector2 start, Vector2 direction, float distance, Layers layers, out RaycastHit hit);
}

/// <summary>
/// A system which allows simple raycasting through colliders.
/// </summary>
[Category(CommonCategories.Physics)]
public class SimplePhysicsLoop : FixedTimeStepLoop, ISimplePhysicsLoop {
    /// <summary>
    /// Gets the collider tree.
    /// </summary>
    /// <value>The collider tree.</value>
    protected QuadTree<Collider> ColliderTree { get; } = new(0, float.MinValue * 0.5f, float.MinValue * 0.5f, float.MaxValue, float.MaxValue);

    /// <inheritdoc />
    public IReadOnlyCollection<RaycastHit> BoundingAreaCastAll(BoundingArea boundingArea, Layers layers) {
        var hits = new List<RaycastHit>();
        foreach (var collider in this.GetFilteredColliders(boundingArea, layers)) {
            if (collider.Intersects(boundingArea, out var colliderHits)) {
                hits.AddRange(colliderHits);
            }
        }

        return hits;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene) {
        base.Initialize(scene);
        this.InsertColliders();
    }

    /// <inheritdoc />
    public IReadOnlyList<RaycastHit> RaycastAll(Vector2 start, Vector2 direction, float distance, Layers layers) {
        var ray = new LineSegment(start, direction, distance);
        var hits = new List<RaycastHit>();

        foreach (var collider in this.GetFilteredColliders(ray.BoundingArea, layers)) {
            if (collider.IsHitBy(ray, out var hit)) {
                hits.Add(hit);
            }
        }

        return hits;
    }

    /// <inheritdoc />
    public bool TryRaycast(Vector2 start, Vector2 direction, float distance, Layers layers, out RaycastHit hit) {
        var hits = this.RaycastAll(start, direction, distance, layers);

        if (hits.Count == 0) {
            hit = RaycastHit.Empty;
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

    private IEnumerable<Collider> GetFilteredColliders(BoundingArea boundingArea, Layers layers) {
        var enabledLayers = this.Game.Project.Settings.LayerSettings.EnabledLayers;
        return this.ColliderTree.RetrievePotentialCollisions(boundingArea).Where(x => (x.Layers & layers & enabledLayers) != Layers.None);
    }

    private void InsertColliders() {
        this.ColliderTree.Clear();
        foreach (var body in this.Scene.PhysicsBodies) {
            this.ColliderTree.InsertMany(body.GetColliders());
        }
    }
}