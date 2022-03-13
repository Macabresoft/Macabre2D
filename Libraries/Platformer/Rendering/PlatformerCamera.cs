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
    private Vector2 _focusOffset = Vector2.Zero;

    /// <summary>
    /// Gets or sets the distance allowed from the player.
    /// </summary>
    [DataMember]
    public Vector2 DistanceAllowedFromParent {
        get => this._distanceAllowedFromParent;
        set => this.Set(ref this._distanceAllowedFromParent, value);
    }

    /// <summary>
    /// Gets or sets the focus offset.
    /// </summary>
    /// <remarks>
    /// If this value is non zero, it essentially offsets where the center of this camera is for operations following its owner.
    /// </remarks>
    [DataMember]
    public Vector2 FocusOffset {
        get => this._focusOffset;
        set => this.Set(ref this._focusOffset, value);
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
            this.LocalPosition = this.Parent.Transform.Position + this.FocusOffset;
        }
        else if (this.Parent is ITransformable parent) {
            var (parentX, parentY) = parent.Transform.Position;
            var x = this.LocalPosition.X + this.FocusOffset.X;
            var horizontalDistance = parentX - x;
            if (horizontalDistance > this.DistanceAllowedFromParent.X) {
                x = parentX - this.DistanceAllowedFromParent.X + this.FocusOffset.X;
            }
            else if (horizontalDistance < -this.DistanceAllowedFromParent.X) {
                x = parentX + this.DistanceAllowedFromParent.X + this.FocusOffset.X;
            }

            var y = this.LocalPosition.Y + this.FocusOffset.Y;
            var verticalDistance = parentY - y;

            if (verticalDistance > this.DistanceAllowedFromParent.Y) {
                y = parentY - this.DistanceAllowedFromParent.Y + this.FocusOffset.X;
            }
            else if (verticalDistance < -this.DistanceAllowedFromParent.Y) {
                y = parentY + this.DistanceAllowedFromParent.Y + this.FocusOffset.X;
            }

            this.LocalPosition = new Vector2(x, y) - this.FocusOffset;
        }
    }
}