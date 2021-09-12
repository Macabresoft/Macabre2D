namespace Macabresoft.Macabre2D.UI.Common.MonoGame.Entities {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A gizmo for editing <see cref="ITileableEntity"/>.
    /// </summary>
    public class TileGizmo : Entity, IGizmo {
        private readonly HashSet<Point> _addedTiles = new();
        private readonly HashSet<Point> _removedTiles = new();
        private readonly IEntityService _entityService;
        private readonly IUndoService _undoService;
        private ICamera _camera;
        private MouseButton? _currentButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorGizmo" /> class.
        /// </summary>
        /// <param name="entityService">The selection service.</param>
        /// <param name="undoService">The undo service.</param>
        public TileGizmo(IEntityService entityService, IUndoService undoService) {
            this._entityService = entityService;
            this._undoService = undoService;
        }
        
        /// <inheritdoc />
        public GizmoKind GizmoKind => GizmoKind.Tile;
        
        /// <inheritdoc />
        public override void Initialize(IScene scene, IEntity entity) {
            base.Initialize(scene, entity);

            if (!this.TryGetParentEntity(out this._camera)) {
                throw new NotSupportedException("Could not find a camera ancestor.");
            }
        }
        
        /// <inheritdoc />
        public bool Update(FrameTime frameTime, InputState inputState) {
            if (this._camera != null && this._entityService.Selected is ITileableEntity tileable) {
                switch (this._currentButton) {
                    case null when inputState.IsButtonNewlyPressed(MouseButton.Left):
                        this.AddTile(tileable, inputState);
                        break;
                    case null: {
                        if (inputState.IsButtonNewlyPressed(MouseButton.Right)) {
                            this.RemoveTile(tileable, inputState);
                        }

                        break;
                    }
                    case MouseButton.Left when inputState.IsButtonHeld(MouseButton.Left):
                        this.AddTile(tileable, inputState);
                        break;
                    case MouseButton.Left:
                        this.CommitAdd(tileable);
                        break;
                    case MouseButton.Right when inputState.IsButtonHeld(MouseButton.Right):
                        this.RemoveTile(tileable, inputState);
                        break;
                    case MouseButton.Right:
                        this.CommitRemove(tileable);
                        break;
                }
            }

            return true;
        }

        private void AddTile(ITileableEntity tileable, InputState inputState) {
            this._currentButton = MouseButton.Left;
            var mousePosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);
            var tile = tileable.GetTileThatContains(mousePosition);
            if (tileable.AddTile(tile)) {
                this._addedTiles.Add(tile);
            }
        }

        private void RemoveTile(ITileableEntity tileable, InputState inputState) {
            this._currentButton = MouseButton.Right;
            var mousePosition = this._camera.ConvertPointFromScreenSpaceToWorldSpace(inputState.CurrentMouseState.Position);
            var tile = tileable.GetTileThatContains(mousePosition);
            if (tileable.RemoveTile(tile)) {
                this._removedTiles.Add(tile);
            }
        }

        private void CommitAdd(ITileableEntity tileable) {
            this._currentButton = null;

            if (tileable != null && this._addedTiles.Any()) {
                var tiles = this._addedTiles.ToList();
                this._undoService.Do(() => {
                    foreach (var tile in tiles) {
                        tileable.AddTile(tile);
                    }
                }, () => {
                    foreach (var tile in tiles) {
                        tileable.RemoveTile(tile);
                    }
                });
            }
            
            this._addedTiles.Clear();
        }

        private void CommitRemove(ITileableEntity tileable) {
            this._currentButton = null;
            
            if (tileable != null && this._removedTiles.Any()) {
                var tiles = this._removedTiles.ToList();
                this._undoService.Do(() => {
                    foreach (var tile in tiles) {
                        tileable.RemoveTile(tile);
                    }
                }, () => {
                    foreach (var tile in tiles) {
                        tileable.AddTile(tile);
                    }
                });
            }
            
            this._removedTiles.Clear();
        }
    }
}