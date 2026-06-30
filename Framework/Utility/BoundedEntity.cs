namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A basic entity with a bounding area based on width and height that implements <see cref="IBoundableEntity" />.
/// </summary>
public abstract class BoundedEntity : Entity, IBoundableEntity {

    /// <summary>
    /// Gets the bounding area instance.
    /// </summary>
    [DataMember]
    public BoundingAreaInstance BoundingAreaInstance { get; } = new();

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundedEntity" /> class.
    /// </summary>
    protected BoundedEntity() {
    }

    /// <inheritdoc />
    public BoundingArea BoundingArea => this.BoundingAreaInstance.BoundingArea;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.BoundingAreaInstance.BoundingAreaChanged -= this.BoundingAreaInstance_BoundingAreaChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.BoundingAreaInstance.BoundingAreaChanged += this.BoundingAreaInstance_BoundingAreaChanged;
    }

    /// <inheritdoc />
    protected override IEnumerable<IGameObjectReference> GetGameObjectReferences() {
        yield return this.BoundingAreaInstance;
    }

    private void BoundingAreaInstance_BoundingAreaChanged(object? sender, EventArgs e) {
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}