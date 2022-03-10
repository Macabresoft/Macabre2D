namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="ICamera" /> for platformers.
/// </summary>
public class PlatformerCamera : Camera, IUpdateableEntity {
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
    public void Update(FrameTime frameTime, InputState inputState) {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected override void OnScreenAreaChanged() {
        base.OnScreenAreaChanged();
        this.ResetFocusedBoundingArea();
    }

    private void ResetFocusedBoundingArea() {
        var height = this.ViewHeight * this.FocusedScreenArea.Y;
        var width = this.ViewWidth * this.FocusedScreenArea.X;
        var minimum = this.Transform.Position;
        this.FocusedBoundingArea = new BoundingArea(minimum, width, height);
    }
}