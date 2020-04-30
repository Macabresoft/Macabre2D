namespace Macabre2D.UI.CommonLibrary.Controls.ValueEditors {

    using Microsoft.Xna.Framework;
    using System.Windows;

    public partial class ColorEditor : NamedValueEditor<Color> {
        private System.Windows.Media.Color _selectedColor;

        public ColorEditor() : base() {
            this.InitializeComponent();
        }

        public System.Windows.Media.Color SelectedColor {
            get {
                return this._selectedColor;
            }
            set {
                if (value != this._selectedColor) {
                    this._selectedColor = value;
                    this.Value = new Color(this._selectedColor.R, this._selectedColor.G, this._selectedColor.B, this._selectedColor.A);
                    this.RaisePropertyChanged(nameof(this.SelectedColor));
                }
            }
        }

        protected override void OnValueChanged(Color newValue, Color oldValue, DependencyObject d) {
            if (newValue != oldValue && d is ColorEditor editor) {
                editor.SelectedColor = new System.Windows.Media.Color() {
                    R = newValue.R,
                    G = newValue.G,
                    B = newValue.B,
                    A = newValue.A
                };
            }

            base.OnValueChanged(newValue, oldValue, d);
        }
    }
}