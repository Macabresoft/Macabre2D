namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Input;

public partial class InputActionControl : UserControl, IObserver<AvaloniaPropertyChangedEventArgs<InputAction>>, IObserver<AvaloniaPropertyChangedEventArgs<InputSettings>> {
    public static readonly DirectProperty<InputActionControl, string> ActionNameProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, string>(
            nameof(ActionName),
            editor => editor.ActionName,
            (editor, value) => editor.ActionName = value,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<InputAction> ActionProperty =
        AvaloniaProperty.Register<InputActionControl, InputAction>(
            nameof(Action),
            defaultBindingMode: BindingMode.OneTime);

    public static readonly StyledProperty<InputSettings> InputSettingsProperty =
        AvaloniaProperty.Register<InputActionControl, InputSettings>(
            nameof(InputSettings),
            defaultBindingMode: BindingMode.OneTime);

    public static readonly DirectProperty<InputActionControl, bool> IsPredefinedProperty =
        AvaloniaProperty.RegisterDirect<InputActionControl, bool>(
            nameof(IsPredefined),
            editor => editor.IsPredefined,
            (editor, value) => editor.IsPredefined = value,
            defaultBindingMode: BindingMode.OneWay);

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


    private readonly IProjectService _projectService;
    private readonly IUndoService _undoService;
    private string _actionName;
    private bool _isPredefined;
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

    public InputActionControl() : this(Resolver.Resolve<IProjectService>(), Resolver.Resolve<IUndoService>()) {
    }

    public InputActionControl(IProjectService projectService, IUndoService undoService) {
        this._projectService = projectService;
        this._undoService = undoService;
        ActionProperty.Changed.Subscribe(this);
        InputSettingsProperty.Changed.Subscribe(this);
        this.InitializeComponent();
    }

    public static IReadOnlyCollection<Buttons> AvailableGamePadButtons { get; }

    public static IReadOnlyCollection<Keys> AvailableKeys { get; }

    public static IReadOnlyCollection<MouseButton> AvailableMouseButtons { get; }

    public InputAction Action {
        get => this.GetValue(ActionProperty);
        set => this.SetValue(ActionProperty, value);
    }

    public string ActionName {
        get => this._actionName;
        set {
            if (value != this._actionName && this.InputSettings is { } inputSettings) {
                var originalValue = inputSettings.GetName(this.Action);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(ActionNameProperty, ref this._actionName, value);
                        inputSettings.SetName(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(ActionNameProperty, ref this._actionName, originalValue);
                        inputSettings.SetName(this.Action, originalValue);
                    });
            }
        }
    }

    public InputSettings InputSettings {
        get => this.GetValue(InputSettingsProperty);
        set => this.SetValue(InputSettingsProperty, value);
    }

    public bool IsPredefined {
        get => this._isPredefined;
        set => this.SetAndRaise(IsPredefinedProperty, ref this._isPredefined, value);
    }

    public Buttons SelectedGamePadButtons {
        get => this._selectedGamePadButtons;
        set {
            if (value != this._selectedGamePadButtons) {
                var defaultBindings = this._projectService.CurrentProject.DefaultUserSettings.Input;
                defaultBindings.TryGetBindings(this.Action, out var originalValue, out _, out _);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(SelectedGamePadButtonsProperty, ref this._selectedGamePadButtons, value);
                        defaultBindings.SetGamePadBinding(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(SelectedGamePadButtonsProperty, ref this._selectedGamePadButtons, originalValue);
                        defaultBindings.SetGamePadBinding(this.Action, originalValue);
                    });
            }
        }
    }

    public Keys SelectedKey {
        get => this._selectedKey;
        set {
            if (value != this._selectedKey) {
                var defaultBindings = this._projectService.CurrentProject.DefaultUserSettings.Input;
                defaultBindings.TryGetBindings(this.Action, out _, out var originalValue, out _);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(SelectedKeyProperty, ref this._selectedKey, value);
                        defaultBindings.SetKeyBinding(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(SelectedKeyProperty, ref this._selectedKey, originalValue);
                        defaultBindings.SetKeyBinding(this.Action, originalValue);
                    });
            }
        }
    }

    public MouseButton SelectedMouseButton {
        get => this._selectedMouseButton;
        set {
            if (value != this._selectedMouseButton) {
                var defaultBindings = this._projectService.CurrentProject.DefaultUserSettings.Input;
                defaultBindings.TryGetBindings(this.Action, out _, out _, out var originalValue);
                this._undoService.Do(() =>
                    {
                        this.SetAndRaise(SelectedMouseButtonProperty, ref this._selectedMouseButton, value);
                        defaultBindings.SetMouseBinding(this.Action, value);
                    },
                    () =>
                    {
                        this.SetAndRaise(SelectedMouseButtonProperty, ref this._selectedMouseButton, originalValue);
                        defaultBindings.SetMouseBinding(this.Action, originalValue);
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
    }

    public void OnNext(AvaloniaPropertyChangedEventArgs<InputAction> value) {
        this.Reset();
    }

    private void RaisePropertyChanged<T>(DirectPropertyBase<T> property, T value) {
        this.RaisePropertyChanged(property, default, value);
    }

    private void Reset() {
        if (this.InputSettings is { } inputSettings && this.Action != InputAction.None) {
            var defaultBindings = this._projectService.CurrentProject.DefaultUserSettings.Input;
            this._actionName = inputSettings.GetName(this.Action);
            defaultBindings.TryGetBindings(this.Action, out this._selectedGamePadButtons, out this._selectedKey, out this._selectedMouseButton);

            this.RaisePropertyChanged(ActionNameProperty, this._actionName);
            this.RaisePropertyChanged(SelectedGamePadButtonsProperty, this._selectedGamePadButtons);
            this.RaisePropertyChanged(SelectedKeyProperty, this._selectedKey);
            this.RaisePropertyChanged(SelectedMouseButtonProperty, this._selectedMouseButton);

            this.IsPredefined = InputSettings.PredefinedActions.Contains(this.Action);
        }
    }
}