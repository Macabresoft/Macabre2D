namespace Macabresoft.Macabre2D.AvaloniaUI {

    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A minimal instance of <see cref="Game" /> that is run for Avalonia.
    /// </summary>
    public sealed class AvaloniaGame : DefaultGame {

        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaGame" /> class.
        /// </summary>
        public AvaloniaGame() : base() {
            this.IsFixedTimeStep = false;
        }

        /// <inheritdoc />
        public override bool IsDesignMode => true;

        /// <summary>
        /// Gets the keyboard.
        /// </summary>
        /// <value>The keyboard.</value>
        public MonoGameKeyboard Keyboard { get; private set; }

        /// <summary>
        /// Gets the mouse.
        /// </summary>
        /// <value>The mouse.</value>
        public MonoGameMouse Mouse { get; private set; }

        /// <summary>
        /// Initializes the specified mouse.
        /// </summary>
        /// <param name="mouse">The mouse.</param>
        /// <param name="keyboard">The keyboard.</param>
        public void Initialize(MonoGameMouse mouse, MonoGameKeyboard keyboard) {
            this.Mouse = mouse;
            this.Keyboard = keyboard;
        }

        /// <inheritdoc />
        protected override void UpdateInputState() {
            this.InputState = new InputState(this.Mouse.State, this.Keyboard.GetState(), this.InputState);
        }
    }
}