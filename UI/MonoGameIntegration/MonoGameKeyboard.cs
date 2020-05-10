// Much of the code in this file is originally from MarcStan's MonoGame.Framework.WpfInterop library
// located at github.com/MarcStan/monogame-framework-wpfinterop/ It has been modified to fit my
// specific needs, but full credit to that repository.

namespace Macabre2D.UI.MonoGameIntegration {

    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows;
    using Keyboard = System.Windows.Input.Keyboard;

    public sealed class MonoGameKeyboard {
        private readonly FrameworkElement _focusElement;

        public MonoGameKeyboard(FrameworkElement focusElement) {
            this._focusElement = focusElement ?? throw new ArgumentNullException(nameof(focusElement));
            Instance = this;
        }

        public static MonoGameKeyboard Instance { get; private set; }

        public KeyboardState GetState() {
            if (this._focusElement.IsMouseDirectlyOver && Keyboard.FocusedElement != this._focusElement) {
                // we assume the user wants keyboard input into the control when his mouse is over
                // it in order for the events to register we must focus it

                if (WindowHelper.IsControlOnActiveWindow(this._focusElement)) {
                    // however, only focus if we are the active window, otherwise the window will
                    // become active and pop into foreground just by hovering the mouse over the
                    // game panel

                    //finally check if user wants us to focus already on mouse over
                    // otherwise, don't focus (and let WpfMouse manually set focus)
                    //if (this._focusElement.FocusOnMouseOver) {
                    this._focusElement.Focus();
                    //}
                }
            }
            return new KeyboardState(GetKeys(this._focusElement));
        }

        private static Keys[] GetKeys(IInputElement focusElement) {
            // the buffer must be exactly 256 bytes long as per API definition
            var keyStates = new byte[256];

            if (!NativeGetKeyboardState(keyStates)) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var pressedKeys = new List<Keys>();
            if (focusElement.IsKeyboardFocused) {
                // skip the first 8 entries as they are actually mouse events and not keyboard keys
                const int skipMouseKeys = 8;
                for (var i = skipMouseKeys; i < keyStates.Length; i++) {
                    var key = keyStates[i];

                    //Logical 'and' so we can drop the low-order bit for toggled keys, else that key will appear with the value 1!
                    if ((key & 0x80) != 0) {
                        //This is just for a short demo, you may want this to return
                        //multiple keys!
                        if (key != 0) {
                            pressedKeys.Add((Keys)i);
                        }
                    }
                }
            }

            return pressedKeys.ToArray();
        }

        [DllImport("user32.dll", EntryPoint = "GetKeyboardState", SetLastError = true)]
        private static extern bool NativeGetKeyboardState([Out] byte[] keyStates);
    }
}