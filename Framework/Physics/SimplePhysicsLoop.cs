namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Macabresoft.Macabre2D.Project.Common;
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
    /// Checks if any colliders with the specified layers overlap with the provided <see cref="BoundingArea" />.
    /// </summary>
    /// <param name="boundingArea">The bounding area.</param>
    /// <param name="layers">The layers.</param>
    /// <returns>A value indicating whether or not there was a hit.</returns>
    bool TryBoundingAreaCast(BoundingArea boundingArea, Layers layers);

    /// <summary>
    /// Checks if any colliders with the specified layers have bounding areas that overlap with the provided <see cref="BoundingArea" />.
    /// </summary>
    /// <param name="boundingArea">The bounding area.</param>
    /// <param name="layers">The layers.</param>
    /// <returns>A value indicating whether or not there was a hit.</returns>
    bool TryBoundingAreaToBoundingAreaCast(BoundingArea boundingArea, Layers layers);

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

    /// <summary>
    /// Performs a raycast across colliders' bounding areas in the scene, but stops after the first collision.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="direction">The direction.</param>
    /// <param name="distance">The distance.</param>
    /// <param name="layers">The layers.</param>
    /// <param name="hitEntity">The hit entity.</param>
    /// <returns>A value indicating whether or not anything was hit.</returns>
    bool TryRaycastToBoundingArea(Vector2 start, Vector2 direction, float distance, Layers layers, [NotNullWhen(true)] out IEntity? hitEntity);
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
    protected QuadTree<Collider> ColliderTree { get; private set; } = QuadTree<Collider>.Default;

    /// <inheritdoc />
    public virtual IReadOnlyCollection<RaycastHit> BoundingAreaCastAll(BoundingArea boundingArea, Layers layers) {
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
        if (!Framework.Scene.IsNullOrEmpty(this.Scene)) {
            this.Scene.PropertyChanged -= this.Scene_PropertyChanged;
        }

        base.Initialize(scene);

        if (!Framework.Scene.IsNullOrEmpty(this.Scene)) {
            this.Scene.PropertyChanged += this.Scene_PropertyChanged;
        }

        this.ResetTree();
    }

    /// <inheritdoc />
    public virtual IReadOnlyList<RaycastHit> RaycastAll(Vector2 start, Vector2 direction, float distance, Layers layers) {
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
    public bool TryBoundingAreaCast(BoundingArea boundingArea, Layers layers) {
        foreach (var collider in this.GetFilteredColliders(boundingArea, layers)) {
            if (collider.Intersects(boundingArea, out _)) {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public bool TryBoundingAreaToBoundingAreaCast(BoundingArea boundingArea, Layers layers) {
        return !boundingArea.IsEmpty && this.GetFilteredColliders(boundingArea, layers).Any(x => !x.BoundingArea.IsEmpty && x.BoundingArea.Overlaps(boundingArea));
    }

    /// <inheritdoc />
    public virtual bool TryRaycast(Vector2 start, Vector2 direction, float distance, Layers layers, out RaycastHit hit) {
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
    public bool TryRaycastToBoundingArea(Vector2 start, Vector2 direction, float distance, Layers layers, [NotNullWhen(true)] out IEntity? hitEntity) {
        hitEntity = null;
        var ray = new LineSegment(start, direction, distance);

        if (!ray.BoundingArea.IsEmpty) {
            var collider = this.GetFilteredColliders(ray.BoundingArea, layers).FirstOrDefault(x => !x.BoundingArea.IsEmpty && x.BoundingArea.Overlaps(ray.BoundingArea));
            hitEntity = collider?.Body;
        }

        return hitEntity != null;
    }

    /// <inheritdoc />
    protected override void FixedUpdate(FrameTime frameTime, InputState inputState) {
        this.InsertColliders();
    }

    private IEnumerable<Collider> GetFilteredColliders(BoundingArea boundingArea, Layers layers) {
        var enabledLayers = this.Game.Project.LayerSettings.EnabledLayers;
        return this.ColliderTree.RetrievePotentialCollisions(boundingArea).Where(x => (x.Layers & layers & enabledLayers) != Layers.None);
    }

    private void InsertColliders() {
        this.ColliderTree.Clear();

        if (this.Scene.BoundingArea.IsEmpty) {
            foreach (var body in this.Scene.PhysicsBodies) {
                this.ColliderTree.InsertMany(body.GetColliders());
            }
        }
        else {
            foreach (var body in this.Scene.PhysicsBodies) {
                this.ColliderTree.InsertMany(body.GetColliders().Where(x => x.BoundingArea.Overlaps(this.Scene.BoundingArea)));
            }
        }
    }

    private void ResetTree() {
        this.ColliderTree.Clear();

        if (this.Scene.BoundingArea.IsEmpty) {
            this.ColliderTree = QuadTree<Collider>.Default;
        }
        else {
            this.ColliderTree = new QuadTree<Collider>(
                0,
                this.Scene.BoundingArea.Minimum.X,
                this.Scene.BoundingArea.Minimum.Y,
                this.Scene.BoundingArea.Width,
                this.Scene.BoundingArea.Height);
        }

        this.InsertColliders();
    }

    private void Scene_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IScene.BoundingArea)) {
            this.ResetTree();
        }
    }
}