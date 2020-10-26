namespace Macabresoft.Macabre2D.Editor.AvaloniaInterop {

    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// A <see cref="IGame" /> that can run within Avalonia.
    /// </summary>
    public interface IAvaloniaGame : IGame, IDisposable {

        /// <summary>
        /// Initializes the specified mouse.
        /// </summary>
        /// <param name="mouse">The mouse.</param>
        /// <param name="keyboard">The keyboard.</param>
        void Initialize(MonoGameMouse mouse, MonoGameKeyboard keyboard);

        /// <summary>
        /// Runs one frame.
        /// </summary>
        void RunOneFrame();
    }

    /// <summary>
    /// A minimal instance of <see cref="Game" /> that is run for Avalonia.
    /// </summary>
    public class AvaloniaGame : BaseGame, IAvaloniaGame {

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

        /// <inheritdoc />
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