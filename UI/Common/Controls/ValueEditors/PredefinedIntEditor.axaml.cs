namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Unity;

public partial class PredefinedIntEditor : BaseNumericEditor<int> {
    public static readonly DirectProperty<PredefinedIntEditor, IReadOnlyCollection<PredefinedInteger>> AvailablePredefinedIntegersProperty =
        AvaloniaProperty.RegisterDirect<PredefinedIntEditor, IReadOnlyCollection<PredefinedInteger>>(
            nameof(AvailablePredefinedIntegers),
            editor => editor.AvailablePredefinedIntegers);

    public static readonly DirectProperty<PredefinedIntEditor, PredefinedInteger> SelectedIntegerProperty =
        AvaloniaProperty.RegisterDirect<PredefinedIntEditor, PredefinedInteger>(
            nameof(SelectedInteger),
            editor => editor.SelectedInteger,
            (editor, value) => editor.SelectedInteger = value);

    private readonly ObservableCollectionExtended<PredefinedInteger> _availablePredefinedIntegers = new();

    private bool _isSettingInteger;
    private PredefinedInteger _selectedInteger;

    public PredefinedIntEditor() : this(null, PredefinedIntegerKind.RenderOrder) {
    }

    [InjectionConstructor]
    public PredefinedIntEditor(ValueControlDependencies dependencies, PredefinedIntegerKind predefinedIntegerKind) : base(dependencies) {
        this._availablePredefinedIntegers.AddRange(PredefinedIntegers.GetCollection(predefinedIntegerKind));
        this.ResetSelected();
        this.InitializeComponent();
    }

    public IReadOnlyCollection<PredefinedInteger> AvailablePredefinedIntegers => this._availablePredefinedIntegers;

    public PredefinedInteger SelectedInteger {
        get => this._selectedInteger;
        set {
            try {
                this._isSettingInteger = true;
                this.SetAndRaise(SelectedIntegerProperty, ref this._selectedInteger, value);
                if (!this._selectedInteger.IsUndefined()) {
                    this.Value = value.Value;
                }
            }
            finally {
                this._isSettingInteger = false;
            }
        }
    }

    protected override int ConvertValue(object calculatedValue) => Convert.ToInt32(calculatedValue);

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<int> args) {
        base.OnValueChanged(args);
        this.ResetSelected();
    }

    private void ResetSelected() {
        if (!this._isSettingInteger) {
            var selectedInteger = PredefinedIntegers.UndefinedValue;
            foreach (var predefinedInteger in this.AvailablePredefinedIntegers) {
                if (predefinedInteger.Value == this.Value) {
                    selectedInteger = predefinedInteger;
                    break;
                }
            }

            this.SelectedInteger = selectedInteger;
        }
    }

    private void SelectingItemsControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        this._integerSelectionButton.Flyout?.Hide();
    }
}