namespace Macabre2D.Engine.Windows.Common {

    using System.Windows.Controls;

    public static class TreeViewExtensions {

        public static TreeViewItem FindTreeViewItem(this ItemsControl treeViewOrTreeViewItem, object item) {
            if (treeViewOrTreeViewItem != null) {
                if (treeViewOrTreeViewItem.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem treeViewItem) {
                    return treeViewItem;
                }

                foreach (var child in treeViewOrTreeViewItem.Items) {
                    var childTreeViewItem = treeViewOrTreeViewItem.ItemContainerGenerator.ContainerFromItem(child) as TreeViewItem;

                    if (child == item) {
                        return childTreeViewItem;
                    }
                    else if (FindTreeViewItem(childTreeViewItem, item) is TreeViewItem deepTreeViewItem) {
                        return deepTreeViewItem;
                    }
                }
            }

            return null;
        }
    }
}