namespace Macabresoft.Macabre2D.UI.SceneEditor {
    using System;
    using System.Collections.Generic;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;

    public class SceneTreeView : UserControl {
        public static readonly DirectProperty<SceneTreeView, IReadOnlyCollection<IControl>> AddMenuItemsProperty =
            AvaloniaProperty.RegisterDirect<SceneTreeView, IReadOnlyCollection<IControl>>(
                nameof(AddMenuItems),
                editor => editor.AddMenuItems);

        public static readonly DirectProperty<SceneTreeView, SceneTreeBaseViewModel> ViewModelProperty =
            AvaloniaProperty.RegisterDirect<SceneTreeView, SceneTreeBaseViewModel>(
                nameof(TreeBaseViewModel),
                editor => editor.TreeBaseViewModel);

        private Guid _dragTarget;

        public SceneTreeView() {
            this.DataContext = Resolver.Resolve<SceneTreeBaseViewModel>();
            this.InitializeComponent();
            this.AddHandler(DragDrop.DropEvent, this.Drop);
            this.AddMenuItems = MenuItemHelper.CreateAddMenuItems(this.TreeBaseViewModel.EntityService.AvailableTypes, true);
        }

        public IReadOnlyCollection<IControl> AddMenuItems { get; }

        public SceneTreeBaseViewModel TreeBaseViewModel => this.DataContext as SceneTreeBaseViewModel;

        private void Drop(object sender, DragEventArgs e) {
            if (e.Source is IControl { DataContext: IEntity targetEntity } &&
                e.Data.Get(string.Empty) is IEntity sourceEntity) {
                this.TreeBaseViewModel.MoveEntity(sourceEntity, targetEntity);
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