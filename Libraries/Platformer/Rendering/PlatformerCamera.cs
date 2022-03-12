namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="ICamera" /> for platformers.
/// </summary>
public class PlatformerCamera : Camera {
    private Vector2 _distanceAllowedFromParent = Vector2.Zero;

    /// <summary>
    /// Gets or sets the distance allowed from the player.
    /// </summary>
    [DataMember]
    public Vector2 DistanceAllowedFromParent {
        get => this._distanceAllowedFromParent;
        set => this.Set(ref this._distanceAllowedFromParent, value);
    }

    /// <inheritdoc />
    protected override bool IsTransformRelativeToParent => false;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        if (!IsNullOrEmpty(this.Parent, out var originalParent)) {
            originalParent.PropertyChanged -= this.Parent_PropertyChanged;
        }

        base.Initialize(scene, parent);

        if (!IsNullOrEmpty(this.Parent, out var newParent)) {
            newParent.PropertyChanged += this.Parent_PropertyChanged;
        }

        this.UpdatePosition();
    }

    private void Parent_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(ITransformable.Transform)) {
            this.UpdatePosition();
        }
    }

    private void UpdatePosition() {
        if (this.DistanceAllowedFromParent == Vector2.Zero) {
            this.LocalPosition = this.Parent.Transform.Position;
        }
        else if (this.Parent is ITransformable parent) {
            var parentPosition = parent.Transform.Position;
            var x = this.LocalPosition.X;
            var horizontalDistance = parentPosition.X - this.LocalPosition.X;
            if (horizontalDistance > this.DistanceAllowedFromParent.X) {
                x = parentPosition.X - this.DistanceAllowedFromParent.X;
            }
            else if (horizontalDistance < -this.DistanceAllowedFromParent.X) {
                x = parentPosition.X + this.DistanceAllowedFromParent.X;
            }

            var y = this.LocalPosition.Y;
            var verticalDistance = parentPosition.Y - this.LocalPosition.Y;

            if (verticalDistance > this.DistanceAllowedFromParent.Y) {
                y = parentPosition.Y - this.DistanceAllowedFromParent.Y;
            }
            else if (verticalDistance < -this.DistanceAllowedFromParent.Y) {
                y = parentPosition.Y + this.DistanceAllowedFromParent.Y;
            }

            this.LocalPosition = new Vector2(x, y);
        }
    }
}