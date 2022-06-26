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
    /// Performs a raycast across colliders in the scene, returning all collisions in its path.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="distance">The distance.</param>
    /// <param name="layers">The layers.</param>
    /// <returns>Any hits that occurred during the raycast.</returns>
    IReadOnlyList<RaycastHit> RaycastAll(Vector2 start, Vector2 direction, float distance, Layers layers);

    /// <summary>
    /// Performs a box cast. Returns all collision points as well as any vertices of the collider that reside within the <see cref="BoundingArea" /> provided.
    /// </summary>
    /// <param name="box">The box as a <see cref="BoundingArea" />.</param>
    /// <param name="layers">The layers for which to cast.</param>
    /// <param name="intersections">The intersections found.</param>
    /// <returns>A value indicating whether or not any intersections were found.</returns>
    bool TryBoxCast(BoundingArea box, Layers layers, out IReadOnlyCollection<Vector2> intersections);

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
    public bool TryBoxCast(BoundingArea box, Layers layers, out IReadOnlyCollection<Vector2> intersections) {
        var actualIntersections = new List<Vector2>();
        foreach (var collider in this.GetFilteredColliders(box, layers)) {
            if (collider.Intersects(box, out var colliderIntersections)) {
                actualIntersections.AddRange(colliderIntersections);
            }
        }

        intersections = actualIntersections;
        return actualIntersections.Any();
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