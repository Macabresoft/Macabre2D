namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;
using Unity;

public class InputSettingsEditor : ValueEditorControl<InputSettings> {
    private readonly IUndoService _undoService;
    private BoolEditor _leftAnalogControl;
    private BoolEditor _mouseControl;
    private BoolEditor _rightAnalogControl;

    public InputSettingsEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public InputSettingsEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;
        this.InitializeComponent();
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
        base.OnAttachedToLogicalTree(e);
        this.ConstructControl();
    }

    private void ConstructControl() {
        if (this.Value == null) {
            return;
        }

        var grid = this.LogicalChildren.OfType<Grid>().First();

        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        this._mouseControl = new BoolEditor(new ValueControlDependencies(
            this.Value.DefaultBindings.IsMouseEnabled,
            typeof(bool),
            nameof(this.Value.DefaultBindings.IsMouseEnabled),
            "Mouse Enabled",
            this.Value.DefaultBindings)) {
            [Grid.RowProperty] = 0
        };

        this._mouseControl.ValueChanged += this.OnIsMouseEnabledChanged;
        grid.Children.Add(this._mouseControl);

        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        this._leftAnalogControl = new BoolEditor(new ValueControlDependencies(
            this.Value.DefaultBindings.IsLeftAnalogEnabled,
            typeof(bool),
            nameof(this.Value.DefaultBindings.IsLeftAnalogEnabled),
            "Left Analog Enabled",
            this.Value.DefaultBindings)) {
            [Grid.RowProperty] = 1
        };

        this._leftAnalogControl.ValueChanged += this.OnIsLeftAnalogEnabledChanged;
        grid.Children.Add(this._leftAnalogControl);

        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        this._rightAnalogControl = new BoolEditor(new ValueControlDependencies(
            this.Value.DefaultBindings.IsRightAnalogEnabled,
            typeof(bool),
            nameof(this.Value.DefaultBindings.IsRightAnalogEnabled),
            "Right Analog Enabled",
            this.Value.DefaultBindings)) {
            [Grid.RowProperty] = 2
        };

        this._rightAnalogControl.ValueChanged += this.OnIsRightAnalogEnabledChanged;
        grid.Children.Add(this._rightAnalogControl);

        var existingRows = grid.RowDefinitions.Count;
        var rowValues = Enum.GetValues<InputAction>().ToList();
        rowValues.Remove(InputAction.None);
        var gamePadButtons = Enum.GetValues<Buttons>().ToList();
        gamePadButtons.Remove(Buttons.None);
        var keys = Enum.GetValues<Keys>().ToList();
        var mouseButtons = Enum.GetValues<MouseButton>().ToList();

        for (var i = 0; i <= rowValues.Count; i++) {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        for (var i = 0; i < rowValues.Count; i++) {
            var row = i * 2 + existingRows;
            var action = rowValues[i];

            var control = new InputActionControl(this._undoService, this.Value, action, gamePadButtons, keys, mouseButtons) {
                [Grid.RowProperty] = row
            };

            grid.Children.Add(control);

            if (i < rowValues.Count - 1) {
                var separator = new Border {
                    [Grid.RowProperty] = row + 1,
                    [Grid.ColumnProperty] = 0,
                    [Grid.ColumnSpanProperty] = 2
                };

                separator.Classes.Add("HorizontalSeparator");
                grid.Children.Add(separator);
            }
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void OnIsLeftAnalogEnabledChanged(object sender, ValueChangedEventArgs<object> e) {
        this._undoService.Do(() =>
        {
            var updatedValue = (bool)e.UpdatedValue;
            if (this.Value.DefaultBindings.IsLeftAnalogEnabled != updatedValue) {
                this.Value.DefaultBindings.IsLeftAnalogEnabled = updatedValue;
                this._leftAnalogControl.Value = updatedValue;
            }
        }, () =>
        {
            var originalValue = (bool)e.OriginalValue;
            if (this.Value.DefaultBindings.IsLeftAnalogEnabled != originalValue) {
                this.Value.DefaultBindings.IsLeftAnalogEnabled = originalValue;
                this._leftAnalogControl.Value = originalValue;
            }
        });
    }

    private void OnIsMouseEnabledChanged(object sender, ValueChangedEventArgs<object> e) {
        this._undoService.Do(() =>
        {
            var updatedValue = (bool)e.UpdatedValue;
            if (this.Value.DefaultBindings.IsMouseEnabled != updatedValue) {
                this.Value.DefaultBindings.IsMouseEnabled = updatedValue;
                this._mouseControl.Value = updatedValue;
            }
        }, () =>
        {
            var originalValue = (bool)e.OriginalValue;
            if (this.Value.DefaultBindings.IsMouseEnabled != originalValue) {
                this.Value.DefaultBindings.IsMouseEnabled = originalValue;
                this._mouseControl.Value = originalValue;
            }
        });
    }

    private void OnIsRightAnalogEnabledChanged(object sender, ValueChangedEventArgs<object> e) {
        this._undoService.Do(() =>
        {
            var updatedValue = (bool)e.UpdatedValue;
            if (this.Value.DefaultBindings.IsRightAnalogEnabled != updatedValue) {
                this.Value.DefaultBindings.IsRightAnalogEnabled = updatedValue;
                this._rightAnalogControl.Value = updatedValue;
            }
        }, () =>
        {
            var originalValue = (bool)e.OriginalValue;
            if (this.Value.DefaultBindings.IsRightAnalogEnabled != originalValue) {
                this.Value.DefaultBindings.IsRightAnalogEnabled = originalValue;
                this._rightAnalogControl.Value = originalValue;
            }
        });
    }
}