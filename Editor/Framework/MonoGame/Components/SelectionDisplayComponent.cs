namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using System.ComponentModel;
    using System.Linq;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A component which displays the currently selected <see cref="IGameEntity"/> and <see cref="Framework.IGameComponent"/>.
    /// </summary>
    public class SelectionDisplayComponent : BaseDrawerComponent {
        private readonly IEditorService _editorService;
        private readonly IEntitySelectionService _selectionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionDisplayComponent" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="selectionService">The selection service.</param>
        public SelectionDisplayComponent(IEditorService editorService, IEntitySelectionService selectionService) : base() {
            this.UseDynamicLineThickness = true;
            this.LineThickness = 2f;
            this._editorService = editorService;
            this._editorService.PropertyChanged += EditorService_PropertyChanged;
            this._selectionService = selectionService;

            this.Color = this._editorService.SelectionColor;
        }

        private void EditorService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IEditorService.SelectionColor)) {
                this.Color = this._editorService.SelectionColor;
            }
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
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                var shadowOffset = lineThickness * GameSettings.Instance.InversePixelsPerUnit;

                if (!this.BoundingArea.IsEmpty) {
                    var minimum = this.BoundingArea.Minimum;
                    var maximum = this.BoundingArea.Maximum;
                    
                    var points = new[] { minimum, new Vector2(minimum.X, maximum.Y), maximum, new Vector2(maximum.X, minimum.Y) };
                    
                    var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);
                    var shadowPoints = points.Select(x => x + shadowOffsetVector).ToArray();
                    
                    this.PrimitiveDrawer.DrawPolygon(spriteBatch, this._editorService.DropShadowColor, lineThickness, shadowPoints);
                    this.PrimitiveDrawer.DrawPolygon(spriteBatch, this.Color, lineThickness, points);
                }

                if (this._selectionService.SelectedEntity != null) {
                    var position = this._selectionService.SelectedEntity.Transform.Position;

                    var crosshairLength = viewBoundingArea.Height * 0.01f;
                    var left = new Vector2(position.X - crosshairLength, position.Y);
                    var right = new Vector2(position.X + crosshairLength, position.Y);
                    var top = new Vector2(position.X, position.Y + crosshairLength);
                    var bottom = new Vector2(position.X, position.Y - crosshairLength);
                    
                    var verticalShadowOffset = new Vector2(0f, shadowOffset);
                    this.PrimitiveDrawer.DrawLine(spriteBatch, left + verticalShadowOffset, right + verticalShadowOffset, this._editorService.DropShadowColor, lineThickness);
                    var horizontalShadowOffset = new Vector2(-shadowOffset, 0f);
                    this.PrimitiveDrawer.DrawLine(spriteBatch, top + horizontalShadowOffset, bottom + horizontalShadowOffset, this._editorService.DropShadowColor, lineThickness);
                    
                    this.PrimitiveDrawer.DrawLine(spriteBatch, left, right, this.Color, lineThickness);
                    this.PrimitiveDrawer.DrawLine(spriteBatch, top, bottom, this.Color, lineThickness);
                }
            }
        }
    }
}