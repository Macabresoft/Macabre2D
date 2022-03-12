namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="ICamera" /> for platformers.
/// </summary>
public class PlatformerCamera : Camera {
    private Vector2 _focusedScreenArea = new(0.25f, 0.25f);

    /// <summary>
    /// Gets the focused bounding area.
    /// </summary>
    public BoundingArea FocusedBoundingArea { get; private set; } = BoundingArea.Empty;

    /// <summary>
    /// Gets or sets the area of the camera the tracked object can freely move without the camera having to move. This is a percentage of the total screen area.
    /// </summary>
    [DataMember]
    public Vector2 FocusedScreenArea {
        get => this._focusedScreenArea;
        set {
            if (this.Set(ref this._focusedScreenArea, value.Clamp(Vector2.Zero, Vector2.One))) {
                this.ResetFocusedBoundingArea();
            }
        }
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

    /// <inheritdoc />
    protected override void OnScreenAreaChanged() {
        base.OnScreenAreaChanged();
        this.ResetFocusedBoundingArea();
    }

    private void Parent_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(ITransformable.Transform)) {
            this.UpdatePosition();
        }
    }

    private void ResetFocusedBoundingArea() {
        var height = this.ViewHeight * this.FocusedScreenArea.Y;
        var width = this.ViewWidth * this.FocusedScreenArea.X;
        var minimum = this.Transform.Position;
        this.FocusedBoundingArea = new BoundingArea(minimum, width, height);
    }

    private void UpdatePosition() {
        if (this.FocusedBoundingArea.IsEmpty) {
            this.LocalPosition = this.Parent.Transform.Position;
        }
        else if (this.Parent is IBoundable boundable) {
            if (boundable.BoundingArea.Contains(this.FocusedBoundingArea)) {
                this.LocalPosition = this.Parent.Transform.Position;
            }
            else if (!this.FocusedBoundingArea.Contains(boundable.BoundingArea)) {
                // TODO: move so that the focused bounding area once again contains the parent
            }
        }
    }
}