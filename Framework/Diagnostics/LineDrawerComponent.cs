namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Runtime.Serialization;

    /// <summary>
    /// Draws a line.
    /// </summary>
    [Display(Name = "Line Drawer (Diagnostics)")]
    public sealed class LineDrawerComponent : BaseDrawerComponent {
        private Vector2 _endPoint;
        private Vector2 _startPoint;

        /// <inheritdoc />
        public override BoundingArea BoundingArea {
            get {
                return new BoundingArea(Vector2.Min(this.StartPoint, this.EndPoint), Vector2.Max(this.StartPoint, this.EndPoint));
            }
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>The end point.</value>
        [DataMember(Order = 4)]
        public Vector2 EndPoint {
            get {
                return this._endPoint;
            }

            set {
                this.Set(ref this._endPoint, value);
            }
        }

        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        /// <value>The start point.</value>
        [DataMember(Order = 3)]
        public Vector2 StartPoint {
            get {
                return this._startPoint;
            }

            set {
                this.Set(ref this._startPoint, value);
            }
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer != null && this.StartPoint != this.EndPoint && this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                this.PrimitiveDrawer.DrawLine(spriteBatch, this.Entity.Scene.Game.Project.Settings.PixelsPerUnit, this.StartPoint, this.EndPoint, this.Color, lineThickness);
            }
        }
    }
}