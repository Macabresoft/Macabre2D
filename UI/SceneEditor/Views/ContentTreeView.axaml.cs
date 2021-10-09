namespace Macabresoft.Macabre2D.UI.SceneEditor {
    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;

    public class ContentTreeView : UserControl {
        public static readonly DirectProperty<ContentTreeView, ContentTreeBaseViewModel> ViewModelProperty =
            AvaloniaProperty.RegisterDirect<ContentTreeView, ContentTreeBaseViewModel>(
                nameof(BaseViewModel),
                editor => editor.BaseViewModel);

        private Guid _dragTarget;

        public ContentTreeView() {
            this.DataContext = Resolver.Resolve<ContentTreeBaseViewModel>();
            this.InitializeComponent();
            this.AddHandler(DragDrop.DropEvent, this.Drop);
        }

        public ContentTreeBaseViewModel BaseViewModel => this.DataContext as ContentTreeBaseViewModel;

        private async void Drop(object sender, DragEventArgs e) {
            if (e.Source is IControl { DataContext: IContentDirectory targetDirectory } &&
                e.Data.Get(string.Empty) is IContentNode sourceNode) {
                await this.BaseViewModel.MoveNode(sourceNode, targetDirectory);
            }

            this._dragTarget = Guid.Empty;
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private async void Node_OnPointerMoved(object sender, PointerEventArgs e) {
            if (this._dragTarget != Guid.Empty && sender is IControl { DataContext: IContentNode sourceNode } && sourceNode.Id == this._dragTarget) {
                this._dragTarget = sourceNode.Id;
                var dragData = new GenericDataObject(sourceNode, sourceNode.Name);
                await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
            }
        }

        private void Node_OnPointerPressed(object sender, PointerPressedEventArgs e) {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed && sender is IControl { DataContext: IContentNode sourceNode }) {
                this._dragTarget = sourceNode.Id;
            }
        }

        private void Node_OnPointerReleased(object sender, PointerReleasedEventArgs e) {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased) {
                this._dragTarget = Guid.Empty;
            }
        }
    }
}