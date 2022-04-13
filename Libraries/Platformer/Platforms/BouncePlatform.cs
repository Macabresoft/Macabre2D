namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A platform that bounces actors.
/// </summary>
public class BouncePlatform : Platform, IUpdateableEntity {
    private Vector2 _bounceVelocity;

    /// <summary>
    /// Gets or sets the bounce velocity;
    /// </summary>
    [DataMember]
    public Vector2 BounceVelocity {
        get => this._bounceVelocity;
        set => this.Set(ref this._bounceVelocity, value);
    }

    /// <inheritdoc />
    public void Update(FrameTime frameTime, InputState inputState) {
        foreach (var attached in this.Attached) {
            attached.Bounce(this.BounceVelocity);
        }
    }
}