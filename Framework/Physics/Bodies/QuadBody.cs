namespace Macabre2D.Framework;

using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

/// <summary>
/// A base for quad bodies.
/// </summary>
public abstract class QuadBody : PhysicsBody {
    /// <summary>
    /// Initializes a new instance of <see cref="QuadBody" />.
    /// </summary>
    protected QuadBody() : base() {
        this.OverrideLayersBottomEdge.PropertyChanged += this.OnLayerOverrideChanged;
        this.OverrideLayersLeftEdge.PropertyChanged += this.OnLayerOverrideChanged;
        this.OverrideLayersRightEdge.PropertyChanged += this.OnLayerOverrideChanged;
        this.OverrideLayersTopEdge.PropertyChanged += this.OnLayerOverrideChanged;
    }

    /// <inheritdoc />
    public override bool HasCollider => this.GetColliders().Any();

    /// <summary>
    /// Gets the bottom edge's overriden layer.
    /// </summary>
    [DataMember(Name = "Bottom Layers", Order = 103)]
    public LayersOverride OverrideLayersBottomEdge { get; } = new();

    /// <summary>
    /// Gets the left edge's overriden layer.
    /// </summary>
    [DataMember(Name = "Left Layers", Order = 100)]
    public LayersOverride OverrideLayersLeftEdge { get; } = new();

    /// <summary>
    /// Gets the right edge's overriden layer.
    /// </summary>
    [DataMember(Name = "Right Layers", Order = 102)]
    public LayersOverride OverrideLayersRightEdge { get; } = new();

    /// <summary>
    /// Gets the top edge's overriden layer.
    /// </summary>
    [DataMember(Name = "Top Layers", Order = 101)]
    public LayersOverride OverrideLayersTopEdge { get; } = new();

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.ResetColliders();
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.ResetColliders();
    }

    /// <summary>
    /// Resets the colliders on this body.
    /// </summary>
    protected abstract void ResetColliders();

    private void OnLayerOverrideChanged(object? sender, PropertyChangedEventArgs e) {
        this.ResetColliders();
    }
}