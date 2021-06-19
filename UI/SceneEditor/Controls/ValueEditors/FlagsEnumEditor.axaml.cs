namespace Macabresoft.Macabre2D.UI.SceneEditor.Controls.ValueEditors {
    using System;
    using System.Reactive;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;
    using ReactiveUI;

    public class FlagsEnumEditor : ValueEditorControl<object> {
        public static readonly StyledProperty<Type> EnumTypeProperty =
            AvaloniaProperty.Register<FlagsEnumEditor, Type>(nameof(EnumType));

        public static readonly DirectProperty<FlagsEnumEditor, ICommand> ToggleValueCommandProperty =
            AvaloniaProperty.RegisterDirect<FlagsEnumEditor, ICommand>(
                nameof(EnumType),
                editor => editor.ToggleValueCommand);

        private readonly ReactiveCommand<object, Unit> _toggleValueCommand;

        public FlagsEnumEditor() {
            this._toggleValueCommand = ReactiveCommand.Create<object, Unit>(this.ToggleValue);
            this.InitializeComponent();
        }

        public ICommand ToggleValueCommand => this._toggleValueCommand;

        public Type EnumType {
            get => this.GetValue(EnumTypeProperty);
            set => this.SetValue(EnumTypeProperty, value);
        }

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            this.EnumType = valueType;
            this.RaisePropertyChanged(EnumTypeProperty, Optional<Type>.Empty, this.EnumType);
            base.Initialize(value, valueType, valuePropertyName, title, owner);
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private Unit ToggleValue(object value) {
            var original = Convert.ToInt32(this.Value);
            var toggled = Convert.ToInt32(value);

            if ((original & toggled) == toggled) {
                this.Value = Enum.Parse(this.EnumType, (original & ~toggled).ToString());
            }
            else {
                this.Value = Enum.Parse(this.EnumType, (original | toggled).ToString());
            }

            return Unit.Default;
        }

        private void SelectingItemsControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (sender is ComboBox comboBox) {
                comboBox.SelectedItem = null;
            }
        }
    }
}