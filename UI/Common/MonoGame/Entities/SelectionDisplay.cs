namespace Macabresoft.Macabre2D.UI.Common {
    using System.ComponentModel;
    using System.Linq;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A component which displays the currently selected <see cref="IEntity" />.
    /// </summary>
    internal class SelectionDisplay : BaseDrawer {
        private readonly IEditorService _editorService;
        private readonly IEntityService _entityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionDisplay" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="entityService">The selection service.</param>
        public SelectionDisplay(IEditorService editorService, IEntityService entityService) : base() {
            this.UseDynamicLineThickness = true;
            this.LineThickness = 2f;
            this._editorService = editorService;
            this._editorService.PropertyChanged += this.EditorService_PropertyChanged;
            this._entityService = entityService;

            this.Color = this._editorService.SelectionColor;
        }

        /// <inheritdoc />
        public override BoundingArea BoundingArea {
            get {
                if (this._entityService.Selected is IBoundable boundable) {
                    return boundable.BoundingArea;
                }

                return BoundingArea.Empty;
            }
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer == null || this.LineThickness <= 0f || this.Color == Color.Transparent) {
                return;
            }

            if (this.Scene.Game.SpriteBatch is SpriteBatch spriteBatch && this.PrimitiveDrawer is PrimitiveDrawer drawer) {
                var settings = this.Scene.Game.Project.Settings;
                var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                var shadowOffset = lineThickness * settings.InversePixelsPerUnit * 0.5f;
                var shadowOffsetVector = new Vector2(-shadowOffset, shadowOffset);

                if (!this.BoundingArea.IsEmpty) {
                    var minimum = this.BoundingArea.Minimum;
                    var maximum = this.BoundingArea.Maximum;

                    var points = new[] { minimum, new Vector2(minimum.X, maximum.Y), maximum, new Vector2(maximum.X, minimum.Y) };

                    var shadowPoints = points.Select(x => x + shadowOffsetVector).ToArray();

                    drawer.DrawPolygon(spriteBatch, settings.PixelsPerUnit, this._editorService.DropShadowColor, lineThickness, shadowPoints);
                    drawer.DrawPolygon(spriteBatch, settings.PixelsPerUnit, this.Color, lineThickness, points);
                }

                if (this._entityService.Selected != null) {
                    if (this._editorService.SelectedGizmo == GizmoKind.Selector) {
                        var position = this._entityService.Selected.Transform.Position;

                        var crosshairLength = viewBoundingArea.Height * 0.01f;
                        var left = new Vector2(position.X - crosshairLength, position.Y);
                        var right = new Vector2(position.X + crosshairLength, position.Y);
                        var top = new Vector2(position.X, position.Y + crosshairLength);
                        var bottom = new Vector2(position.X, position.Y - crosshairLength);

                        var verticalShadowOffset = new Vector2(0f, shadowOffset);
                        drawer.DrawLine(
                            spriteBatch,
                            settings.PixelsPerUnit,
                            left + shadowOffsetVector,
                            right + shadowOffsetVector,
                            this._editorService.DropShadowColor,
                            lineThickness);

                        var horizontalShadowOffset = new Vector2(-shadowOffset, 0f);
                        drawer.DrawLine(
                            spriteBatch,
                            settings.PixelsPerUnit,
                            top + shadowOffsetVector,
                            bottom + shadowOffsetVector,
                            this._editorService.DropShadowColor,
                            lineThickness);

                        drawer.DrawLine(spriteBatch, settings.PixelsPerUnit, left, right, this.Color, lineThickness);
                        drawer.DrawLine(spriteBatch, settings.PixelsPerUnit, top, bottom, this.Color, lineThickness);
                    }

                    if (this._entityService.Selected is IPhysicsBody body) {
                        var colliders = body.GetColliders();
                        foreach (var collider in colliders) {
                            drawer.DrawCollider(
                                collider,
                                spriteBatch,
                                settings.PixelsPerUnit,
                                this._editorService.DropShadowColor,
                                lineThickness,
                                shadowOffsetVector);

                            drawer.DrawCollider(
                                collider,
                                spriteBatch,
                                settings.PixelsPerUnit,
                                this._editorService.ColliderColor,
                                lineThickness,
                                Vector2.Zero);
                        }
                    }
                }
            }
        }

        private void EditorService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IEditorService.SelectionColor)) {
                this.Color = this._editorService.SelectionColor;
            }
        }
    }
}