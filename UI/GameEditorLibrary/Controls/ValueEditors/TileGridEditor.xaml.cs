namespace Macabre2D.UI.GameEditorLibrary.Controls.ValueEditors {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Controls.ValueEditors;
    using Microsoft.Xna.Framework;
    using System.Windows;

    public partial class TileGridEditor : NamedValueEditor<TileGrid>, ISeparatedValueEditor {

        public static readonly DependencyProperty ShowBottomSeparatorProperty = DependencyProperty.Register(
            nameof(ShowBottomSeparator),
            typeof(bool),
            typeof(TileGridEditor),
            new PropertyMetadata(true));

        public static readonly DependencyProperty ShowTopSeparatorProperty = DependencyProperty.Register(
            nameof(ShowTopSeparator),
            typeof(bool),
            typeof(TileGridEditor),
            new PropertyMetadata(true));

        public TileGridEditor() : base() {
            this.InitializeComponent();
        }

        public Vector2 Offset {
            get {
                return this.Value.Offset;
            }

            set {
                if (value != this.TileSize) {
                    this.Value = new TileGrid(this.TileSize, value);
                }
            }
        }

        public bool ShowBottomSeparator {
            get { return (bool)this.GetValue(ShowBottomSeparatorProperty); }
            set { this.SetValue(ShowBottomSeparatorProperty, value); }
        }

        public bool ShowTopSeparator {
            get { return (bool)this.GetValue(ShowTopSeparatorProperty); }
            set { this.SetValue(ShowTopSeparatorProperty, value); }
        }

        public Vector2 TileSize {
            get {
                return this.Value.TileSize;
            }

            set {
                if (value != this.TileSize) {
                    this.Value = new TileGrid(value, this.Offset);
                }
            }
        }

        protected override void OnValueChanged(TileGrid newValue, TileGrid oldValue, DependencyObject d) {
            if (d is TileGridEditor editor) {
                editor.RaisePropertyChanged();
            }

            base.OnValueChanged(newValue, oldValue, d);
        }

        private void RaisePropertyChanged() {
            this.RaisePropertyChanged(nameof(this.Offset));
            this.RaisePropertyChanged(nameof(this.TileSize));
        }
    }
}