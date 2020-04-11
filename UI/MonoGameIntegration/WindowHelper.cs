// Much of the code in this file is originally from MarcStan's MonoGame.Framework.WpfInterop library
// located at github.com/MarcStan/monogame-framework-wpfinterop/ It has been modified to fit my
// specific needs, but full credit to that repository.
namespace Macabre2D.UI.MonoGameIntegration {

    using System;
    using System.Linq;
    using System.Windows;

    internal static class WindowHelper {

        public static bool IsControlOnActiveWindow(IInputElement element) {
            var activeWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            var controlWindow = GetWindowFrom(element);
            return controlWindow == activeWindow;
        }

        private static Window GetWindowFrom(IInputElement focusElement) {
            if (focusElement is FrameworkElement frameworkElement) {
                return Window.GetWindow(frameworkElement);
                // we use D3D11Host which derives from image, so the _focusElement should always be
                // castable to FrameworkElement for us
            }
            else {
                throw new NotSupportedException("Only FrameworkElement is currently supported.");
            }
        }
    }
}