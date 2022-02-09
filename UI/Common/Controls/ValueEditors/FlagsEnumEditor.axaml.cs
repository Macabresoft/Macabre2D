namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Linq;
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

    public static readonly DirectProperty<FlagsEnumEditor, ICommand> SelectAllCommandProperty =
        AvaloniaProperty.RegisterDirect<FlagsEnumEditor, ICommand>(
            nameof(SelectAllCommand),
            editor => editor.SelectAllCommand);

    public static readonly DirectProperty<FlagsEnumEditor, ICommand> ToggleValueCommandProperty =
        AvaloniaProperty.RegisterDirect<FlagsEnumEditor, ICommand>(
            nameof(ToggleValueCommand),
            editor => editor.ToggleValueCommand);

    public FlagsEnumEditor() : this(null) {
    }

    [InjectionConstructor]
    public FlagsEnumEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.EnumType = dependencies?.ValueType;
        this.ClearCommand = ReactiveCommand.Create(this.Clear);
        this.SelectAllCommand = ReactiveCommand.Create(this.SelectAll);
        this.ToggleValueCommand = ReactiveCommand.Create<object>(this.ToggleValue);
        this.InitializeComponent();
    }

    public ICommand ClearCommand { get; }

    public Type EnumType { get; }

    public ICommand SelectAllCommand { get; }

    public ICommand ToggleValueCommand { get; }

    private void Clear() {
        this.Value = Enum.Parse(this.EnumType, 0.ToString());
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void SelectAll() {
        var enums = Enum.GetValues(this.EnumType).Cast<object>().Select(Convert.ToInt32);
        var all = enums.Aggregate(0, (current, value) => current | value);
        var original = Convert.ToInt32(this.Value);
        if (all != original) {
            this.Value = Enum.Parse(this.EnumType, all.ToString());
        }
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