namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Unity;

public class FlagsEnumEditor : ValueEditorControl<object> {
    public static readonly DirectProperty<FlagsEnumEditor, Type> EnumTypeProperty =
        AvaloniaProperty.RegisterDirect<FlagsEnumEditor, Type>(
            nameof(EnumType),
            editor => editor.EnumType);

    public static readonly DirectProperty<FlagsEnumEditor, ICommand> ToggleValueCommandProperty =
        AvaloniaProperty.RegisterDirect<FlagsEnumEditor, ICommand>(
            nameof(EnumType),
            editor => editor.ToggleValueCommand);

    public FlagsEnumEditor() : this(null) {
    }

    [InjectionConstructor]
    public FlagsEnumEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.EnumType = dependencies?.ValueType;
        this.ToggleValueCommand = ReactiveCommand.Create<object>(this.ToggleValue);
        this.InitializeComponent();
    }

    public Type EnumType { get; }

    public ICommand ToggleValueCommand { get; }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void SelectingItemsControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (sender is ComboBox comboBox) {
            comboBox.SelectedItem = null;
        }
    }

    private void ToggleValue(object value) {
        var original = Convert.ToInt32(this.Value);
        var toggled = Convert.ToInt32(value);

        if ((original & toggled) == toggled) {
            this.Value = Enum.Parse(this.EnumType, (original & ~toggled).ToString());
        }
        else {
            this.Value = Enum.Parse(this.EnumType, (original | toggled).ToString());
        }
    }
}