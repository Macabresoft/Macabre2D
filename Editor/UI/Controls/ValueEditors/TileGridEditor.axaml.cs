namespace Macabresoft.Macabre2D.Editor.UI.Controls.ValueEditors {
    using Avalonia;
    using Avalonia.Data;
    using Avalonia.LogicalTree;
    using Avalonia.Markup.Xaml;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.Framework;
    using Microsoft.Xna.Framework;

    public class TileGridEditor : ValueEditorControl<TileGrid> {
        public static readonly DirectProperty<TileGridEditor, Vector2> TileSizeValueProperty =
            AvaloniaProperty.RegisterDirect<TileGridEditor, Vector2>(
                nameof(TileSizeValue),
                editor => editor.TileSizeValue,
                (editor, value) => editor.TileSizeValue = value);


        private Vector2 _tileSizeValue;

        public TileGridEditor() {
            this.InitializeComponent();
        }

        public Vector2 TileSizeValue {
            get => this._tileSizeValue;
            set => this.SetAndRaise(TileSizeValueProperty, ref this._tileSizeValue, value);
        }

        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
            base.OnAttachedToLogicalTree(e);
            this.UpdateDisplayValues();
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
            base.OnPropertyChanged(change);

            if (change.Property.Name == nameof(this.Value)) {
                this.UpdateDisplayValues();
            }
            else if (change.Property.Name == nameof(this.TileSizeValue)) {
                Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new TileGrid(this.TileSizeValue)));
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void UpdateDisplayValues() {
            this._tileSizeValue = this.Value.TileSize;
            Dispatcher.UIThread.Post(() => {
                this.RaisePropertyChanged(TileSizeValueProperty, Optional<Vector2>.Empty, this.TileSizeValue);
            });
        }
    }
}