namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="ICamera" /> for platformers.
/// </summary>
public class PlatformerCamera : Camera {
    private float _horizontalDistanceAllowed = 2f;

    /// <summary>
    /// Gets or sets the horizontal distance allowed from the player.
    /// </summary>
    [DataMember]
    public float HorizontalDistanceAllowed {
        get => this._horizontalDistanceAllowed;
        set => this.Set(ref this._horizontalDistanceAllowed, value);
    }

    /// <summary>
    /// Updates the position according to the specified actor state.
    /// </summary>
    /// <param name="previousState">The previous state.</param>
    /// <param name="currentState">The current state.</param>
    public void UpdateDesiredPosition(ActorState previousState, ActorState currentState) {
        if (this.HorizontalDistanceAllowed == 0f) {
            this.LocalPosition = Vector2.Zero;
        }
        else {
            var x = Math.Clamp(this.LocalPosition.X + currentState.Position.X - previousState.Position.X, -this.HorizontalDistanceAllowed, this.HorizontalDistanceAllowed);
            this.LocalPosition = new Vector2(x, this.LocalPosition.Y);
        }
    }
}