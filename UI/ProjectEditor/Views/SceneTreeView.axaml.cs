namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views {
    using System;
    using System.Collections.Generic;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.ViewModels;
    using Macabresoft.Macabre2D.UI.ProjectEditor.Helpers;

    public class SceneTreeView : UserControl {
        public static readonly DirectProperty<SceneTreeView, IReadOnlyCollection<IControl>> AddMenuItemsProperty =
            AvaloniaProperty.RegisterDirect<SceneTreeView, IReadOnlyCollection<IControl>>(
                nameof(AddMenuItems),
                editor => editor.AddMenuItems);

        public static readonly DirectProperty<SceneTreeView, SceneTreeViewModel> ViewModelProperty =
            AvaloniaProperty.RegisterDirect<SceneTreeView, SceneTreeViewModel>(
                nameof(TreeViewModel),
                editor => editor.TreeViewModel);

        private Guid _dragTarget;

        public SceneTreeView() {
            this.DataContext = Resolver.Resolve<SceneTreeViewModel>();
            this.InitializeComponent();
            this.AddHandler(DragDrop.DropEvent, this.Drop);
            this.AddMenuItems = MenuItemHelper.CreateAddMenuItems(this.TreeViewModel.EntityService.AvailableTypes, true);
        }

        public IReadOnlyCollection<IControl> AddMenuItems { get; }

        public SceneTreeViewModel TreeViewModel => this.DataContext as SceneTreeViewModel;

        private void Drop(object sender, DragEventArgs e) {
            if (e.Source is IControl { DataContext: IEntity targetEntity } &&
                e.Data.Get(string.Empty) is IEntity sourceEntity) {
                this.TreeViewModel.MoveEntity(sourceEntity, targetEntity);
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
}