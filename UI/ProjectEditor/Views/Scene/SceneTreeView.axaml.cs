namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Scene {
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Scene;
    using System;

    public class SceneTreeView : UserControl {
        public static readonly DirectProperty<SceneTreeView, SceneTreeViewModel> ViewModelProperty =
            AvaloniaProperty.RegisterDirect<SceneTreeView, SceneTreeViewModel>(
                nameof(ViewModel),
                editor => editor.ViewModel);

        private Guid _dragTarget;
        
        public SceneTreeView() {
            this.DataContext = Resolver.Resolve<SceneTreeViewModel>();
            this.InitializeComponent();
            this.AddHandler(DragDrop.DropEvent, this.Drop);
        }

        public SceneTreeViewModel ViewModel => this.DataContext as SceneTreeViewModel;

        private void Drop(object sender, DragEventArgs e) {
            if (e.Source is IControl { DataContext: IEntity targetEntity } &&
                e.Data.Get(string.Empty) is IEntity sourceEntity) {
                this.ViewModel.MoveEntity(sourceEntity, targetEntity);
            }
            
            this._dragTarget = Guid.Empty;
        }

        private void Entity_OnPointerPressed(object sender, PointerPressedEventArgs e) {
            if (sender is IControl { DataContext: IEntity entity }) {
                this._dragTarget = entity.Id;
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void Entity_OnPointerReleased(object sender, PointerReleasedEventArgs e) {
            this._dragTarget = Guid.Empty;
        }

        private async void Entity_OnPointerMoved(object sender, PointerEventArgs e) {
            if (this._dragTarget != Guid.Empty && sender is IControl { DataContext: IEntity entity } && entity.Id == this._dragTarget) {
                var dragData = new GenericDataObject(entity, entity.Name);
                await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
            }
        }
    }
}