namespace Macabre2D.UI.Controls.ValueEditors {

    using System;
    using System.Windows;

    public partial class EnumEditor : NamedValueEditor<object> {

        public static readonly DependencyProperty EnumTypeProperty = DependencyProperty.Register(
            nameof(EnumEditor.EnumType),
            typeof(Type),
            typeof(EnumEditor),
            new PropertyMetadata());

        public EnumEditor() : base() {
            this.InitializeComponent();
        }

        public Type EnumType {
            get { return (Type)this.GetValue(EnumTypeProperty); }
            set { this.SetValue(EnumTypeProperty, value); }
        }
    }
}