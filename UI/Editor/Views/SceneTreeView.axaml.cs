namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using DynamicData;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;

public class SceneTreeView : UserControl {
    public static readonly Thickness DefaultPadding = new(2D);
    public static readonly Thickness EmptyThickness = new(0D);

    public static readonly DirectProperty<SceneTreeView, SceneTreeViewModel> ViewModelProperty =
        AvaloniaProperty.RegisterDirect<SceneTreeView, SceneTreeViewModel>(
            nameof(ViewModel),
            editor => editor.ViewModel);

    private static readonly Thickness InsertAboveHighlightPadding = new(2D, 0D, 2D, 2D);
    private static readonly Thickness InsertAboveHighlightThickness = new(0D, 2D, 0D, 0D);

    private static readonly Thickness InsertBelowHighlightPadding = new(2D, 2D, 2D, 0D);
    private static readonly Thickness InsertBelowHighlightThickness = new(0D, 0D, 0D, 2D);

    private TreeViewItem _currentDropTarget;
    private Guid _draggedEntityId;
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
            this.ResetDropTarget(control, e);
        }
    }

    private void Drop(object sender, DragEventArgs e) {
        if (e.Source is IControl control &&
            e.Data.Get(string.Empty) is Guid sourceEntityId &&
            this.ViewModel.EntityService.Selected?.Id == sourceEntityId) {
            var insertKind = this.GetInsertKind(e);
            if (insertKind == InsertKind.Child) {
                if (control.DataContext is IEntity targetEntity) {
                    this.ViewModel.MoveEntity(this.ViewModel.EntityService.Selected, targetEntity);
                }
                else if (control.DataContext is EntityCollection) {
                    this.ViewModel.MoveEntity(this.ViewModel.EntityService.Selected, this.ViewModel.SceneService.CurrentScene);
                }
            }
            else if (control.DataContext is IEntity entity && !Entity.IsNullOrEmpty(entity.Parent, out var parent)) {
                var index = parent.Children.IndexOf(entity);

                if (parent == this.ViewModel.EntityService.Selected.Parent && parent.Children.IndexOf(this.ViewModel.EntityService.Selected) < index) {
                    index -= 1;
                }
                
                if (insertKind == InsertKind.Above) {
                    this.ViewModel.MoveEntity(this.ViewModel.EntityService.Selected, parent, index);
                }
                else if (insertKind == InsertKind.Below) {
                    this.ViewModel.MoveEntity(this.ViewModel.EntityService.Selected, parent, index + 1);
                }
            }
        }

        this._isDragging = false;
        this._draggedEntityId = Guid.Empty;
        this.ResetDropTarget(null, null);
    }

    private async void Entity_OnPointerMoved(object sender, PointerEventArgs e) {
        if (!this._isDragging && this._draggedEntityId != Guid.Empty && sender is IControl { DataContext: IEntity entity } control && entity.Id == this._draggedEntityId) {
            this._isDragging = true;
            var dragData = new GenericDataObject(entity.Id, entity.Name);
            await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
        }
    }

    private void Entity_OnPointerPressed(object sender, PointerPressedEventArgs e) {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed && sender is IControl { DataContext: IEntity entity }) {
            this._draggedEntityId = entity.Id;
        }
    }

    private void Entity_OnPointerReleased(object sender, PointerReleasedEventArgs e) {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased) {
            this._isDragging = false;
            this._draggedEntityId = Guid.Empty;
            this.ResetDropTarget(null, null);
        }
    }

    private InsertKind GetInsertKind(DragEventArgs e) {
        var result = InsertKind.Child;
        if (e != null && this._currentDropTarget is { DataContext: not EntityCollection and not IScene }) {
            var (_, y) = e.GetPosition(this._currentDropTarget);
            var (_, height) = this._currentDropTarget.Bounds.Size;
            var margin = height * 0.33;
            if (y < margin) {
                result = InsertKind.Above;
            }
            else if (y > height - margin) {
                result = InsertKind.Below;
            }
        }

        return result;
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void ResetDropTarget(IControl newTarget, DragEventArgs e) {
        if (this._currentDropTarget != null) {
            this._currentDropTarget.BorderThickness = EmptyThickness;
            this._currentDropTarget.Padding = DefaultPadding;
            this._currentDropTarget = null;
        }

        if (newTarget is { DataContext: IEntity entity } && entity.Id != this._draggedEntityId && newTarget.FindAncestor<TreeViewItem>() is var treeViewTarget) {
            this._currentDropTarget = treeViewTarget;
            this.SetDropHighlight(e);
        }
    }

    private void SetDropHighlight(DragEventArgs e) {
        var insertKind = this.GetInsertKind(e);
        switch (insertKind) {
            case InsertKind.Above:
                this._currentDropTarget.BorderThickness = InsertAboveHighlightThickness;
                this._currentDropTarget.Padding = InsertAboveHighlightPadding;
                break;
            case InsertKind.Below:
                this._currentDropTarget.BorderThickness = InsertBelowHighlightThickness;
                this._currentDropTarget.Padding = InsertBelowHighlightPadding;
                break;
            case InsertKind.Child:
            default: {
                if (this._currentDropTarget != null) {
                    this._currentDropTarget.BorderThickness = EmptyThickness;
                    this._currentDropTarget.Padding = DefaultPadding;
                }

                break;
            }
        }
    }

    private void TreeView_OnLostFocus(object sender, RoutedEventArgs e) {
        if (this._currentDropTarget != null) {
            this.ResetDropTarget(null, null);
        }
    }
}