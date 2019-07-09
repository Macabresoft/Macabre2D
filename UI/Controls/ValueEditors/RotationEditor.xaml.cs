namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.Framework;
    using System.Windows;

    public partial class RotationEditor : NamedValueEditor<Rotation> {

        public RotationEditor() {
            this.InitializeComponent();
        }

        public float Angle {
            get {
                return this.Value?.Angle ?? 0f;
            }

            set {
                this.UpdateProperty(nameof(Rotation.Angle), this.Angle, value);
            }
        }

        protected override void OnValueChanged(Rotation newValue, Rotation oldValue, DependencyObject d) {
            base.OnValueChanged(newValue, oldValue, d);
            this.RaisePropertyChanged(nameof(this.Angle));

            if (oldValue != null) {
                oldValue.AngleChanged -= this.Rotation_AngleChanged;
            }

            if (newValue != null) {
                newValue.AngleChanged += this.Rotation_AngleChanged;
            }
        }

        private void Rotation_AngleChanged(object sender, System.EventArgs e) {
            this.RaisePropertyChanged(nameof(this.Angle));
        }
    }
}