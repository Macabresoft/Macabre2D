namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework.Input;

public partial class InputActionControl : UserControl, IObserver<AvaloniaPropertyChangedEventArgs<InputAction>>, IObserver<AvaloniaPropertyChangedEventArgs<InputBindings>> {
    public static readonly StyledProperty<InputAction> ActionProperty =
        AvaloniaProperty.Register<InputActionControl, InputAction>(
            nameof(Action),
            defaultBindingMode: BindingMode.OneTime);

    public static readonly StyledProperty<InputBindings> InputBindingsProperty =
        AvaloniaProperty.Register<InputActionControl, InputBindings>(
            nameof(InputBindings),
            defaultBindingMode: BindingMode.OneTime);


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

    private readonly IUndoService _undoService;
    private Buttons _selectedGamePadButtons;
    private Keys _selectedKey;
    private MouseButton _selectedMouseButton;

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
        InputBindingsProperty.Changed.Subscribe(this);
        this.InitializeComponent();
    }

    public static IReadOnlyCollection<Buttons> AvailableGamePadButtons { get; }

    public static IReadOnlyCollection<Keys> AvailableKeys { get; }

    public static IReadOnlyCollection<MouseButton> AvailableMouseButtons { get; }

    public InputAction Action {
        get => this.GetValue(ActionProperty);
        set => this.SetValue(ActionProperty, value);
    }

    public InputBindings InputBindings {
        get => this.GetValue(InputBindingsProperty);
        set => this.SetValue(InputBindingsProperty, value);
    }

    public Buttons SelectedGamePadButtons {
        get => this._selectedGamePadButtons;
        set {
            if (value != this._selectedGamePadButtons && this.InputBindings is { } inputBindings) {
                inputBindings.TryGetBindings(this.Action, out var originalValue, out _, out _);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(SelectedGamePadButtonsProperty, ref this._selectedGamePadButtons, value);
                        inputBindings.SetGamePadBinding(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(SelectedGamePadButtonsProperty, ref this._selectedGamePadButtons, originalValue);
                        inputBindings.SetGamePadBinding(this.Action, originalValue);
                    });
            }
        }
    }

    public Keys SelectedKey {
        get => this._selectedKey;
        set {
            if (value != this._selectedKey && this.InputBindings is { } inputBindings) {
                inputBindings.TryGetBindings(this.Action, out _, out var originalValue, out _);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(SelectedKeyProperty, ref this._selectedKey, value);
                        inputBindings.SetKeyBinding(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(SelectedKeyProperty, ref this._selectedKey, originalValue);
                        inputBindings.SetKeyBinding(this.Action, originalValue);
                    });
            }
        }
    }

    public MouseButton SelectedMouseButton {
        get => this._selectedMouseButton;
        set {
            if (value != this._selectedMouseButton && this.InputBindings is { } inputBindings) {
                inputBindings.TryGetBindings(this.Action, out _, out _, out var originalValue);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(SelectedMouseButtonProperty, ref this._selectedMouseButton, value);
                        inputBindings.SetMouseBinding(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(SelectedMouseButtonProperty, ref this._selectedMouseButton, originalValue);
                        inputBindings.SetMouseBinding(this.Action, originalValue);
                    });
            }
        }
    }

    public void OnCompleted() {
    }

    public void OnError(Exception error) {
    }

    public void OnNext(AvaloniaPropertyChangedEventArgs<InputBindings> value) {
        this.Reset();
    }

    public void OnNext(AvaloniaPropertyChangedEventArgs<InputAction> value) {
        this.Reset();
    }

    private void RaisePropertyChanged<T>(DirectPropertyBase<T> property, T value) {
        this.RaisePropertyChanged(property, default, value);
    }

    private void Reset() {
        if (this.InputBindings is { } inputBindings && this.Action != InputAction.None) {
            inputBindings.TryGetBindings(this.Action, out this._selectedGamePadButtons, out this._selectedKey, out this._selectedMouseButton);

            this.RaisePropertyChanged(SelectedGamePadButtonsProperty, this._selectedGamePadButtons);
            this.RaisePropertyChanged(SelectedKeyProperty, this._selectedKey);
            this.RaisePropertyChanged(SelectedMouseButtonProperty, this._selectedMouseButton);
        }
    }
}