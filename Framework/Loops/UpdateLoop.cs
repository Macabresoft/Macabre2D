namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

/// <summary>
/// A loop that calls updates on entities.
/// </summary>
public class UpdateLoop : Loop {
    /// <inheritdoc />
    public override LoopKind Kind => LoopKind.Update;

    /// <summary>
    /// Gets the bottom edge's overriden layer.
    /// </summary>
    [DataMember(Name = "Layers to Update")]
    public LayersOverride LayersToUpdate { get; } = new();

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        foreach (var entity in this.GetEntitiesToUpdate()) {
            entity.Update(frameTime, inputState);
        }
    }

    private IEnumerable<IUpdateableEntity> GetEntitiesToUpdate() {
        return !this.LayersToUpdate.IsEnabled ? this.Scene.UpdateableEntities : this.Scene.UpdateableEntities.Where(x => (x.Layers & this.LayersToUpdate.Value) != Layers.None);
    }
}