namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

public class ConveyorBelt : BaseMovingPlatform, IUpdateableEntity {
    private float _velocity = 1f;

    /// <summary>
    /// Gets or sets the velocity of this conveyor belt in units per second.
    /// </summary>
    [DataMember]
    public float Velocity {
        get => this._velocity;
        set => this.Set(ref this._velocity, value);
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        var amount = this.Velocity * (float)frameTime.SecondsPassed;
        this.MoveAttached(new Vector2(amount, 0f));
    }
}