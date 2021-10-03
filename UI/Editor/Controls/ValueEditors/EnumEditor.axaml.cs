namespace Macabresoft.Macabre2D.UI.Editor {
    using System;
    using Avalonia;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;

    public class EnumEditor : ValueEditorControl<object> {
        public static readonly DirectProperty<EnumEditor, Type> EnumTypeProperty =
            AvaloniaProperty.RegisterDirect<EnumEditor, Type>(
                nameof(EnumType),
                editor => editor.EnumType);

        public EnumEditor() {
            this.InitializeComponent();
        }

        public Type EnumType { get; private set; }

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            this.EnumType = valueType;
            this.RaisePropertyChanged(EnumTypeProperty, Optional<Type>.Empty, this.EnumType);
            base.Initialize(value, valueType, valuePropertyName, title, owner);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}