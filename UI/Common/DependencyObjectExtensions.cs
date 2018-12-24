namespace Macabre2D.UI.Common {

    using System.Windows;
    using System.Windows.Media;

    public static class DependencyObjectExtensions {

        public static T FindAncestor<T>(this DependencyObject dependencyObject) where T : DependencyObject {
            dependencyObject = VisualTreeHelper.GetParent(dependencyObject);

            if (dependencyObject is T parent) {
                return parent;
            }
            else {
                return dependencyObject?.FindAncestor<T>();
            }
        }

        public static T GetChildOfType<T>(this DependencyObject dependencyObject) where T : DependencyObject {
            T result = null;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject) && result == null; i++) {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                result = (child as T) ?? child?.GetChildOfType<T>();
            }

            return result;
        }
    }
}