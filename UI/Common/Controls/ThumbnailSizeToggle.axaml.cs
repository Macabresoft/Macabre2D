namespace Macabresoft.Macabre2D.UI.Common {
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;
    using ReactiveUI;

    public class ThumbnailSizeToggle : UserControl {
        public static readonly StyledProperty<ThumbnailSize> SelectedSizeProperty = AvaloniaProperty.Register<ThumbnailSizeToggle, ThumbnailSize>(
            nameof(SelectedSize),
            ThumbnailSize.Small,
            defaultBindingMode: BindingMode.TwoWay);

        public static readonly DirectProperty<ThumbnailSizeToggle, ICommand> SelectSizeCommandProperty =
            AvaloniaProperty.RegisterDirect<ThumbnailSizeToggle, ICommand>(
                nameof(SelectSizeCommand),
                editor => editor.SelectSizeCommand);

        public ThumbnailSizeToggle() {
            this.SelectSizeCommand = ReactiveCommand.Create<ThumbnailSize>(this.SelectSize);
            this.InitializeComponent();
        }

        public ICommand SelectSizeCommand { get; }

        public ThumbnailSize SelectedSize {
            get => this.GetValue(SelectedSizeProperty);
            set => this.SetValue(SelectedSizeProperty, value);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void SelectSize(ThumbnailSize size) {
            this.SelectedSize = size;
        }
    }
}