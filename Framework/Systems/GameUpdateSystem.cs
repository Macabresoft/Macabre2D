namespace Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A <see cref="IGameSystem" /> that also implements <see cref="IUpdateableGameObject" />.
/// </summary>
public abstract class GameUpdateSystem : GameSystem, IUpdateableGameObject {
    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged;

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


    /// <inheritdoc />
    public abstract void Update(FrameTime frameTime, InputState inputState);
}