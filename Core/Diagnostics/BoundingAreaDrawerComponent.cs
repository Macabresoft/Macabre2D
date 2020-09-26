namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Draws bounding areas from colliders for debugging purposes.
    /// </summary>
    public class BoundingAreaDrawerComponent : BaseDrawerComponent, IGameUpdateableComponent {
        private readonly List<BoundingArea> _boundingAreas = new List<BoundingArea>();
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

            if (this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                foreach (var boundingArea in this._boundingAreas) {
                    var minimum = boundingArea.Minimum;
                    var maximum = boundingArea.Maximum;
                    var lineThickness = this.GetLineThickness(viewBoundingArea.Height);

                    var points = new Vector2[] { minimum, new Vector2(minimum.X, maximum.Y), maximum, new Vector2(maximum.X, minimum.Y) };
                    this.PrimitiveDrawer.DrawPolygon(spriteBatch, this.Color, lineThickness, points);
                }
            }
        }

        public void Update(FrameTime frameTime, InputState inputState) {
            this._boundingAreas.Clear();
            this._boundingAreas.AddRange(this.Entity.Components.OfType<IBoundable>().Where(x => x != this).Select(x => x.BoundingArea).Where(x => !x.IsEmpty).ToList());
            this._boundingArea = BoundingArea.Combine(this._boundingAreas.ToArray());
        }
    }
}