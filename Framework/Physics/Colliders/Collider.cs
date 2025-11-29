namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// Collider to be attached to bodies in the physics engine.
/// </summary>
[DataContract]
public abstract class Collider : PropertyChangedNotifier, IBoundable {
    /// <summary>
    /// The empty collider.
    /// </summary>
    public static readonly Collider Empty = new EmptyCollider();

    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly ResettableLazy<Vector2> _worldPosition;
    private Vector2 _offset;

    private Layers _overrideLayers = Layers.None;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="Collider" /> class.
    /// </summary>
    protected Collider() {
        this._worldPosition = new ResettableLazy<Vector2>(() => this.Body?.GetWorldPosition(this.Offset) ?? Vector2.Zero);
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <summary>
    /// Gets the body that this collider is attached to.
    /// </summary>
    public IPhysicsBody Body { get; private set; } = EmptyObject.Instance;

    /// <inheritdoc />
    public BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets or sets the layers.
    /// </summary>
    [DataMember(Name = "Collider Layers")]
    public Layers Layers {
        get => this._overrideLayers != Layers.None ? this._overrideLayers : this.Body?.Layers ?? Layers.None;
        set => this._overrideLayers = value;
    }

    /// <summary>
    /// Gets the offset.
    /// </summary>
    [DataMember]
    public Vector2 Offset {
        get => this._offset;

        set {
            if (this._offset != value) {
                this._offset = value;
                this.Reset();
            }
        }
    }

    /// <summary>
    /// Gets the world position.
    /// </summary>
    protected Vector2 WorldPosition => this._worldPosition.Value;

    /// <summary>
    /// Gets a value indicating whether this instance collides with the specified collider.
    /// </summary>
    /// <param name="other">The other collider.</param>
    /// <param name="collision">The collision.</param>
    /// <returns>A value indicating whether the collision occurred.</returns>
    public bool CollidesWith(Collider other, out CollisionEventArgs? collision) {
        if (!this.BoundingArea.Overlaps(other.BoundingArea)) {
            collision = null;
            return false;
        }

        var overlap = float.MaxValue;
        var smallestAxis = Vector2.Zero;

        var axes = new List<Vector2>(this.GetAxesForSat(other));
        axes.AddRange(other.GetAxesForSat(this));

        var thisContainsOther = true;
        var otherContainsThis = true;

        foreach (var axis in axes) {
            var projectionA = this.GetProjection(axis);
            var projectionB = other.GetProjection(axis);

            if (projectionA.OverlapsWith(projectionB)) {
                var currentOverlap = projectionA.GetOverlap(projectionB);

                var aContainsB = projectionA.Contains(projectionB);
                var bContainsA = projectionB.Contains(projectionA);

                // Information gathering for the collision
                if (!aContainsB) {
                    thisContainsOther = false;
                }

                if (!bContainsA) {
                    otherContainsThis = false;
                }

                if (aContainsB || bContainsA) {
                    var minimum = Math.Abs(projectionA.Minimum - projectionB.Minimum);
                    var maximum = Math.Abs(projectionA.Maximum - projectionB.Maximum);

                    if (minimum < maximum) {
                        currentOverlap += minimum;
                    }
                    else {
                        currentOverlap += maximum;
                    }
                }

                if (currentOverlap < overlap) {
                    overlap = currentOverlap;
                    smallestAxis = axis;
                }
            }
            else {
                // Gap was found, return false.
                collision = null;
                return false;
            }
        }

        var minimumTranslation = overlap * smallestAxis;

        // This is important to determine which direction this collider has to move to be
        // separated from the other collider.
        if (Vector2.Dot(this.GetCenter() - other.GetCenter(), minimumTranslation) < 0) {
            minimumTranslation = -minimumTranslation;
        }

        collision = new CollisionEventArgs(this, other, smallestAxis, minimumTranslation, thisContainsOther, otherContainsThis);
        return true;
    }

    /// <summary>
    /// Determines whether this collider contains the other collider.
    /// </summary>
    /// <param name="other">The other collider.</param>
    /// <returns><c>true</c> if this collider contains the other collider; otherwise, <c>false</c>.</returns>
    public abstract bool Contains(Collider other);

    /// <summary>
    /// Determines whether this collider contains the specified point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns><c>true</c> if this collider contains the specified point; otherwise, <c>false</c>.</returns>
    public abstract bool Contains(Vector2 point);

    /// <summary>
    /// Gets the center of the collider.
    /// </summary>
    /// <returns>The center of the collider.</returns>
    public abstract Vector2 GetCenter();

    /// <summary>
    /// Initializes this collider with the specified body.
    /// </summary>
    /// <param name="body">The body.</param>
    public void Initialize(IPhysicsBody body) {
        this.Body.TransformChanged -= this.Body_TransformChanged;
        this.Body = body;
        this.Body.TransformChanged += this.Body_TransformChanged;
        this.Reset();
    }

    /// <summary>
    /// Deinitializes this collider.
    /// </summary>
    public void Deinitialize() {
        this.Body.TransformChanged -= this.Body_TransformChanged;
        this.Body = EmptyObject.Instance;
        this.Reset();
    }

    /// <summary>
    /// Determines whether a bounding area intersects with this collider.
    /// </summary>
    /// <param name="boundingArea">The bounding area.</param>
    /// <param name="hits">The hits.</param>
    /// <returns>A value indicating whether an intersection occured.</returns>
    public virtual bool Intersects(BoundingArea boundingArea, out IEnumerable<RaycastHit> hits) {
        if (this.BoundingArea.Overlaps(boundingArea)) {
            var actualHits = new List<RaycastHit>();
            var lineSegments = this.GetLineSegments(boundingArea);

            foreach (var lineSegment in lineSegments) {
                if (this.IsHitBy(lineSegment, out var hit) && hit != RaycastHit.Empty) {
                    actualHits.Add(hit);
                }
            }

            if (actualHits.Any()) {
                hits = actualHits;
                return true;
            }
        }

        hits = [];
        return false;
    }

    /// <summary>
    /// Determines whether [is candidate for collision] [the specified other].
    /// </summary>
    /// <param name="other">The other collider.</param>
    /// <returns></returns>
    public bool IsCandidateForCollision(Collider other) => this.BoundingArea.Overlaps(other.BoundingArea);

    /// <summary>
    /// Determines whether this collider is hit by the specified ray.
    /// </summary>
    /// <param name="ray">The ray.</param>
    /// <param name="hit">The hit.</param>
    /// <returns><c>true</c> if this collider is hit by the specified ray; otherwise, <c>false</c>.</returns>
    public bool IsHitBy(LineSegment ray, out RaycastHit hit) {
        if (!ray.BoundingArea.Overlaps(this.BoundingArea)) {
            hit = RaycastHit.Empty;
            return false;
        }

        return this.TryHit(ray, out hit);
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public virtual void Reset() {
        this.ResetLazyFields();
    }

    /// <summary>
    /// Creates the bounding area.
    /// </summary>
    /// <returns>The created bounding area.</returns>
    protected abstract BoundingArea CreateBoundingArea();

    /// <summary>
    /// Gets the axes to test for the Separating Axis Theorem.
    /// </summary>
    /// <param name="other">The other collider.</param>
    /// <returns>The axes to test for the Separating Axis Theorem</returns>
    protected abstract IReadOnlyCollection<Vector2> GetAxesForSat(Collider other);

    /// <summary>
    /// Gets line segments from the edges of a bounding area.
    /// </summary>
    /// <param name="boundingArea">The bounding area.</param>
    /// <returns>The line segments from the edges of a bounding area.</returns>
    protected IEnumerable<LineSegment> GetLineSegments(BoundingArea boundingArea) {
        var bottomLeft = boundingArea.Minimum;
        var bottomRight = new Vector2(boundingArea.Maximum.X, boundingArea.Minimum.Y);
        var topLeft = new Vector2(boundingArea.Minimum.X, boundingArea.Maximum.Y);
        var topRight = boundingArea.Maximum;
        return [
            new LineSegment(bottomLeft, topLeft),
            new LineSegment(topLeft, topRight),
            new LineSegment(topRight, bottomRight),
            new LineSegment(bottomRight, bottomLeft)
        ];
    }

    /// <summary>
    /// Gets the normal from the line defined by two points.
    /// </summary>
    /// <param name="firstPoint">The first point.</param>
    /// <param name="secondPoint">The second point.</param>
    /// <returns>The normal.</returns>
    protected Vector2 GetNormal(Vector2 firstPoint, Vector2 secondPoint) {
        var edge = firstPoint - secondPoint;
        var normal = edge.GetPerpendicular();
        normal.Normalize();
        return normal;
    }

    /// <summary>
    /// Gets the projection of this collider onto the specified axis.
    /// </summary>
    /// <param name="axis">The axis.</param>
    /// <returns>The projection of this collider onto the specified axis.</returns>
    protected abstract Projection GetProjection(Vector2 axis);

    /// <summary>
    /// Resets the lazy fields.
    /// </summary>
    protected virtual void ResetLazyFields() {
        this._worldPosition.Reset();
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }

    /// <summary>
    /// Tries to process a hit between this collider and a ray.
    /// </summary>
    /// <param name="ray">The ray.</param>
    /// <param name="result">The result.</param>
    /// <returns>A value indicating whether a hit occurred.</returns>
    protected abstract bool TryHit(LineSegment ray, out RaycastHit result);

    private void Body_TransformChanged(object? sender, EventArgs e) {
        this.ResetLazyFields();
    }
}