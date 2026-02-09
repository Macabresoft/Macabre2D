namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Unity;

public partial class RenderSettingsEditor : ValueEditorControl<RenderSettings> {

    public static readonly DirectProperty<RenderSettingsEditor, BlendStateType> CurrentBlendStateProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, BlendStateType>(
            nameof(CurrentBlendState),
            editor => editor.CurrentBlendState,
            (editor, value) => editor.CurrentBlendState = value);

    public static readonly DirectProperty<RenderSettingsEditor, Color> CurrentColorProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, Color>(
            nameof(CurrentColor),
            editor => editor.CurrentColor,
            (editor, value) => editor.CurrentColor = value);

    public static readonly DirectProperty<RenderSettingsEditor, bool> IsOverrideEnabledProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, bool>(
            nameof(IsOverrideEnabled),
            editor => editor.IsOverrideEnabled,
            (editor, value) => editor.IsOverrideEnabled = value);

    public static readonly DirectProperty<RenderSettingsEditor, RenderPriority> SelectedPriorityProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, RenderPriority>(
            nameof(SelectedPriority),
            editor => editor.SelectedPriority,
            (editor, value) => editor.SelectedPriority = value);

    private readonly IAssetManager _assetManager;

    private readonly HashSet<IValueControl> _shaderEditors = new();

    private readonly IUndoService _undoService;
    private readonly IValueControlService _valueControlService;
    private ValueControlCollection _controlCollection;
    private BlendStateType _currentBlendState;
    private Color _currentColor;
    private bool _isOverrideEnabled;
    private RenderPriority _selectedPriority;

    public RenderSettingsEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<IUndoService>(),
        Resolver.Resolve<IValueControlService>()) {
    }

    [InjectionConstructor]
    public RenderSettingsEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        IUndoService undoService,
        IValueControlService valueControlService) : base(dependencies) {
        this._assetManager = assetManager;
        this._undoService = undoService;
        this._valueControlService = valueControlService;

        this.Priorities = Enum.GetValues<RenderPriority>().ToList();

        this.ResetShaderEditors();
        this.InitializeComponent();
    }

    public BlendStateType CurrentBlendState {
        get => this._currentBlendState;
        set {
            if (this._currentBlendState != value) {
                var priority = this.SelectedPriority;
                var originalValue = this._currentBlendState;
                var newValue = value;
                this._undoService.Do(() =>
                {
                    this._currentBlendState = newValue;
                    this.Value.SetRenderPriorityBlendState(priority, this._currentBlendState);
                    this.RaisePropertyChanged(CurrentBlendStateProperty, originalValue, newValue);
                }, () =>
                {
                    this._currentBlendState = originalValue;
                    this.Value.SetRenderPriorityBlendState(priority, this._currentBlendState);
                    this.RaisePropertyChanged(CurrentBlendStateProperty, newValue, originalValue);
                });
            }
        }
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property.Name == nameof(IValueEditor.Collection)) {
            this.ResetShaderEditors();
        }
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<RenderSettings> args) {
        base.OnValueChanged(args);
        this.Reset();
    }

    private void AssetGuidEditorOnValueChanged(object sender, ValueChangedEventArgs<object> e) {
        this.ResetShaderEditors();
    }

    private void ClearEditors() {
        if (this._shaderEditors.Any()) {
            this.Collection.RemoveControls(this._shaderEditors);
            this.UnsubscribeFromShaderChange();
            this._shaderEditors.Clear();
            this._valueControlService.ReturnControls(this._controlCollection);
            this._controlCollection = null;
        }
    }

    private void Reset() {
        var originalBlendState = this._currentBlendState;
        this._currentBlendState = this.Value.GetRenderPriorityBlendStateType(this.SelectedPriority);
        this.RaisePropertyChanged(CurrentBlendStateProperty, originalBlendState, this._currentBlendState);

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

        this.ResetShaderEditors();
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

    private void ResetShaderEditors() {
        try {
            this.IgnoreUpdates = true;
            this.ClearEditors();

            if (this._valueControlService != null && this.Collection != null) {
                var shaderReference = this.Value.GetShaderForRenderPriority(this.SelectedPriority);
                shaderReference.Initialize(this._assetManager, BaseGame.Empty);
                this._controlCollection = this._valueControlService.CreateControl(shaderReference, "Shader");

                if (this._controlCollection != null) {
                    this._shaderEditors.Clear();
                    this._shaderEditors.AddRange(this._controlCollection.ValueControls);

                    if (this._shaderEditors.OfType<IValueEditor<Guid>>().FirstOrDefault(x => x.ValuePropertyName == nameof(ShaderReference.ContentId)) is { } shaderEditor) {
                        shaderEditor.Title = "Shader";
                    }

                    this.Collection.AddControls(this._shaderEditors);
                    this.SubscribeToShaderChange();
                }
            }
        }
        finally {
            this.IgnoreUpdates = false;
        }
    }

    private void SubscribeToShaderChange() {
        if (this._shaderEditors.OfType<AssetGuidEditor>().FirstOrDefault() is { } assetGuidEditor) {
            assetGuidEditor.ValueChanged += this.AssetGuidEditorOnValueChanged;
        }
    }

    private void UnsubscribeFromShaderChange() {
        if (this._shaderEditors.OfType<AssetGuidEditor>().FirstOrDefault() is { } assetGuidEditor) {
            assetGuidEditor.ValueChanged -= this.AssetGuidEditorOnValueChanged;
        }
    }
}