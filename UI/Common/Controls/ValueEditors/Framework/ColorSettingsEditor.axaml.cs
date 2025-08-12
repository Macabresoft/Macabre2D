namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Unity;

public partial class ColorSettingsEditor : ValueEditorControl<RenderSettings> {

    public static readonly DirectProperty<ColorSettingsEditor, Color> CurrentColorProperty =
        AvaloniaProperty.RegisterDirect<ColorSettingsEditor, Color>(
            nameof(CurrentColor),
            editor => editor.CurrentColor,
            (editor, value) => editor.CurrentColor = value);

    public static readonly DirectProperty<ColorSettingsEditor, bool> IsOverrideEnabledProperty =
        AvaloniaProperty.RegisterDirect<ColorSettingsEditor, bool>(
            nameof(IsOverrideEnabled),
            editor => editor.IsOverrideEnabled,
            (editor, value) => editor.IsOverrideEnabled = value);

    public static readonly DirectProperty<ColorSettingsEditor, RenderPriority> SelectedPriorityProperty =
        AvaloniaProperty.RegisterDirect<ColorSettingsEditor, RenderPriority>(
            nameof(SelectedPriority),
            editor => editor.SelectedPriority,
            (editor, value) => editor.SelectedPriority = value);

    private readonly IUndoService _undoService;
    private Color _currentColor;
    private bool _isOverrideEnabled;
    private RenderPriority _selectedPriority;

    public ColorSettingsEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public ColorSettingsEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;
        var priorities = Enum.GetValues<RenderPriority>().ToList();
        this.Priorities = priorities;

        this.InitializeComponent();
    }

    public Color CurrentColor {
        get => this._currentColor;
        set {
            if (this._currentColor != value) {
                var priority = this.SelectedPriority;
                var originalValue = this._currentColor;
                var newValue = value;
                this._undoService.Do(() =>
                {
                    this._currentColor = newValue;
                    this.Value.SetRenderPriorityColor(priority, this._currentColor);
                    this.RaisePropertyChanged(CurrentColorProperty, originalValue, newValue);
                }, () =>
                {
                    this._currentColor = originalValue;
                    this.Value.SetRenderPriorityColor(priority, this._currentColor);
                    this.RaisePropertyChanged(CurrentColorProperty, newValue, originalValue);
                });
            }
        }
    }

    public bool IsOverrideEnabled {
        get => this._isOverrideEnabled;
        set {
            if (this._isOverrideEnabled != value) {
                var priority = this.SelectedPriority;
                var originalValue = this._isOverrideEnabled;
                var newValue = value;
                this._undoService.Do(() =>
                {
                    this._isOverrideEnabled = newValue;
                    if (this._isOverrideEnabled) {
                        this.Value.EnableRenderPriorityColor(priority);
                    }
                    else {
                        this.Value.RemoveRenderPriorityColor(priority);
                    }

                    this.RaisePropertyChanged(IsOverrideEnabledProperty, originalValue, newValue);
                    this.ResetColor();
                }, () =>
                {
                    this._isOverrideEnabled = originalValue;
                    if (this._isOverrideEnabled) {
                        this.Value.EnableRenderPriorityColor(priority);
                    }
                    else {
                        this.Value.RemoveRenderPriorityColor(priority);
                    }

                    this.RaisePropertyChanged(IsOverrideEnabledProperty, newValue, originalValue);
                    this.ResetColor();
                });
            }
        }
    }

    public IReadOnlyCollection<RenderPriority> Priorities { get; }

    public RenderPriority SelectedPriority {
        get => this._selectedPriority;
        set {
            if (this._selectedPriority != value) {
                var originalValue = this._selectedPriority;
                this._selectedPriority = value;
                this.RaisePropertyChanged(SelectedPriorityProperty, this._selectedPriority, originalValue);
                this.Reset();
            }

        }
    }
    
    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<RenderSettings> args) {
        base.OnValueChanged(args);
        this.Reset();
    }

    private void ResetColor() {
        var originalColor = this._currentColor;
        
        if (this._isOverrideEnabled && this.Value.TryGetColorForRenderPriority(this.SelectedPriority, out var color)) {
            this._currentColor = color;
        }
        else {
            this._currentColor = Color.Transparent;
            this._isOverrideEnabled = false;
            this.RaisePropertyChanged(IsOverrideEnabledProperty, true, this._isOverrideEnabled);
        }
        
        this.RaisePropertyChanged(CurrentColorProperty, originalColor, this._currentColor);
    }

    private void Reset() {
        var originalColor = this._currentColor;
        var originalIsOverrideEnabled = this._isOverrideEnabled;

        if (this.Value.TryGetColorForRenderPriority(this.SelectedPriority, out var color)) {
            this._currentColor = color;
            this._isOverrideEnabled = true;
        }
        else {
            this._currentColor = Color.Transparent;
            this._isOverrideEnabled = false;
        }

        this.RaisePropertyChanged(CurrentColorProperty, originalColor, this._currentColor);
        this.RaisePropertyChanged(IsOverrideEnabledProperty, originalIsOverrideEnabled, this._isOverrideEnabled);
    }
}