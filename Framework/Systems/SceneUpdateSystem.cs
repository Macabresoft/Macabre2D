namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabre2D.Project.Common;

/// <summary>
/// The kind of system being run.
/// </summary>
public enum SceneUpdateSystemKind {
    None = 0,
    Update = 1,
    PreUpdate = 2,
    PostUpdate = 3
}

/// <summary>
/// Interface for a <see cref="ISceneSystem"/> that has an update loop.
/// </summary>
public interface ISceneUpdateSystem : ISceneSystem, IUpdateableGameObject {
    /// <summary>
    /// Gets the kind of update.
    /// </summary>
    SceneUpdateSystemKind Kind { get; }
}

/// <summary>
/// A system that calls updates on entities.
/// </summary>
public class SceneUpdateSystem : SceneSystem, ISceneUpdateSystem {
    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged;
    
    /// <inheritdoc />
    public virtual SceneUpdateSystemKind Kind => SceneUpdateSystemKind.Update;

    /// <inheritdoc />
    [DataMember]
    public bool ShouldUpdate {
        get;
        set {
            if (this.Set(ref field, value)) {
                this.ShouldUpdateChanged.SafeInvoke(this);
            }
        }
    } = true;

    /// <summary>
    /// Gets the bottom edge's overriden layer.
    /// </summary>
    [DataMember(Name = "Layers to Update")]
    public LayersOverride LayersToUpdate { get; } = new();

    /// <inheritdoc />
    public virtual void Update(FrameTime frameTime, InputState inputState) {
        foreach (var entity in this.GetEntitiesToUpdate()) {
            entity.Update(frameTime, inputState);
        }
    }

    private IEnumerable<IUpdateableEntity> GetEntitiesToUpdate() {
        return !this.LayersToUpdate.IsEnabled ? this.Scene.UpdateableEntities : this.Scene.UpdateableEntities.Where(x => (x.Layers & this.LayersToUpdate.Value) != Layers.None);
    }
}