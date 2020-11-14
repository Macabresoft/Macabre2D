namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A base class for gizmos that can operate on one axis or the other.
    /// </summary>
    public abstract class BaseAxisGizmoComponent : GameUpdateableComponent {
        
        /// <summary>
        /// Represents the axis a gizmo is being operated on.
        /// </summary>
        protected enum GizmoAxis {
            X,
            Y,
            Neutral,
            None
        }
        
        /// <summary>
        /// Gets or sets the neutral axis position, which is the intersection of the X and Y axis.
        /// </summary>
        protected Vector2 NeutralAxisPosition { get; set; }
        
        /// <summary>
        /// Gets or sets the end point of the x axis line.
        /// </summary>
        protected Vector2 XAxisPosition { get; set; }
        
        /// <summary>
        /// Gets or sets the end point of the y axis line.
        /// </summary>
        protected Vector2 YAxisPosition { get; set; }
        
        /// <summary>
        /// Gets or sets the current axis being operated on.
        /// </summary>
        protected GizmoAxis CurrentAxis { get; set; } = GizmoAxis.None;

        /// <summary>
        /// Gets the length of an axis line based on the view height.
        /// </summary>
        /// <param name="viewHeight">The view height of the current camera.</param>
        /// <returns>The length of an axis line.</returns>
        protected float GetAxisLength(float viewHeight) {
            return viewHeight * 0.1f;
        }

        protected void ResetEndPoints(ITransformable transformable) {
            var worldTransform = transformable.Transform;
            // TODO: the stuff
        }
    }
}