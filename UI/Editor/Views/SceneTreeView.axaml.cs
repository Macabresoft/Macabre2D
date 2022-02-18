namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;

public class SceneTreeView : UserControl {
    public static readonly DirectProperty<SceneTreeView, SceneTreeViewModel> ViewModelProperty =
        AvaloniaProperty.RegisterDirect<SceneTreeView, SceneTreeViewModel>(
            nameof(ViewModel),
            editor => editor.ViewModel);

    private static readonly Thickness InsertHighlightThickness = new(0D, 0D, 0D, 2D);
    private static readonly Thickness InsertHighlightPadding = new(2D, 2D, 2D, 0D);
    public static readonly Thickness EmptyThickness = new(0D);
    public static readonly Thickness DefaultPadding = new(2D);

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

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void ResetDropTarget(IControl newTarget, DragEventArgs e) {
        if (this._currentDropTarget != null) {
            this._currentDropTarget.BorderThickness = EmptyThickness;
            this._currentDropTarget.Padding = DefaultPadding;
        }

        this._currentDropTarget = newTarget?.FindAncestor<TreeViewItem>();

        if (this._currentDropTarget?.DataContext is IEntity entity && entity.Id == this._draggedEntityId) {
            this._currentDropTarget = null;
        }
        
        this.SetDropHighlight(e);
    }

    private void SetDropHighlight(DragEventArgs e) {
        if (this._currentDropTarget != null && e != null) {
            var (_, y) = e.GetPosition(this._currentDropTarget);
            var (_, height) = this._currentDropTarget.Bounds.Size;
            var margin = height * 0.33;
            
            if (y > height - margin && this._currentDropTarget.DataContext is not EntityCollection and not IScene) {
                this._currentDropTarget.BorderThickness = InsertHighlightThickness;
                this._currentDropTarget.Padding = InsertHighlightPadding;
            }
            else {
                this._currentDropTarget.BorderThickness = EmptyThickness;
                this._currentDropTarget.Padding = DefaultPadding;
            }
        }
    }

    private void TreeView_OnLostFocus(object sender, RoutedEventArgs e) {
        if (this._currentDropTarget != null) {
            this.ResetDropTarget(null, null);
        }
    }
}