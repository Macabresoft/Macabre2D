namespace Macabresoft.Macabre2D.Editor.Library.MonoGame.Components {
    using Macabresoft.Macabre2D.Editor.Library.Services;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// A component which selects entities and components based on their bounding areas.
    /// </summary>
    public class SelectorComponent : GameUpdateableComponent {
        private readonly IEntitySelectionService _selectionService;
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectorComponent" /> class.
        /// </summary>
        /// <param name="selectionService">The selection service.</param>
        public SelectorComponent(IEntitySelectionService selectionService) : base() {
            this._selectionService = selectionService;
        }
        
        /// <inheritdoc />
        public override void Update(FrameTime frameTime, InputState inputState) {
            if (inputState.CurrentMouseState.LeftButton == ButtonState.Pressed &&
                inputState.PreviousMouseState.LeftButton == ButtonState.Released) {
                
            }
        }
    }
}