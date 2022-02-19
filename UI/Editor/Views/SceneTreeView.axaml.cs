namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;
using DynamicData;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;

public class SceneTreeView : UserControl {
    public static readonly Thickness DefaultPadding = new(2D);
    public static readonly Thickness EmptyThickness = new(0D);
    private static readonly Thickness InsertBelowHighlightPadding = new(2D, 2D, 2D, 0D);
    private static readonly Thickness InsertBelowHighlightThickness = new(0D, 0D, 0D, 2D);
    
    public static readonly DirectProperty<SceneTreeView, object> DraggedObjectProperty =
        AvaloniaProperty.RegisterDirect<SceneTreeView, object>(
            nameof(DraggedObject),
            control => control.DraggedObject,
            (control, value) => control.DraggedObject = value);
    
    private TreeViewItem _currentDropTarget;
    private object _draggedObject;
    private bool _isDragging;

    public static IMultiValueConverter AllowDropConverter { get; } = new SceneTreeAllowDropConverter();
    
    public SceneTreeView() {
        this.ViewModel = Resolver.Resolve<SceneTreeViewModel>();
        this.InitializeComponent();
        this.AddHandler(DragDrop.DropEvent, this.Drop);
        this.AddHandler(DragDrop.DragOverEvent, this.Drag);
    }

    public SceneTreeViewModel ViewModel { get; }

    public object DraggedObject {
        get => this._draggedObject;
        set => this.SetAndRaise(DraggedObjectProperty, ref this._draggedObject, value);
    }

    private bool CanInsert(IControl target) {
        return target is { DataContext: EntityCollection } || target is { DataContext: IEntity entity } && entity != this.DraggedObject;
    }

    private void Drag(object sender, DragEventArgs e) {
        if (e.Source is IControl { DataContext: { } } control) {
            this.ResetDropTarget(control, e);
        }
    }

    private void Drop(object sender, DragEventArgs e) {
        if (e.Source is IControl control &&
            e.Data.Get(string.Empty) is Guid sourceEntityId &&
            this.ViewModel.EntityService.Selected?.Id == sourceEntityId) {
            if (!this.IsInsert(e)) {
                switch (control.DataContext) {
                    case IEntity targetEntity:
                        this.ViewModel.MoveEntity(this.ViewModel.EntityService.Selected, targetEntity);
                        break;
                    case EntityCollection:
                        this.ViewModel.MoveEntity(this.ViewModel.EntityService.Selected, this.ViewModel.SceneService.CurrentScene);
                        break;
                }
            }
            else {
                switch (control.DataContext) {
                    case IEntity entity when !Entity.IsNullOrEmpty(entity.Parent, out var parent): {
                        var index = parent.Children.IndexOf(entity);

                        if (parent == this.ViewModel.EntityService.Selected.Parent && parent.Children.IndexOf(this.ViewModel.EntityService.Selected) < index) {
                            index -= 1;
                        }

                        this.ViewModel.MoveEntity(this.ViewModel.EntityService.Selected, parent, index + 1);
                        break;
                    }
                    case EntityCollection:
                        this.ViewModel.MoveEntity(this.ViewModel.EntityService.Selected, this.ViewModel.SceneService.CurrentScene, 0);
                        break;
                }
            }
        }

        this._isDragging = false;
        this.DraggedObject = null;
        this.ResetDropTarget(null, null);
    }

    private async void Entity_OnPointerMoved(object sender, PointerEventArgs e) {
        if (!this._isDragging && this.DraggedObject != null && sender is IControl { DataContext: IEntity entity } control && entity == this.DraggedObject) {
            this._isDragging = true;
            var dragData = new GenericDataObject(entity.Id, entity.Name);
            await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
        }
    }

    private void Entity_OnPointerPressed(object sender, PointerPressedEventArgs e) {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed && sender is IControl { DataContext: IEntity entity }) {
            this.DraggedObject = entity;
        }
    }

    private void Entity_OnPointerReleased(object sender, PointerReleasedEventArgs e) {
        if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased) {
            this._isDragging = false;
            this.DraggedObject = null;
            this.ResetDropTarget(null, null);
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private bool IsInsert(DragEventArgs e) {
        var result = false;

        if (e != null && this._currentDropTarget is { DataContext: not IScene }) {
            IControl toCheck = null;

            if (this._currentDropTarget.DataContext is EntityCollection) {
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

    private void ResetDropTarget(IControl newTarget, DragEventArgs e) {
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

                if (draggedObject is IEntity && dropTarget is IEntity or EntityCollection && dropTarget != draggedObject) {
                    return true;
                }
            }

            return false;
        }
    }
}