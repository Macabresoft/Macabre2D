namespace Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs {
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs;
    using Unity;

    public class SpriteAnimationEditorDialog : BaseDialog {
        public SpriteAnimationEditorDialog() {
        }

        [InjectionConstructor]
        public SpriteAnimationEditorDialog(SpriteAnimationEditorViewModel viewModel) {
            this.DataContext = viewModel;
            viewModel.CloseRequested += this.OnCloseRequested;
            this.InitializeComponent();
        }

        public SpriteAnimationEditorViewModel ViewModel => this.DataContext as SpriteAnimationEditorViewModel;

        private void Frames_OnValueChanged(object sender, NumericUpDownValueChangedEventArgs e) {
            if (this.ViewModel is SpriteAnimationEditorViewModel viewModel) {
                var oldValue = (int)e.OldValue;
                var newValue = (int)e.NewValue;
                viewModel.CommitFrames(oldValue, newValue);
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCloseRequested(object sender, bool e) {
            this.Close(e);
        }

        private void SpriteIndex_OnValueChanged(object sender, NumericUpDownValueChangedEventArgs e) {
            if (this.ViewModel is SpriteAnimationEditorViewModel viewModel) {
                var oldValue = (byte)e.OldValue;
                var newValue = (byte)e.NewValue;
                viewModel.CommitSpriteIndex(oldValue, newValue);
            }
        }
    }
}