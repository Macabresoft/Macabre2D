namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views {
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Input;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.ViewModels;
    using System;

    public class ContentTreeView : UserControl {
        public static readonly DirectProperty<ContentTreeView, ContentTreeViewModel> ViewModelProperty =
            AvaloniaProperty.RegisterDirect<ContentTreeView, ContentTreeViewModel>(
                nameof(ViewModel),
                editor => editor.ViewModel);

        private Guid _dragTarget;
        
        public ContentTreeView() {
            this.DataContext = Resolver.Resolve<ContentTreeViewModel>();
            this.InitializeComponent();
            this.AddHandler(DragDrop.DropEvent, this.Drop);
        }

        public ContentTreeViewModel ViewModel => this.DataContext as ContentTreeViewModel;

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void Node_OnPointerPressed(object sender, PointerPressedEventArgs e) {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed && sender is IControl { DataContext: IContentNode sourceNode }) {
                this._dragTarget = sourceNode.Id;
            }
        }
        
        private async void Drop(object sender, DragEventArgs e) {
            if (e.Source is IControl { DataContext: IContentDirectory targetDirectory } &&
                e.Data.Get(string.Empty) is IContentNode sourceNode) {
                await this.ViewModel.MoveNode(sourceNode, targetDirectory);
            }

            this._dragTarget = Guid.Empty;
        }

        private void Node_OnPointerReleased(object sender, PointerReleasedEventArgs e) {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased) {
                this._dragTarget = Guid.Empty;
            }
        }

        private async void Node_OnPointerMoved(object sender, PointerEventArgs e) {
            if (this._dragTarget != Guid.Empty && sender is IControl { DataContext: IContentNode sourceNode } && sourceNode.Id == this._dragTarget) {
                this._dragTarget = sourceNode.Id;
                var dragData = new GenericDataObject(sourceNode, sourceNode.Name);
                await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
            }
        }
    }
}