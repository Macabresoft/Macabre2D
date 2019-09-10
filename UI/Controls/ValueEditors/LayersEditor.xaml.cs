namespace Macabre2D.UI.Controls.ValueEditors {

    using Macabre2D.Framework;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public partial class LayersEditor : NamedValueEditor<Layers> {

        public LayersEditor() {
            this.InitializeComponent();
        }

        protected override void OnValueChanged(Layers newValue, Layers oldValue, DependencyObject d) {
            foreach (var item in this._layersItemsControl.Items.OfType<CheckBox>()) {
                // Since we have static bindings we need to update these incase GameSettings has changed.
                item.GetBindingExpression(CheckBox.ContentProperty).UpdateTarget();
            }

            base.OnValueChanged(newValue, oldValue, d);
        }
    }
}