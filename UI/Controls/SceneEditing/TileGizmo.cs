namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class TileGizmo : IGizmo {
        private readonly IUndoService _undoService;
        private EditorGame _game;
        private GridDrawer _gridDrawer;

        public TileGizmo(IUndoService undoService) {
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

                this._gridDrawer.Grid = tileable.Grid;
                this._gridDrawer.Draw(gameTime, viewBoundingArea);
            }
        }

        public void Initialize(EditorGame game) {
            this._game = game;

            this._gridDrawer = new GridDrawer() {
                Camera = this._game.CurrentCamera,
                UseDynamicLineThickness = true,
                LineThickness = 1f
            };

            this._gridDrawer.Initialize(this._game.CurrentScene);
        }

        public bool Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState, Vector2 mousePosition, ComponentWrapper selectedComponent) {
            return true; // This should always have interactions
        }
    }
}