namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.Framework;
    using Microsoft.Xna.Framework;
    using System.Windows;

    public partial class TileGridEditor : NamedValueEditor<TileGrid> {

        public TileGridEditor() {
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