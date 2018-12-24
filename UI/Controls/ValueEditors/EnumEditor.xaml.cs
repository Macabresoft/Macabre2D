namespace Macabre2D.UI.Controls.ValueEditors {

    using System;
    using System.Windows;

    public partial class EnumEditor : NamedValueEditor<object> {

        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register(
            nameof(EnumType),
            typeof(Type),
            typeof(EnumEditor),
            new PropertyMetadata());

        public EnumEditor() {
            this.InitializeComponent();
        }

        public Type EnumType {
            get { return (Type)this.GetValue(EnumTypeProperty); }
            set { this.SetValue(EnumTypeProperty, value); }
        }
    }
}