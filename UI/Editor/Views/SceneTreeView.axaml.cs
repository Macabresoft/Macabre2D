namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;

public class SceneTreeView : UserControl {
    public static readonly DirectProperty<SceneTreeView, SceneTreeViewModel> ViewModelProperty =
        AvaloniaProperty.RegisterDirect<SceneTreeView, SceneTreeViewModel>(
            nameof(ViewModel),
            editor => editor.ViewModel);

    private TreeViewItem _currentDropTarget;

    private Guid _dragTarget;
    private bool _isDragging;

    public SceneTreeView() {
        this.ViewModel = Resolver.Resolve<SceneTreeViewModel>();
        this.InitializeComponent();
        this.AddHandler(DragDrop.DropEvent, this.Drop);
        this.AddHandler(DragDrop.DragOverEvent, this.Drag);
    }

    public SceneTreeViewModel ViewModel { get; }

    private void Drag(object sender, DragEventArgs e) {
        if (e.Source is IControl { DataContext: { } } control) {
            this.ResetDropTarget(control);
            this.SetDropHighlight(e);
        }
    }

    private void Drop(object sender, DragEventArgs e) {
        if (e.Source is IControl control &&
            e.Data.Get(string.Empty) is Guid sourceEntityId &&
            this.ViewModel.EntityService.Selected?.Id == sourceEntityId) {
            switch (control.DataContext) {
                case IEntity targetEntity:
                    this.ViewModel.MoveEntity(this.ViewModel.EntityService.Selected, targetEntity);
                    break;
                case EntityCollection:
                    this.ViewModel.MoveEntity(this.ViewModel.EntityService.Selected, this.ViewModel.SceneService.CurrentScene);
                    break;
            }
        }

        this._isDragging = false;
        this._dragTarget = Guid.Empty;
        this.ResetDropTarget(null);
    }

    private async void Entity_OnPointerMoved(object sender, PointerEventArgs e) {
        if (!this._isDragging && this._dragTarget != Guid.Empty && sender is IControl { DataContext: IEntity entity } control && entity.Id == this._dragTarget) {
            this._isDragging = true;
            var dragData = new GenericDataObject(entity.Id, entity.Name);
            await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
        }
    }

    private void Entity_OnPointerPressed(object sender, PointerPressedEventArgs e) {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed && sender is IControl { DataContext: IEntity entity }) {
            this._dragTarget = entity.Id;
        }
    }

    private void Entity_OnPointerReleased(object sender, PointerReleasedEventArgs e) {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased) {
            this._isDragging = false;
            this._dragTarget = Guid.Empty;
            this.ResetDropTarget(null);
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void ResetDropTarget(IControl newTarget) {
        var treeViewItem = newTarget?.FindAncestor<TreeViewItem>();
        if (treeViewItem != this._currentDropTarget) {
            if (this._currentDropTarget != null) {
                var borders = this._currentDropTarget.GetVisualDescendants().OfType<Border>().ToList();
                var topBorder = borders.FirstOrDefault(x => x.Name == "Top");
                if (topBorder != null) {
                    topBorder.IsVisible = false;
                }

                var middleBorder = borders.FirstOrDefault(x => x.Name == "Middle");
                if (middleBorder != null) {
                    middleBorder.IsVisible = false;
                }

                var bottomBorder = borders.FirstOrDefault(x => x.Name == "Bottom");
                if (bottomBorder != null) {
                    bottomBorder.IsVisible = false;
                }
            }

            this._currentDropTarget = treeViewItem;
        }
    }

    private void SetDropHighlight(DragEventArgs e) {
        if (this._currentDropTarget != null) {
            var (_, y) = e.GetPosition(this._currentDropTarget);
            var (_, height) = this._currentDropTarget.Bounds.Size;
            var margin = height * 0.1;
            var borders = this._currentDropTarget.GetVisualDescendants().OfType<Border>().ToList();
            var topBorder = borders.FirstOrDefault(x => x.Name == "Top");
            var middleBorder = borders.FirstOrDefault(x => x.Name == "Middle");
            var bottomBorder = borders.FirstOrDefault(x => x.Name == "Bottom");

            if (topBorder != null && middleBorder != null && bottomBorder != null) {
                if (y < margin) {
                    topBorder.IsVisible = true;
                    middleBorder.IsVisible = false;
                    bottomBorder.IsVisible = false;
                }
                else if (y > height - margin) {
                    topBorder.IsVisible = false;
                    middleBorder.IsVisible = false;
                    bottomBorder.IsVisible = true;
                }
                else {
                    topBorder.IsVisible = false;
                    middleBorder.IsVisible = true;
                    bottomBorder.IsVisible = false;
                }
            }
        }
    }
}