namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public sealed class TranslateGizmo : BaseGizmo {

        public TranslateGizmo(EditorGame editorGame) : base(editorGame) {
        }

        public override void Draw(GameTime gameTime, float viewHeight, BaseComponent selectedComponent) {
            base.Draw(gameTime, viewHeight, selectedComponent);
        }

        public override void Initialize() {
            base.Initialize();
        }

        public override void Update(GameTime gameTime, MouseState mouseState, BaseComponent selectedComponent) {
            // TODO: allow dragging of the axis.
            return;
        }
    }
}