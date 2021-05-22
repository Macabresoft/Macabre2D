namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel.DataAnnotations;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Draws bounding areas from colliders for debugging purposes.
    /// </summary>
    [Display(Name = "Bounding Area Drawer (Diagnostics)")]
    public class BoundingAreaDrawer : BaseDrawer, IGameUpdateableEntity {
        private BoundingArea _boundingArea;

        /// <inheritdoc />
        public override BoundingArea BoundingArea => this._boundingArea;

        /// <inheritdoc />
        public int UpdateOrder => 0;

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer == null || this.LineThickness <= 0f || this.Color == Color.Transparent || this.BoundingArea.Maximum == this.BoundingArea.Minimum) {
                return;
            }

            if (this.Scene.Game.SpriteBatch is SpriteBatch spriteBatch && !this._boundingArea.IsEmpty) {
                var minimum = this._boundingArea.Minimum;
                var maximum = this._boundingArea.Maximum;
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);

                var points = new[] { minimum, new Vector2(minimum.X, maximum.Y), maximum, new Vector2(maximum.X, minimum.Y) };
                this.PrimitiveDrawer.DrawPolygon(
                    spriteBatch,
                    this.Scene.Game.Project.Settings.PixelsPerUnit,
                    this.Color,
                    lineThickness,
                    points);
            }
        }

        public void Update(FrameTime frameTime, InputState inputState) {
            if (this.Parent is IBoundable boundable) {
                this._boundingArea = boundable.BoundingArea;
            }
            else {
                this._boundingArea = BoundingArea.Empty;
            }
        }
    }
}