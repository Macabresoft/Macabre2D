namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Content {
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Content;

    public class ContentTreeView : UserControl {
        public static readonly DirectProperty<ContentTreeView, ContentTreeViewModel> ViewModelProperty =
            AvaloniaProperty.RegisterDirect<ContentTreeView, ContentTreeViewModel>(
                nameof(ViewModel),
                editor => editor.ViewModel);

        public ContentTreeView() {
            this.DataContext = Resolver.Resolve<ContentTreeViewModel>();
            this.InitializeComponent();
            this.AddHandler(DragDrop.DropEvent, this.Drop);
        }

        public ContentTreeViewModel ViewModel => this.DataContext as ContentTreeViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
        
        private async void Node_OnPointerPressed(object sender, PointerPressedEventArgs e) {
            if (sender is IControl { DataContext: IContentNode sourceNode }) {
                var dragData = new GenericDataObject(sourceNode, sourceNode.Name);
                await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
            }
        }
        
        private async void Drop(object sender, DragEventArgs e) {
            if (e.Source is IControl { DataContext: IContentDirectory targetDirectory } &&
                e.Data.Get(string.Empty) is IContentNode sourceNode) {
                await this.ViewModel.MoveNode(sourceNode, targetDirectory);
            }
        }
    }
}