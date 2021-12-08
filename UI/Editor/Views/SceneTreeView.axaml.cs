namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;

public class SceneTreeView : UserControl {
    public static readonly DirectProperty<SceneTreeView, IReadOnlyCollection<IControl>> AddEntityMenuItemsProperty =
        AvaloniaProperty.RegisterDirect<SceneTreeView, IReadOnlyCollection<IControl>>(
            nameof(AddEntityMenuItems),
            editor => editor.AddEntityMenuItems);

    public static readonly DirectProperty<SceneTreeView, IReadOnlyCollection<IControl>> AddSystemMenuItemsProperty =
        AvaloniaProperty.RegisterDirect<SceneTreeView, IReadOnlyCollection<IControl>>(
            nameof(AddSystemMenuItems),
            editor => editor.AddSystemMenuItems);

    public static readonly DirectProperty<SceneTreeView, SceneTreeViewModel> ViewModelProperty =
        AvaloniaProperty.RegisterDirect<SceneTreeView, SceneTreeViewModel>(
            nameof(ViewModel),
            editor => editor.ViewModel);

    private Guid _dragTarget;

    public SceneTreeView() {
        this.ViewModel = Resolver.Resolve<SceneTreeViewModel>();
        this.InitializeComponent();
        this.AddHandler(DragDrop.DropEvent, this.Drop);
        this.AddEntityMenuItems = MenuItemHelper.CreateAddMenuItems(this.ViewModel.EntityService.AvailableTypes, true);
        this.AddSystemMenuItems = MenuItemHelper.CreateAddMenuItems(this.ViewModel.SystemService.AvailableTypes, true);
    }

    public IReadOnlyCollection<IControl> AddEntityMenuItems { get; }

    public IReadOnlyCollection<IControl> AddSystemMenuItems { get; }

    public SceneTreeViewModel ViewModel { get; }

    private void Drop(object sender, DragEventArgs e) {
        if (e.Source is IControl { DataContext: IEntity targetEntity } &&
            e.Data.Get(string.Empty) is IEntity sourceEntity) {
            this.ViewModel.MoveEntity(sourceEntity, targetEntity);
        }

        this._dragTarget = Guid.Empty;
    }

    private async void Entity_OnPointerMoved(object sender, PointerEventArgs e) {
        if (this._dragTarget != Guid.Empty && sender is IControl { DataContext: IEntity entity } && entity.Id == this._dragTarget) {
            var dragData = new GenericDataObject(entity, entity.Name);
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
            this._dragTarget = Guid.Empty;
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}