﻿namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Entities {
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
        private readonly ISelectionService _selectionService;
        private Camera _camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorGrid" /> class.
        /// </summary>
        /// <param name="editorService">The editor service.</param>
        /// <param name="sceneService">The scene service.</param>
        /// <param name="selectionService">The selection service.</param>
        public EditorGrid(IEditorService editorService, ISceneService sceneService, ISelectionService selectionService) {
            this._editorService = editorService;
            this._sceneService = sceneService;
            this._selectionService = selectionService;
        }

        /// <inheritdoc />
        public override BoundingArea BoundingArea => this._camera?.BoundingArea ?? BoundingArea.Empty;


        /// <inheritdoc />
        public override void Initialize(IScene scene, IEntity entity) {
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
                if (this._editorService.GridDivisions > 0 && this.ResolveGridContainer() is IGridContainer container) {
                    if (!Framework.Scene.IsNullOrEmpty(this._sceneService.CurrentScene) && this.Color != this._sceneService.CurrentScene.BackgroundColor) {
                        this.ResetColor();
                    }

                    var lineThickness = this.GetLineThickness(viewBoundingArea.Height);
                    this.DrawGrid(spriteBatch, viewBoundingArea, container.Grid, lineThickness, 0.2f);

                    var majorGridSize = container.Grid.TileSize * this._editorService.GridDivisions;
                    var grid = new TileGrid(majorGridSize, container.Grid.Offset);
                    this.DrawGrid(spriteBatch, viewBoundingArea, grid, lineThickness, 0.5f);
                }
            }
        }

        private void DrawGrid(SpriteBatch spriteBatch, BoundingArea boundingArea, TileGrid grid, float lineThickness, float alpha) {
            if (this.PrimitiveDrawer != null) {
                var shadowOffset = lineThickness * this.Scene.Game.Project.Settings.InversePixelsPerUnit;
                var horizontalShadowOffset = new Vector2(-shadowOffset, 0f);
                var verticalShadowOffset = new Vector2(0f, shadowOffset);
                var color = this.Color * alpha;

                var columns = GridDrawer.GetGridPositions(boundingArea.Minimum.X, boundingArea.Maximum.X, grid.TileSize.X, grid.Offset.X);
                var pixelsPerUnit = this.Scene.Game.Project.Settings.PixelsPerUnit;
                foreach (var column in columns) {
                    var minimum = new Vector2(column, boundingArea.Minimum.Y);
                    var maximum = new Vector2(column, boundingArea.Maximum.Y);

                    if (Math.Abs(column - grid.Offset.X) < float.Epsilon) {
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

                var rows = GridDrawer.GetGridPositions(boundingArea.Minimum.Y, boundingArea.Maximum.Y, grid.TileSize.Y, grid.Offset.Y);
                foreach (var row in rows) {
                    var minimum = new Vector2(boundingArea.Minimum.X, row);
                    var maximum = new Vector2(boundingArea.Maximum.X, row);

                    if (Math.Abs(row - grid.Offset.Y) < float.Epsilon) {
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
            this.Color = !Framework.Scene.IsNullOrEmpty(this._sceneService.CurrentScene) ? this._sceneService.CurrentScene.BackgroundColor.GetContrastingBlackOrWhite() : this.Scene.BackgroundColor.GetContrastingBlackOrWhite();
        }

        private IGridContainer ResolveGridContainer() {
            var gridContainer = GridContainer.EmptyGridContainer;
            if (this._selectionService.SelectedEntity != null) {
                if (this._selectionService.SelectedEntity is IGridContainer container) {
                    gridContainer = container;
                }
                else if (this._selectionService.SelectedEntity.TryGetParentEntity(out container) && container != null) {
                    gridContainer = container;
                }
            }

            return gridContainer;
        }
    }
}