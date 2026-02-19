namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Macabre2D.Common;
using Macabresoft.AvaloniaEx;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using Microsoft.Xna.Framework.Input;

public partial class InputActionControl : UserControl, IObserver<AvaloniaPropertyChangedEventArgs<InputAction>>, IObserver<AvaloniaPropertyChangedEventArgs<InputSettings>> {
    public static readonly StyledProperty<InputAction> ActionProperty =
        AvaloniaProperty.Register<InputActionControl, InputAction>(
            nameof(Action),
            defaultBindingMode: BindingMode.OneWay);

    public static readonly StyledProperty<InputSettings> InputSettingsProperty =
        AvaloniaProperty.Register<InputActionControl, InputSettings>(
            nameof(InputSettings),
            defaultBindingMode: BindingMode.OneTime);

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

    public static readonly DirectProperty<InputActionControl, Buttons> SelectedPrimaryButtonProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, Buttons>(
            nameof(SelectedPrimaryButton),
            editor => editor.SelectedPrimaryButton,
            (editor, value) => editor.SelectedPrimaryButton = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<InputActionControl, Buttons> SelectedSecondaryButtonProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, Buttons>(
            nameof(SelectedSecondaryButton),
            editor => editor.SelectedSecondaryButton,
            (editor, value) => editor.SelectedSecondaryButton = value,
            defaultBindingMode: BindingMode.TwoWay);

    private readonly IUndoService _undoService;
    private bool _isSetting;
    private Keys _selectedKey;
    private MouseButton _selectedMouseButton;
    private Buttons _selectedPrimaryButton;
    private Buttons _selectedSecondaryButton;

    static InputActionControl() {
        var gamePadButtons = Enum.GetValues<Buttons>().ToList();
        gamePadButtons.Remove(Buttons.None);
        AvailableGamePadButtons = gamePadButtons;
        AvailableKeys = Enum.GetValues<Keys>().ToList();
        AvailableMouseButtons = Enum.GetValues<MouseButton>().ToList();
    }

    public InputActionControl() : this(Resolver.Resolve<IUndoService>()) {
    }

    public InputActionControl(IUndoService undoService) {
        this._undoService = undoService;
        ActionProperty.Changed.Subscribe(this);
        InputSettingsProperty.Changed.Subscribe(this);
        this.InitializeComponent();
    }

    public InputAction Action {
        get => this.GetValue(ActionProperty);
        set => this.SetValue(ActionProperty, value);
    }

    public static IReadOnlyCollection<Buttons> AvailableGamePadButtons { get; }

    public static IReadOnlyCollection<Keys> AvailableKeys { get; }

    public static IReadOnlyCollection<MouseButton> AvailableMouseButtons { get; }

    public InputSettings InputSettings {
        get => this.GetValue(InputSettingsProperty);
        set => this.SetValue(InputSettingsProperty, value);
    }

    public Keys SelectedKey {
        get => this._selectedKey;
        set {
            if (value != this._selectedKey && this.InputSettings is { } inputBindings) {
                var action = this.Action;
                inputBindings.TryGetBindings(this.Action, out _, out _, out var originalValue, out _);
                this._undoService.Do(() =>
                    {
                        try {
                            this._isSetting = true;
                            this.SetAndRaise(SelectedKeyProperty, ref this._selectedKey, value);
                            inputBindings.SetKeyBinding(action, value);
                        }
                        finally {
                            this._isSetting = false;
                        }
                    },
                    () =>
                    {
                        try {
                            this._isSetting = true;
                            this.SetAndRaise(SelectedKeyProperty, ref this._selectedKey, originalValue);
                            inputBindings.SetKeyBinding(action, originalValue);
                        }
                        finally {
                            this._isSetting = false;
                        }
                    });
            }
        }
    }

    public MouseButton SelectedMouseButton {
        get => this._selectedMouseButton;
        set {
            if (value != this._selectedMouseButton && this.InputSettings is { } inputBindings) {
                var action = this.Action;
                inputBindings.TryGetBindings(this.Action, out _, out _, out _, out var originalValue);
                this._undoService.Do(() =>
                    {
                        try {
                            this._isSetting = true;
                            this.SetAndRaise(SelectedMouseButtonProperty, ref this._selectedMouseButton, value);
                            inputBindings.SetMouseBinding(action, value);
                        }
                        finally {
                            this._isSetting = false;
                        }
                    },
                    () =>
                    {
                        try {
                            this._isSetting = true;
                            this.SetAndRaise(SelectedMouseButtonProperty, ref this._selectedMouseButton, originalValue);
                            inputBindings.SetMouseBinding(action, originalValue);
                        }
                        finally {
                            this._isSetting = false;
                        }
                    });
            }
        }
    }

    public Buttons SelectedPrimaryButton {
        get => this._selectedPrimaryButton;
        set {
            if (value != this._selectedPrimaryButton && this.InputSettings is { } inputBindings) {
                var action = this.Action;
                inputBindings.TryGetBindings(this.Action, out var originalValue, out _, out _, out _);
                this._undoService.Do(() =>
                    {
                        try {
                            this._isSetting = true;
                            this.SetAndRaise(SelectedPrimaryButtonProperty, ref this._selectedPrimaryButton, value);
                            inputBindings.SetPrimaryGamePadBinding(action, value);
                        }
                        finally {
                            this._isSetting = false;
                        }
                    },
                    () =>
                    {
                        try {
                            this._isSetting = true;
                            this.SetAndRaise(SelectedPrimaryButtonProperty, ref this._selectedPrimaryButton, originalValue);
                            inputBindings.SetPrimaryGamePadBinding(action, originalValue);
                        }
                        finally {
                            this._isSetting = false;
                        }
                    });
            }
        }
    }

    public Buttons SelectedSecondaryButton {
        get => this._selectedSecondaryButton;
        set {
            if (value != this._selectedSecondaryButton && this.InputSettings is { } inputBindings) {
                var action = this.Action;
                inputBindings.TryGetBindings(this.Action, out _, out var originalValue, out _, out _);
                this._undoService.Do(() =>
                    {
                        try {
                            this._isSetting = true;
                            this.SetAndRaise(SelectedSecondaryButtonProperty, ref this._selectedSecondaryButton, value);
                            inputBindings.SetSecondaryGamePadBinding(action, value);
                        }
                        finally {
                            this._isSetting = false;
                        }
                    },
                    () =>
                    {
                        try {
                            this._isSetting = true;
                            this.SetAndRaise(SelectedSecondaryButtonProperty, ref this._selectedSecondaryButton, originalValue);
                            inputBindings.SetSecondaryGamePadBinding(action, originalValue);
                        }
                        finally {
                            this._isSetting = false;
                        }
                    });
            }
        }
    }

    public void OnCompleted() {
    }

    public void OnError(Exception error) {
    }

    public void OnNext(AvaloniaPropertyChangedEventArgs<InputSettings> value) {
        this.Reset();

        if (value.OldValue.Value != null) {
            value.OldValue.Value.BindingChanged -= this.OnBindingChanged;
        }

        if (value.NewValue.Value != null) {
            value.NewValue.Value.BindingChanged += this.OnBindingChanged;
        }
    }

    public void OnNext(AvaloniaPropertyChangedEventArgs<InputAction> value) {
        this.Reset();
    }

    protected override void OnUnloaded(RoutedEventArgs e) {
        base.OnUnloaded(e);

        if (this.InputSettings != null) {
            this.InputSettings.BindingChanged -= this.OnBindingChanged;
        }
    }

    private void OnBindingChanged(object sender, InputAction e) {
        if (this.Action == e) {
            this.Reset();
        }
    }

    private void RaisePropertyChanged<T>(DirectPropertyBase<T> property, T value) {
        this.RaisePropertyChanged(property, default, value);
    }

    private void Reset() {
        if (!this._isSetting && this.InputSettings is { } inputBindings && this.Action != InputAction.None) {
            inputBindings.TryGetBindings(this.Action, out this._selectedPrimaryButton, out this._selectedSecondaryButton, out this._selectedKey, out this._selectedMouseButton);

            this.RaisePropertyChanged(SelectedPrimaryButtonProperty, this._selectedPrimaryButton);
            this.RaisePropertyChanged(SelectedSecondaryButtonProperty, this._selectedPrimaryButton);
            this.RaisePropertyChanged(SelectedKeyProperty, this._selectedKey);
            this.RaisePropertyChanged(SelectedMouseButtonProperty, this._selectedMouseButton);
        }
    }
}