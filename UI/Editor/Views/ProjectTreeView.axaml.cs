namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.UI.Common;
using ReactiveUI;

public partial class ProjectTreeView : UserControl {
    public static readonly DirectProperty<ProjectTreeView, ProjectTreeViewModel> ViewModelProperty =
        AvaloniaProperty.RegisterDirect<ProjectTreeView, ProjectTreeViewModel>(
            nameof(ViewModel),
            editor => editor.ViewModel);

    private Guid _dragTarget;

    public ProjectTreeView() {
        this.ViewModel = Resolver.Resolve<ProjectTreeViewModel>();
        this.RenameCommand = ReactiveCommand.Create<TreeView>(this.Rename);
        this.InitializeComponent();
        this.AddHandler(DragDrop.DropEvent, this.Drop);
    }

    public ICommand RenameCommand { get; }

    public ProjectTreeViewModel ViewModel { get; }

    private async void Drop(object sender, DragEventArgs e) {
        if (e.Source is Control { DataContext: IContentDirectory targetDirectory } &&
            e.Data.Get(string.Empty) is IContentNode sourceNode) {
            await this.ViewModel.MoveNode(sourceNode, targetDirectory);
        }

        this._dragTarget = Guid.Empty;
    }

    private async void Node_OnPointerMoved(object sender, PointerEventArgs e) {
        if (this._dragTarget != Guid.Empty && sender is Control { DataContext: IContentNode sourceNode } && sourceNode.Id == this._dragTarget) {
            this._dragTarget = sourceNode.Id;
            var dragData = new GenericDataObject(sourceNode, sourceNode.Name);
            await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
        }
    }

    private void Node_OnPointerPressed(object sender, PointerPressedEventArgs e) {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed && sender is Control { DataContext: IContentNode sourceNode }) {
            this._dragTarget = sourceNode.Id;
        }
    }

    private void Node_OnPointerReleased(object sender, PointerReleasedEventArgs e) {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased) {
            this._dragTarget = Guid.Empty;
        }
    }

    private void Rename(TreeView treeView) {
        if (treeView.SelectedItem != null && this.ViewModel.ContentService.Selected != null) {
            var editableItem = treeView.GetLogicalDescendants().OfType<EditableSelectableItem>().FirstOrDefault(x => x.DataContext == this.ViewModel.ContentService.Selected);

            if (editableItem != null && editableItem.EditCommand.CanExecute(null)) {
                editableItem.EditCommand.Execute(null);
            }
        }
    }
}