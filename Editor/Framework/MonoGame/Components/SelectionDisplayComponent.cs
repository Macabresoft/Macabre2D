namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A component which displays the currently selected <see cref="IGameEntity"/> and <see cref="Framework.IGameComponent"/>.
    /// </summary>
    public class SelectionDisplayComponent : BaseDrawerComponent {

        private readonly IEntitySelectionService _selectionService;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionDisplayComponent" /> class.
        /// </summary>
        /// <param name="selectionService">The selection service.</param>
        public SelectionDisplayComponent(IEntitySelectionService selectionService) : base() {
            this.UseDynamicLineThickness = true;
            this._selectionService = selectionService;
        }

        /// <inheritdoc />
        public override BoundingArea BoundingArea {
            get {
                if (this._selectionService.SelectedComponent is IBoundable boundable) {
                    return boundable.BoundingArea;
                }

                return BoundingArea.Empty;
            }
        }
        
        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer == null || this.LineThickness <= 0f || this.Color == Color.Transparent || this.BoundingArea.Maximum == this.BoundingArea.Minimum) {
                return;
            }

            if (this.Entity.Scene.Game.SpriteBatch is SpriteBatch spriteBatch) {
                var lineThickness = -1f;
                if (!this.BoundingArea.IsEmpty) {
                    var minimum = this.BoundingArea.Minimum;
                    var maximum = this.BoundingArea.Maximum;
                    lineThickness = this.GetLineThickness(viewBoundingArea.Height);

                    var points = new Vector2[] { minimum, new Vector2(minimum.X, maximum.Y), maximum, new Vector2(maximum.X, minimum.Y) };
                    this.PrimitiveDrawer.DrawPolygon(spriteBatch, this.Color, lineThickness, points);
                }

                if (this._selectionService.SelectedEntity != null) {
                    var position = this._selectionService.SelectedEntity.Transform.Position;

                    if (lineThickness < 0) {
                        lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                    }

                    var crosshairLength = viewBoundingArea.Height * 0.01f;
                    var left = new Vector2(position.X - crosshairLength, position.Y);
                    var right = new Vector2(position.X + crosshairLength, position.Y);
                    var top = new Vector2(position.X, position.Y + crosshairLength);
                    var bottom = new Vector2(position.X, position.Y - crosshairLength);
                    this.PrimitiveDrawer.DrawLine(spriteBatch, left, right, this.Color, lineThickness);
                    this.PrimitiveDrawer.DrawLine(spriteBatch, top, bottom, this.Color, lineThickness);
                }
            }
        }
    }
}