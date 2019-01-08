namespace Macabre2D.UI.Common {

    using System.Windows.Controls;

    public static class TreeViewExtensions {

        public static TreeViewItem FindTreeViewItem(this ItemsControl treeView, object item) {
            if (treeView.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeViewItem) {
                return treeViewItem;
            }

            foreach (var child in treeView.Items) {
                var childTreeViewItem = treeView.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;

                if (FindTreeViewItem(childTreeViewItem, item) is TreeViewItem deepTreeViewItem) {
                    return deepTreeViewItem;
                }
            }

            return null;
        }
    }
}