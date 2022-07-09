namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;

/// <summary>
/// A platform from which actors can bounce.
/// </summary>
public interface IBouncePlatform {
    /// <summary>
    /// Gets the bounce velocity.
    /// </summary>
    public float BounceVelocity { get; }
}

/// <summary>
/// A platform that bounces actors.
/// </summary>
public class BouncePlatform : Platform, IBouncePlatform {
    private float _bounceVelocity;

    /// <inheritdoc />
    [DataMember]
    public float BounceVelocity {
        get => this._bounceVelocity;
        protected set {
            if (value > 0f) {
                this._bounceVelocity = value;
            }

            this.RaisePropertyChanged();
        }
    }
}