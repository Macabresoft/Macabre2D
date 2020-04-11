// Much of the code in this file is originally from MarcStan's MonoGame.Framework.WpfInterop library
// located at github.com/MarcStan/monogame-framework-wpfinterop/ It has been modified to fit my
// specific needs, but full credit to that repository.

namespace Macabre2D.UI.MonoGameIntegration {

    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    public sealed class MonoGameMouse {
        private readonly FrameworkElement _focusElement;

        private bool _captureMouseWithin = true;
        private MouseState _mouseState;

        /// <summary>
        /// Creates a new instance of the mouse helper.
        /// </summary>
        /// <param name="focusElement">
        /// The element that will be used as the focus point. Provide your implementation of <see
        /// cref="WpfGame"/> here.
        /// </param>
        public MonoGameMouse(FrameworkElement focusElement) {
            this._focusElement = focusElement ?? throw new ArgumentNullException(nameof(focusElement));
            this._focusElement.MouseWheel += this.HandleMouse;
            // movement
            this._focusElement.MouseMove += this.HandleMouse;
            this._focusElement.MouseEnter += this.HandleMouse;
            this._focusElement.MouseLeave += this.HandleMouse;
            // clicks
            this._focusElement.MouseLeftButtonDown += this.HandleMouse;
            this._focusElement.MouseLeftButtonUp += this.HandleMouse;
            this._focusElement.MouseRightButtonDown += this.HandleMouse;
            this._focusElement.MouseRightButtonUp += this.HandleMouse;
        }

        /// <summary>
        /// Gets or sets the mouse capture behaviour. If true, the mouse will be captured within the
        /// control. This means that the control will still capture mouse events when the user drags
        /// the mouse outside the control. E.g. mouse down on game window, mouse drag to outside of
        /// the window, mouse release -&gt; the game will still register the mouse release. The
        /// downside is that overlayed elements (textbox, etc.) will never be able to receive focus.
        /// If false, mouse events outside the game window are never registered. E.g. mouse down on
        /// game window, mouse drag to outside of the window, mouse release -&gt; the game will
        /// still thing the mouse is pressed until the cursor enters the window again. The upside is
        /// that overlayed controls (e.g. textboxes) can receive focus and input. Defaults to true.
        /// </summary>
        public bool CaptureMouseWithin {
            get {
                return this._captureMouseWithin;
            }
            set {
                if (!value) {
                    if (
                        this._focusElement.IsMouseCaptured) {
                        this._focusElement.ReleaseMouseCapture();
                    }
                }

                this._captureMouseWithin = value;
            }
        }

        public MouseState GetState() {
            return this._mouseState;
        }

        public void SetCursor(int x, int y) {
            var p = this._focusElement.PointToScreen(new Point(x, y));
            SetCursorPos((int)p.X, (int)p.Y);
        }

        private static double Clamp(double v, int min, double max) {
            return v < min ? min : (v > max ? max : v);
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        private void HandleMouse(object sender, MouseEventArgs e) {
            if (e.Handled) {
                return;
            }

            var pos = e.GetPosition(this._focusElement);

            if (!this.CaptureMouseWithin) {
                // clamp as fast moving mouse will somtimes still report values outside the control
                // without explicit capture (e.g. negative values)
                pos = new Point(Clamp(pos.X, 0, this._focusElement.ActualWidth), Clamp(pos.Y, 0, this._focusElement.ActualHeight));
            }
            if (this._focusElement.IsMouseDirectlyOver && System.Windows.Input.Keyboard.FocusedElement != this._focusElement) {
                if (WindowHelper.IsControlOnActiveWindow(this._focusElement)) {
                    // however, only focus if we are the active window, otherwise the window will
                    // become active and pop into foreground just by hovering the mouse over the
                    // game panel

                    //finally check if user wants us to focus already on mouse over
                    ////if (this._focusElement.FocusOnMouseOver) {
                    this._focusElement.Focus();
                    ////}
                    ////else {
                    ////    // otherwise focus only when the user clicks into the game on windows this
                    ////    // behaviour doesn't require an explicit left click instead, left, middle,
                    ////    // right and even xbuttons work (the only thing that doesn't trigger focus
                    ////    // is scrolling) so mimic that exactly
                    ////    if (e.LeftButton == MouseButtonState.Pressed ||
                    ////        e.RightButton == MouseButtonState.Pressed ||
                    ////        e.MiddleButton == MouseButtonState.Pressed ||
                    ////        e.XButton1 == MouseButtonState.Pressed ||
                    ////        e.XButton2 == MouseButtonState.Pressed) {
                    ////        this._focusElement.Focus();
                    ////    }
                    ////}
                }
            }

            if ((!this._focusElement.IsMouseDirectlyOver || this._focusElement.IsMouseCaptured) && this.CaptureMouseWithin) {
                // IsMouseDirectlyOver always returns true if the mouse is captured, so we need to
                // do our own hit testing if the Mouse is captured to find out whether it is
                // actually over the control or not
                if (this._focusElement.IsMouseCaptured) {
                    // apparently all WPF IInputElements are always Visuals, so we can cast directly
                    var v = (Visual)this._focusElement;
                    var hit = false;
                    VisualTreeHelper.HitTest(v, filterTarget => HitTestFilterBehavior.Continue, target => {
                        if (target.VisualHit == this._focusElement) {
                            // our actual element was hit
                            hit = true;
                        }
                        return HitTestResultBehavior.Continue;
                    }, new PointHitTestParameters(pos));

                    if (!hit) {
                        // outside the hitbox

                        // when the mouse is leaving the control we need to register button releases
                        // when the user clicks in the control, holds the button and moves it
                        // outside the control and releases there it normally does not registered
                        // the control would thus think that the button is still pressed using
                        // capture allows us to receive this event, propagate it and then free the mouse

                        this._mouseState = new MouseState(this._mouseState.X, this._mouseState.Y, this._mouseState.ScrollWheelValue,
                            (ButtonState)e.LeftButton, (ButtonState)e.MiddleButton, (ButtonState)e.RightButton, (ButtonState)e.XButton1,
                            (ButtonState)e.XButton2);
                        // only release if LeftMouse is up
                        if (e.LeftButton == MouseButtonState.Released) {
                            this._focusElement.ReleaseMouseCapture();
                        }
                        e.Handled = true;
                        return;
                    }
                    // inside the control and captured -> still requires full update, so run code
                    // below and don't return right away
                }
                else {
                    // mouse is outside the control and not captured, so don't update the mouse state
                    return;
                }
            }

            if (this.CaptureMouseWithin) {
                // capture the mouse, this allows receiving of mouse event while the mouse is
                // leaving the control: https://msdn.microsoft.com/en-us/library/ms591452(v=vs.110).aspx
                if (!this._focusElement.IsMouseCaptured) {
                    // however, only focus if we are the active window, otherwise the window will
                    // become active and pop into foreground just by hovering the mouse over the
                    // game panel
                    if (WindowHelper.IsControlOnActiveWindow(this._focusElement)) {
                        // however, only focus if we are the active window, otherwise the window
                        // will become active while remaining in the background
                        this._focusElement.CaptureMouse();
                    }
                    else {
                        // don't update mouse events if we are just hovering over different window
                        return;
                    }
                }
            }
            else {
                if (this._focusElement.IsFocused && !WindowHelper.IsControlOnActiveWindow(this._focusElement)) {
                    // don't update mouse events if we are just hovering over different window
                    return;
                }
            }

            e.Handled = true;
            var m = this._mouseState;
            var w = e as MouseWheelEventArgs;
            this._mouseState = new MouseState((int)pos.X, (int)pos.Y, m.ScrollWheelValue + (w?.Delta ?? 0), (ButtonState)e.LeftButton, (ButtonState)e.MiddleButton, (ButtonState)e.RightButton, (ButtonState)e.XButton1, (ButtonState)e.XButton2);
        }
    }
}