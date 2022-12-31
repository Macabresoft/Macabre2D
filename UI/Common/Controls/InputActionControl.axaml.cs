namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;

public class InputActionControl : UserControl {
    public static readonly DirectProperty<InputActionControl, string> ActionNameProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, string>(
            nameof(ActionName),
            editor => editor.ActionName,
            (editor, value) => editor.ActionName = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<InputActionControl, Buttons> SelectedGamePadButtonsProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, Buttons>(
            nameof(SelectedGamePadButtons),
            editor => editor.SelectedGamePadButtons,
            (editor, value) => editor.SelectedGamePadButtons = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<InputActionControl, Keys> SelectedKeyProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, Keys>(
            nameof(SelectedKey),
            editor => editor.SelectedKey,
            (editor, value) => editor.SelectedKey = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<InputActionControl, MouseButton> SelectedMouseButtonProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, MouseButton>(
            nameof(SelectedMouseButton),
            editor => editor.SelectedMouseButton,
            (editor, value) => editor.SelectedMouseButton = value,
            defaultBindingMode: BindingMode.TwoWay);

    private readonly InputSettings _inputSettings;
    private readonly IUndoService _undoService;
    private string _actionName;
    private Buttons _selectedGamePadButtons;
    private Keys _selectedKey;
    private MouseButton _selectedMouseButton;

    public InputActionControl() : this(Resolver.Resolve<IUndoService>(), null, InputAction.Action01, Array.Empty<Buttons>(), Array.Empty<Keys>(), Array.Empty<MouseButton>()) {
    }

    public InputActionControl(
        IUndoService undoService,
        InputSettings inputSettings,
        InputAction action,
        IReadOnlyCollection<Buttons> availableGamePadButtons,
        IReadOnlyCollection<Keys> availableKeys,
        IReadOnlyCollection<MouseButton> availableMouseButtons) {
        this._undoService = undoService;
        this._inputSettings = inputSettings;
        this.Action = action;
        this.AvailableGamePadButtons = availableGamePadButtons;
        this.AvailableKeys = availableKeys;
        this.AvailableMouseButtons = availableMouseButtons;

        this._actionName = this._inputSettings.GetName(this.Action);
        this._inputSettings.DefaultBindings.TryGetBindings(this.Action, out this._selectedGamePadButtons, out this._selectedKey, out this._selectedMouseButton);

        this.InitializeComponent();
    }

    public InputAction Action { get; }

    public string ActionName {
        get => this._actionName;
        set {
            if (value != this._actionName) {
                var originalValue = this._inputSettings.GetName(this.Action);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(ActionNameProperty, ref this._actionName, value);
                        this._inputSettings.SetName(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(ActionNameProperty, ref this._actionName, originalValue);
                        this._inputSettings.SetName(this.Action, originalValue);
                    });
            }
        }
    }

    public IReadOnlyCollection<Buttons> AvailableGamePadButtons { get; }

    public IReadOnlyCollection<Keys> AvailableKeys { get; }

    public IReadOnlyCollection<MouseButton> AvailableMouseButtons { get; }

    public Buttons SelectedGamePadButtons {
        get => this._selectedGamePadButtons;
        set {
            if (value != this._selectedGamePadButtons) {
                this._inputSettings.DefaultBindings.TryGetBindings(this.Action, out var originalValue, out _, out _);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(SelectedGamePadButtonsProperty, ref this._selectedGamePadButtons, value);
                        this._inputSettings.DefaultBindings.SetGamePadBinding(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(SelectedGamePadButtonsProperty, ref this._selectedGamePadButtons, originalValue);
                        this._inputSettings.DefaultBindings.SetGamePadBinding(this.Action, originalValue);
                    });
            }
        }
    }

    public Keys SelectedKey {
        get => this._selectedKey;
        set {
            if (value != this._selectedKey) {
                this._inputSettings.DefaultBindings.TryGetBindings(this.Action, out _, out var originalValue, out _);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(SelectedKeyProperty, ref this._selectedKey, value);
                        this._inputSettings.DefaultBindings.SetKeyBinding(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(SelectedKeyProperty, ref this._selectedKey, originalValue);
                        this._inputSettings.DefaultBindings.SetKeyBinding(this.Action, originalValue);
                    });
            }
        }
    }

    public MouseButton SelectedMouseButton {
        get => this._selectedMouseButton;
        set {
            if (value != this._selectedMouseButton) {
                this._inputSettings.DefaultBindings.TryGetBindings(this.Action, out _, out _, out var originalValue);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(SelectedMouseButtonProperty, ref this._selectedMouseButton, value);
                        this._inputSettings.DefaultBindings.SetMouseBinding(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(SelectedMouseButtonProperty, ref this._selectedMouseButton, originalValue);
                        this._inputSettings.DefaultBindings.SetMouseBinding(this.Action, originalValue);
                    });
            }
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}