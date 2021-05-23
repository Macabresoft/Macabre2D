namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Entities {
    using System;
    using System.ComponentModel;
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Draws a grid for the editor.
    /// </summary>
    internal sealed class EditorGrid : BaseDrawer {
        private readonly IEditorService _editorService;
        private readonly ISceneService _sceneService;
        private Camera _camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorGrid" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="sceneService">The scene service.</param>
        public EditorGrid(IEditorService editorService, ISceneService sceneService) {
            this._editorService = editorService;
            this._sceneService = sceneService;
        }

        /// <inheritdoc />
        public override BoundingArea BoundingArea => this._camera?.BoundingArea ?? BoundingArea.Empty;


        /// <inheritdoc />
        public override void Initialize(IGameScene scene, IGameEntity entity) {
            base.Initialize(scene, entity);

            this.UseDynamicLineThickness = true;

            if (!this.TryGetParentEntity(out this._camera)) {
                throw new NotSupportedException("Could not find a camera ancestor.");
            }

            this._editorService.PropertyChanged += this.EditorService_PropertyChanged;
            this.IsVisible = this._editorService.ShowGrid;
            this.ResetColor();
        }

        /// <inheritdoc />
        public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
            if (this.PrimitiveDrawer == null) {
                return;
            }

            if (this.Scene.Game.SpriteBatch is { } spriteBatch) {
                if (this._editorService.MajorGridSize > 0) {
                    if (!GameScene.IsNullOrEmpty(this._sceneService.CurrentScene) && this.Color != this._sceneService.CurrentScene.BackgroundColor) {
                        this.ResetColor();
                    }

                    var lineThickness = this.GetLineThickness(viewBoundingArea.Height);

                    if (this._editorService.MinorGridDivisions > 0) {
                        var minorGridSize = this._editorService.MajorGridSize / this._editorService.MinorGridDivisions;
                        this.DrawGrid(spriteBatch, viewBoundingArea, minorGridSize, lineThickness, 0.2f);
                    }

                    this.DrawGrid(spriteBatch, viewBoundingArea, this._editorService.MajorGridSize, lineThickness, 0.5f);
                }
            }
        }

        private void DrawGrid(SpriteBatch spriteBatch, BoundingArea boundingArea, float gridSize, float lineThickness, float alpha) {
            if (this.PrimitiveDrawer != null) {
                var shadowOffset = lineThickness * this.Scene.Game.Project.Settings.InversePixelsPerUnit;
                var horizontalShadowOffset = new Vector2(-shadowOffset, 0f);
                var verticalShadowOffset = new Vector2(0f, shadowOffset);
                var color = this.Color * alpha;

                var columns = GridDrawer.GetGridPositions(boundingArea.Minimum.X, boundingArea.Maximum.X, gridSize, 0f);
                var pixelsPerUnit = this.Scene.Game.Project.Settings.PixelsPerUnit;
                foreach (var column in columns) {
                    var minimum = new Vector2(column, boundingArea.Minimum.Y);
                    var maximum = new Vector2(column, boundingArea.Maximum.Y);

                    if (column == 0f) {
                        this.PrimitiveDrawer.DrawLine(
                            spriteBatch,
                            pixelsPerUnit,
                            minimum + horizontalShadowOffset,
                            maximum + horizontalShadowOffset,
                            this._editorService.DropShadowColor,
                            lineThickness);

                        this.PrimitiveDrawer.DrawLine(
                            spriteBatch,
                            pixelsPerUnit,
                            minimum,
                            maximum,
                            this._editorService.YAxisColor,
                            lineThickness);
                    }
                    else {
                        this.PrimitiveDrawer.DrawLine(
                            spriteBatch,
                            pixelsPerUnit,
                            minimum,
                            maximum,
                            color,
                            lineThickness);
                    }
                }

                var rows = GridDrawer.GetGridPositions(boundingArea.Minimum.Y, boundingArea.Maximum.Y, gridSize, 0f);
                foreach (var row in rows) {
                    var minimum = new Vector2(boundingArea.Minimum.X, row);
                    var maximum = new Vector2(boundingArea.Maximum.X, row);

                    if (row == 0f) {
                        this.PrimitiveDrawer.DrawLine(
                            spriteBatch,
                            pixelsPerUnit,
                            minimum + verticalShadowOffset,
                            maximum + verticalShadowOffset,
                            this._editorService.DropShadowColor,
                            lineThickness);

                        this.PrimitiveDrawer.DrawLine(
                            spriteBatch,
                            pixelsPerUnit,
                            minimum,
                            maximum,
                            this._editorService.XAxisColor,
                            lineThickness);
                    }
                    else {
                        this.PrimitiveDrawer.DrawLine(
                            spriteBatch,
                            pixelsPerUnit,
                            minimum,
                            maximum,
                            color,
                            lineThickness);
                    }
                }
            }
        }

        private void EditorService_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IEditorService.ShowGrid)) {
                this.IsVisible = this._editorService.ShowGrid;
            }
        }

        private void ResetColor() {
            this.Color = !GameScene.IsNullOrEmpty(this._sceneService.CurrentScene) ? this._sceneService.CurrentScene.BackgroundColor.GetContrastingBlackOrWhite() : this.Scene.BackgroundColor.GetContrastingBlackOrWhite();
        }
    }
}