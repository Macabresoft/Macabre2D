namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using DynamicData;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using ReactiveUI;

public partial class SceneTreeView : UserControl {
    public static readonly Thickness DefaultPadding = new(2D);

    public static readonly DirectProperty<SceneTreeView, INameable> DraggedObjectProperty =
        AvaloniaProperty.RegisterDirect<SceneTreeView, INameable>(
            nameof(DraggedObject),
            control => control.DraggedObject,
            (control, value) => control.DraggedObject = value);

    public static readonly Thickness EmptyThickness = new(0D);
    private static readonly Thickness InsertBelowHighlightPadding = new(2D, 2D, 2D, 0D);
    private static readonly Thickness InsertBelowHighlightThickness = new(0D, 0D, 0D, 2D);

    private TreeViewItem _currentDropTarget;
    private INameable _draggedObject;
    private bool _isDragging;

    public SceneTreeView() {
        this.ViewModel = Resolver.Resolve<SceneTreeViewModel>();
        this.RenameCommand = ReactiveCommand.Create<TreeView>(this.Rename);

        this.InitializeComponent();

        this.AddHandler(DragDrop.DropEvent, this.Drop);
        this.AddHandler(DragDrop.DragOverEvent, this.Drag);
    }

    public static IMultiValueConverter AllowDropConverter { get; } = new SceneTreeAllowDropConverter();

    public ICommand RenameCommand { get; }

    public SceneTreeViewModel ViewModel { get; }

    public INameable DraggedObject {
        get => this._draggedObject;
        set => this.SetAndRaise(DraggedObjectProperty, ref this._draggedObject, value);
    }

    private bool CanInsert(Control target) =>
        target is { DataContext: IEntity or IGameSystem or EntityCollection or SystemCollection } &&
        target.DataContext != this.DraggedObject;

    private void Drag(object sender, DragEventArgs e) {
        if (e.Source is Control { DataContext: not null } control) {
            this.ResetDropTarget(control, e);
        }
    }

    private void Drop(object sender, DragEventArgs e) {
        if (e.Source is Control control) {
            if (!this.IsInsert(e)) {
                switch (control.DataContext) {
                    case IEntity targetEntity when this.DraggedObject is IEntity:
                        this.ViewModel.MoveEntity(this.ViewModel.EntityService.Selected, targetEntity);
                        break;
                    case EntityCollection when this.DraggedObject is IEntity:
                        this.ViewModel.MoveEntity(this.ViewModel.EntityService.Selected, this.ViewModel.SceneService.CurrentlyEditing);
                        break;
                }
            }
            else {
                int index;
                switch (control.DataContext) {
                    case IEntity entity when this.DraggedObject is IEntity draggedEntity && !Entity.IsNullOrEmpty(entity.Parent, out var parent): {
                        index = parent.Children.IndexOf(entity);

                        if (entity.Parent != draggedEntity.Parent || index < parent.Children.IndexOf(draggedEntity)) {
                            index++;
                        }

                        this.ViewModel.MoveEntity(draggedEntity, parent, index);
                        break;
                    }
                    case EntityCollection when this.DraggedObject is IEntity draggedEntity:
                        this.ViewModel.MoveEntity(draggedEntity, this.ViewModel.SceneService.CurrentlyEditing, 0);
                        break;
                    case IGameSystem targetSystem when this.DraggedObject is IGameSystem draggedSystem:
                        if (this.ViewModel.SceneService.CurrentScene is { } scene) {
                            index = scene.Systems.IndexOf(targetSystem);

                            if (index < scene.Systems.IndexOf(draggedSystem)) {
                                index++;
                            }

                            this.ViewModel.MoveSystem(draggedSystem, index);
                        }

                        break;
                    case SystemCollection when this.DraggedObject is IGameSystem draggedSystem:
                        this.ViewModel.MoveSystem(draggedSystem, 0);
                        break;
                }
            }
        }

        this._isDragging = false;
        this.DraggedObject = null;
        this.ResetDropTarget(null, null);
    }

    private void FilteredNode_OnDoubleTapped(object sender, TappedEventArgs e) {
        this.ViewModel.ClearFilterCommand.Execute(null);
    }

    private bool IsInsert(DragEventArgs e) {
        var result = false;

        if (e != null && this._currentDropTarget is { DataContext: not IScene }) {
            Control toCheck = null;

            if (this._currentDropTarget.DataContext is EntityCollection or SystemCollection) {
                toCheck = this._currentDropTarget.FindDescendantOfType<Border>();
            }

            toCheck ??= this._currentDropTarget;

            var (_, y) = e.GetPosition(toCheck);
            var (_, height) = toCheck.Bounds.Size;
            var margin = height * 0.33;
            result = y > height - margin;
        }

        return result;
    }

    private void Rename(TreeView treeView) {
        if (treeView.SelectedItem != null && this.ViewModel.EntityService.Selected != null) {
            var editableItem = treeView.GetLogicalDescendants().OfType<EditableSelectableItem>().FirstOrDefault(x => x.DataContext == this.ViewModel.EntityService.Selected);

            if (editableItem != null && editableItem.EditCommand.CanExecute(null)) {
                editableItem.EditCommand.Execute(null);
            }
        }
    }

    private void ResetDropTarget(Control newTarget, DragEventArgs e) {
        if (this._currentDropTarget != null) {
            this._currentDropTarget.BorderThickness = EmptyThickness;
            this._currentDropTarget.Padding = DefaultPadding;
            this._currentDropTarget = null;
        }

        if (this.CanInsert(newTarget) && newTarget.FindAncestor<TreeViewItem>() is var treeViewTarget) {
            this._currentDropTarget = treeViewTarget;
            this.SetDropHighlight(e);
        }
    }

    private void SetDropHighlight(DragEventArgs e) {
        if (this.IsInsert(e)) {
            this._currentDropTarget.BorderThickness = InsertBelowHighlightThickness;
            this._currentDropTarget.Padding = InsertBelowHighlightPadding;
        }
        else if (this._currentDropTarget != null) {
            this._currentDropTarget.BorderThickness = EmptyThickness;
            this._currentDropTarget.Padding = DefaultPadding;
        }
    }

    private async void TreeNode_OnPointerMoved(object sender, PointerEventArgs e) {
        if (!this._isDragging && this.DraggedObject != null &&
            sender is Control { DataContext: IGameSystem or IEntity and not IScene } control &&
            control.DataContext == this.DraggedObject) {
            this._isDragging = true;
            var dragData = new GenericDataObject(this.DraggedObject, this.DraggedObject.Name);
            await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
        }
    }

    private void TreeNode_OnPointerPressed(object sender, PointerPressedEventArgs e) {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed &&
            sender is Control { DataContext: IGameSystem or IEntity and not IScene } control) {
            this.DraggedObject = control.DataContext as INameable;
        }
    }

    private void TreeNode_OnPointerReleased(object sender, PointerReleasedEventArgs e) {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased) {
            this._isDragging = false;
            this.DraggedObject = null;
            this.ResetDropTarget(null, null);
        }
    }

    private void TreeView_OnLostFocus(object sender, RoutedEventArgs e) {
        if (this._currentDropTarget != null) {
            this.ResetDropTarget(null, null);
        }
    }

    private class SceneTreeAllowDropConverter : IMultiValueConverter {
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture) {
            if (values.Count == 2) {
                var draggedObject = values[0];
                var dropTarget = values[1];
                return dropTarget != draggedObject &&
                       (draggedObject is IEntity && dropTarget is IEntity or EntityCollection ||
                        draggedObject is IGameSystem && dropTarget is IGameSystem or SystemCollection);
            }

            return false;
        }
    }
}