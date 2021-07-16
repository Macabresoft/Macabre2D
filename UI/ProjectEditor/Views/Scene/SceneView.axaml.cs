namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Scene {
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Scene;
    using System;

    public class SceneView : UserControl {
        public static readonly DirectProperty<SceneView, SceneViewModel> ViewModelProperty =
            AvaloniaProperty.RegisterDirect<SceneView, SceneViewModel>(
                nameof(ViewModel),
                editor => editor.ViewModel);

        private Guid _dragTarget;
        
        public SceneView() {
            this.DataContext = Resolver.Resolve<SceneViewModel>();
            this.InitializeComponent();
            this.AddHandler(DragDrop.DropEvent, this.Drop);
        }

        public SceneViewModel ViewModel => this.DataContext as SceneViewModel;

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
            else {
                this._dragTarget = Guid.Empty;
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