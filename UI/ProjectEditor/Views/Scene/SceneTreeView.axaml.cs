namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Scene {
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Scene;

    public class SceneTreeView : UserControl {
        public static readonly DirectProperty<SceneTreeView, SceneTreeViewModel> ViewModelProperty =
            AvaloniaProperty.RegisterDirect<SceneTreeView, SceneTreeViewModel>(
                nameof(ViewModel),
                editor => editor.ViewModel);

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
        }

        private async void Entity_OnPointerPressed(object sender, PointerPressedEventArgs e) {
            if (sender is IControl { DataContext: IEntity entity }) {
                var dragData = new GenericDataObject(entity, entity.Name);
                await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
                // TODO: could get the result from DoDragDrop and write a message in a status bar
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void SetupDragAndDrop() {
        }
    }
}