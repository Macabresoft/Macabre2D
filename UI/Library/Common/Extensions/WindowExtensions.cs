namespace Macabre2D.UI.Library.Common.Extensions {

    using System.Windows;

    public static class WindowExtensions {

        public static bool SimpleShowDialog(this Window window) {
            var result = window.ShowDialog();
            return result.HasValue && result.Value;
        }
    }
}