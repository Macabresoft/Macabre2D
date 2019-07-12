namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Macabre2D.UI.ServiceInterfaces;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class TileGizmo : IGizmo {
        private readonly IUndoService _undoService;
        private IGame _game;

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
            var viewRatio = GameSettings.Instance.GetPixelAgnosticRatio(viewBoundingArea.Height, this._game.GraphicsDevice.Viewport.Height);
            if (selectedComponent is IRotatable rotatable) {
            }
            else {
            }
        }

        public void Initialize(IGame game) {
            this._game = game;
        }

        public bool Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState, Vector2 mousePosition, ComponentWrapper selectedComponent) {
            return true; // This should always have interactions
        }
    }
}