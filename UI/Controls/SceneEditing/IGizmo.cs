namespace Macabre2D.UI.Controls.SceneEditing {

    using Macabre2D.Framework;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public interface IGizmo {
        string EditingPropertyName { get; }
        bool OverrideSelectionDisplay { get; }

        void Draw(GameTime gameTime, BoundingArea viewBoundingArea, BaseComponent selectedComponent);

        void Initialize(IGame game);

        bool Update(GameTime gameTime, MouseState mouseState, KeyboardState keyboardState, Vector2 mousePosition, ComponentWrapper selectedComponent);
    }
}