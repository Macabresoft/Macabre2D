namespace Macabre2D.Engine.Windows.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.Engine.Windows.Models;
    using Macabre2D.Engine.Windows.Models.FrameworkWrappers;
    using Macabre2D.Engine.Windows.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class TileGizmo : IGizmo {
        private readonly HashSet<Point> _addedTiles = new HashSet<Point>();
        private readonly HashSet<Point> _removedTiles = new HashSet<Point>();
        private readonly ISceneService _sceneService;
        private readonly IUndoService _undoService;
        private EditorGame _game;
        private GridDrawerComponent _gridDrawer;
        private ButtonState _previousLeftButtonState = ButtonState.Released;
        private ButtonState _previousRightButtonState = ButtonState.Released;

        public TileGizmo(ISceneService sceneService, IUndoService undoService) {
            this._sceneService = sceneService;
            this._undoService = undoService;
        }

        public string EditingPropertyName {
            get {
                return string.Empty;
            }
        }

        public bool OverrideSelectionDisplay {
            get {
                return true;
            }
        }

        public void Draw(GameTime gameTime, BoundingArea viewBoundingArea, BaseComponent selectedComponent) {
            if (selectedComponent is ITileable tileable) {
                if (this._game.CurrentScene != null) {
                    this._gridDrawer.Color = new Color(this._game.CurrentScene.BackgroundColor.GetContrastingBlackOrWhite(), 75);
                }

                this._gridDrawer.Grid = tileable.WorldGrid;
                this._gridDrawer.Draw(gameTime, viewBoundingArea);
            }
        }

        public void Initialize(EditorGame game) {
            this._game = game;

            this._gridDrawer = new GridDrawerComponent() {
                Camera = this._game.CurrentCamera,
                UseDynamicLineThickness = true,
                LineThickness = 1f
            };

            this._gridDrawer.Initialize(this._game.CurrentScene);
        }

        public bool Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState, Vector2 mousePosition, ComponentWrapper selectedComponent) {
            if (selectedComponent.Component is ITileable tileable) {
                if (this.ShouldAdd(mouseState)) {
                    this.AddTile(tileable, mousePosition);
                }
                else if (this._previousLeftButtonState == ButtonState.Pressed && this._addedTiles.Any()) {
                    this.CommitAdd(tileable);
                }
                else if (this.ShouldRemove(mouseState)) {
                    this.RemoveTile(tileable, mousePosition);
                }
                else if (this._previousRightButtonState == ButtonState.Pressed && this._removedTiles.Any()) {
                    this.CommitRemove(tileable);
                }
            }

            return true; // This should always have interactions
        }

        private void AddTile(ITileable tileable, Vector2 mousePosition) {
            var tile = tileable.GetTileThatContains(mousePosition);
            if (tileable.AddTile(tile)) {
                this._addedTiles.Add(tile);
                this._previousLeftButtonState = ButtonState.Pressed;
                this._previousRightButtonState = ButtonState.Released;
            }
        }

        private void CommitAdd(ITileable tileable) {
            var hasChanges = this._sceneService.CurrentScene.HasChanges;
            var tiles = this._addedTiles.ToList();
            var undoCommand = new UndoCommand(() => {
                foreach (var tile in tiles) {
                    tileable.AddTile(tile);
                }

                this._sceneService.CurrentScene.HasChanges = true;
            }, () => {
                foreach (var tile in tiles) {
                    tileable.RemoveTile(tile);
                }

                this._sceneService.CurrentScene.HasChanges = hasChanges;
            });

            this._undoService.Do(undoCommand);
            this._addedTiles.Clear();
            this._previousRightButtonState = ButtonState.Released;
            this._previousLeftButtonState = ButtonState.Released;
        }

        private void CommitRemove(ITileable tileable) {
            var hasChanges = this._sceneService.CurrentScene.HasChanges;
            var tiles = this._removedTiles.ToList();
            var undoCommand = new UndoCommand(() => {
                foreach (var tile in tiles) {
                    tileable.RemoveTile(tile);
                }

                this._sceneService.CurrentScene.HasChanges = true;
            }, () => {
                foreach (var tile in tiles) {
                    tileable.AddTile(tile);
                }

                this._sceneService.CurrentScene.HasChanges = hasChanges;
            });

            this._undoService.Do(undoCommand);
            this._removedTiles.Clear();
            this._previousRightButtonState = ButtonState.Released;
            this._previousLeftButtonState = ButtonState.Released;
        }

        private void RemoveTile(ITileable tileable, Vector2 mousePosition) {
            var tile = tileable.GetTileThatContains(mousePosition);
            if (tileable.RemoveTile(tile)) {
                this._removedTiles.Add(tile);
                this._previousRightButtonState = ButtonState.Pressed;
                this._previousLeftButtonState = ButtonState.Released;
            }
        }

        private bool ShouldAdd(MouseState mouseState) {
            return mouseState.LeftButton == ButtonState.Pressed && !(this._previousRightButtonState == ButtonState.Pressed && mouseState.RightButton == ButtonState.Pressed);
        }

        private bool ShouldRemove(MouseState mouseState) {
            return mouseState.RightButton == ButtonState.Pressed && !(this._previousLeftButtonState == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Pressed);
        }
    }
}