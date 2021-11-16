namespace Macabresoft.Macabre2D.UI.Common {
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.Framework;
    using Unity;

    public class SpriteAnimationEditorView : UserControl {
        public SpriteAnimationEditorView() {
        }

        [InjectionConstructor]
        public SpriteAnimationEditorView(SpriteAnimationEditorViewModel viewModel) {
            this.DataContext = viewModel;
            this.InitializeComponent();
        }

        public SpriteAnimationEditorViewModel ViewModel => this.DataContext as SpriteAnimationEditorViewModel;

        private void Frames_OnValueChanged(object sender, NumericUpDownValueChangedEventArgs e) {
            if (this.ViewModel is SpriteAnimationEditorViewModel viewModel && sender is IDataContextProvider { DataContext: SpriteAnimationStep step }) {
                var oldValue = (int)e.OldValue;
                var newValue = (int)e.NewValue;
                viewModel.CommitFrames(step, oldValue, newValue);
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void SpriteIndex_OnValueChanged(object sender, NumericUpDownValueChangedEventArgs e) {
            // IsActive gets set to true after all the bindings are first set, so this ignores initial settings.
            if (this.ViewModel is SpriteAnimationEditorViewModel viewModel && sender is IDataContextProvider { DataContext: SpriteAnimationStep step }) {
                var oldValue = (byte?)e.OldValue;
                var newValue = (byte?)e.NewValue;
                viewModel.CommitSpriteIndex(step, oldValue, newValue);
            }
        }
    }
}