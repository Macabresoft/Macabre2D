namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Windows.Input;
using Avalonia;
using Macabresoft.Macabre2D.Framework;
using ReactiveUI;
using Unity;

public partial class FlagsEnumEditor : ValueEditorControl<object> {
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
        if (EnumHelper.TryGetZero(this.EnumType, out var result) && result != this.Value) {
            this.Value = result;
        }
    }

    private void SelectAll() {
        if (EnumHelper.TryGetAll(this.EnumType, out var result) && result != this.Value) {
            this.Value = result;
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