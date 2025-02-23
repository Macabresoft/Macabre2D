namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Unity;

public partial class InputBindingsEditor : ValueEditorControl<InputBindings> {
    public static readonly DirectProperty<InputBindingsEditor, GamePadDisplay> DesiredGamePadProperty =
        AvaloniaProperty.RegisterDirect<InputBindingsEditor, GamePadDisplay>(
            nameof(DesiredGamePad),
            editor => editor.DesiredGamePad,
            (editor, value) => editor.DesiredGamePad = value);

    public static readonly DirectProperty<InputBindingsEditor, InputDevice> DesiredInputDeviceProperty =
        AvaloniaProperty.RegisterDirect<InputBindingsEditor, InputDevice>(
            nameof(DesiredInputDevice),
            editor => editor.DesiredInputDevice,
            (editor, value) => editor.DesiredInputDevice = value);

    public static readonly DirectProperty<InputBindingsEditor, bool> IsMouseEnabledProperty =
        AvaloniaProperty.RegisterDirect<InputBindingsEditor, bool>(
            nameof(IsMouseEnabled),
            editor => editor.IsMouseEnabled,
            (editor, value) => editor.IsMouseEnabled = value);

    private readonly IUndoService _undoService;

    public InputBindingsEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public InputBindingsEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;
        var actions = Enum.GetValues<InputAction>().ToList();
        actions.Remove(InputAction.None);
        this.Actions = actions;

        this.InitializeComponent();
    }

    public IReadOnlyCollection<InputAction> Actions { get; }

    public GamePadDisplay DesiredGamePad {
        get => this.Value.DesiredGamePad;
        set {
            if (this.Value.DesiredGamePad != value) {
                var originalValue = this.Value.DesiredGamePad;
                var newValue = value;
                this._undoService.Do(() =>
                {
                    this.Value.DesiredGamePad = newValue;
                    this.RaisePropertyChanged(DesiredGamePadProperty, originalValue, newValue);
                }, () =>
                {
                    this.Value.DesiredGamePad = originalValue;
                    this.RaisePropertyChanged(DesiredGamePadProperty, newValue, originalValue);
                });
            }
        }
    }

    public InputDevice DesiredInputDevice {
        get => this.Value.DesiredInputDevice;
        set {
            if (this.Value.DesiredInputDevice != value) {
                var originalValue = this.Value.DesiredInputDevice;
                var newValue = value;
                this._undoService.Do(() =>
                {
                    this.Value.DesiredInputDevice = newValue;
                    this.RaisePropertyChanged(DesiredInputDeviceProperty, originalValue, newValue);
                }, () =>
                {
                    this.Value.DesiredInputDevice = originalValue;
                    this.RaisePropertyChanged(DesiredInputDeviceProperty, newValue, originalValue);
                });
            }
        }
    }

    public bool IsMouseEnabled {
        get => this.Value.IsMouseEnabled;
        set {
            if (this.Value.IsMouseEnabled != value) {
                var originalValue = this.Value.IsMouseEnabled;
                var newValue = value;
                this._undoService.Do(() =>
                {
                    this.Value.IsMouseEnabled = newValue;
                    this.RaisePropertyChanged(IsMouseEnabledProperty, originalValue, newValue);
                }, () =>
                {
                    this.Value.IsMouseEnabled = originalValue;
                    this.RaisePropertyChanged(IsMouseEnabledProperty, newValue, originalValue);
                });
            }
        }
    }
}