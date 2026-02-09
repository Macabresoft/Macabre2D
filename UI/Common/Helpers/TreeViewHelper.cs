namespace Macabre2D.UI.Common;

using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using ReactiveUI;

/// <summary>
/// Helper for <see cref="TreeView"/>
/// </summary>
public static class TreeViewHelper {

    /// <summary>
    /// Static constructor for <see cref="TreeViewHelper"/>.
    /// </summary>
    static TreeViewHelper() {
        CollapseAllCommand = ReactiveCommand.Create<TreeView>(x => SetIsExpanded(x, false));
        ExpandAllCommand = ReactiveCommand.Create<TreeView>(x => SetIsExpanded(x, true));
    }
    
    /// <summary>
    /// A command to expand all nodes in a <see cref="TreeView"/>.
    /// </summary>
    public static ICommand ExpandAllCommand { get; }
    
    /// <summary>
    /// A command to collapse all nodes in a <see cref="TreeView"/>.
    /// </summary>
    public static ICommand CollapseAllCommand { get; }
    
    private static void SetIsExpanded(TreeView treeView, bool isExpanded) {
        if (treeView != null) {
            foreach (var treeViewItem in treeView.GetLogicalDescendants().OfType<TreeViewItem>()) {
                treeViewItem.IsExpanded = isExpanded;
            }
        }
    }
}