namespace Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using Macabresoft.AvaloniaEx;
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

    public static readonly DirectProperty<RenderSettingsEditor, Guid> ScreenSpaceShaderAssetIdProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, Guid>(
            nameof(ScreenSpaceShaderShaderAssetId),
            editor => editor.ScreenSpaceShaderShaderAssetId,
            (editor, value) => editor.ScreenSpaceShaderShaderAssetId = value);

    public static readonly DirectProperty<RenderSettingsEditor, RenderPriority> SelectedPriorityProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, RenderPriority>(
            nameof(SelectedPriority),
            editor => editor.SelectedPriority,
            (editor, value) => editor.SelectedPriority = value);

    public static readonly DirectProperty<RenderSettingsEditor, Guid> ShaderAssetIdProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, Guid>(
            nameof(ShaderAssetId),
            editor => editor.ShaderAssetId,
            (editor, value) => editor.ShaderAssetId = value);

    public static readonly DirectProperty<RenderSettingsEditor, bool> ShareShaderWithScreenSpaceProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, bool>(
            nameof(ShareShaderWithScreenSpace),
            editor => editor.ShareShaderWithScreenSpace,
            (editor, value) => editor.ShareShaderWithScreenSpace = value);

    private readonly IUndoService _undoService;
    private BlendStateType _currentBlendState;
    private Color _currentColor;
    private bool _isOverrideEnabled;
    private bool _shareShaderWithScreenSpace;

    public RenderSettingsEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public RenderSettingsEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies) {
        this._undoService = undoService;

        this.Priorities = Enum.GetValues<RenderPriority>().ToList();

        this.InitializeComponent();
    }

    public IReadOnlyCollection<RenderPriority> Priorities { get; }

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

    public Guid ScreenSpaceShaderShaderAssetId {
        get => !this._shareShaderWithScreenSpace ? this.Value.GetScreenSpaceShaderForRenderPriority(this.SelectedPriority).ContentId : Guid.Empty;
        set {
            if (!this._shareShaderWithScreenSpace) {
                var shaderReference = this.Value.GetScreenSpaceShaderForRenderPriority(this.SelectedPriority);
                var originalValue = shaderReference.ContentId;
                if (originalValue != value) {
                    this._undoService.Do(() =>
                    {
                        shaderReference.ContentId = value;
                        this.RaisePropertyChanged(ScreenSpaceShaderAssetIdProperty, value, originalValue);
                    }, () =>
                    {
                        shaderReference.ContentId = originalValue;
                        this.RaisePropertyChanged(ScreenSpaceShaderAssetIdProperty, originalValue, value);
                    });
                }
            }
        }
    }

    public RenderPriority SelectedPriority {
        get;
        set {
            if (field != value) {
                var originalValue = field;
                field = value;
                this.RaisePropertyChanged(SelectedPriorityProperty, field, originalValue);
                this.Reset();
            }
        }
    }

    public Guid ShaderAssetId {
        get => this.Value.GetShaderForRenderPriority(this.SelectedPriority).ContentId;
        set {
            var shaderReference = this.Value.GetShaderForRenderPriority(this.SelectedPriority);
            var originalValue = shaderReference.ContentId;
            if (originalValue != value) {
                this._undoService.Do(() =>
                {
                    shaderReference.ContentId = value;
                    this.RaisePropertyChanged(ShaderAssetIdProperty, value, originalValue);
                }, () =>
                {
                    shaderReference.ContentId = originalValue;
                    this.RaisePropertyChanged(ShaderAssetIdProperty, originalValue, value);
                });
            }
        }
    }

    public bool ShareShaderWithScreenSpace {
        get => this._shareShaderWithScreenSpace;
        set {
            if (this._shareShaderWithScreenSpace != value) {
                var originalValue = this._shareShaderWithScreenSpace;
                this._shareShaderWithScreenSpace = value;
                this._undoService.Do(() =>
                {
                    var originalShaderId = this.ScreenSpaceShaderShaderAssetId;
                    this.Value.SetShareRenderPriorityShaderWithScreenSpace(this.SelectedPriority, value);
                    this.RaisePropertyChanged(ShareShaderWithScreenSpaceProperty, this._shareShaderWithScreenSpace, value);
                    this.RaisePropertyChanged(ScreenSpaceShaderAssetIdProperty, originalShaderId, this.ScreenSpaceShaderShaderAssetId);
                }, () =>
                {
                    var originalShaderId = this.ScreenSpaceShaderShaderAssetId;
                    this.Value.SetShareRenderPriorityShaderWithScreenSpace(this.SelectedPriority, value);
                    this.RaisePropertyChanged(ShareShaderWithScreenSpaceProperty, this._shareShaderWithScreenSpace, originalValue);
                    this.RaisePropertyChanged(ScreenSpaceShaderAssetIdProperty, originalShaderId, this.ScreenSpaceShaderShaderAssetId);
                });
            }
        }
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<RenderSettings> args) {
        base.OnValueChanged(args);
        this.Reset();
    }

    private void Reset() {
        var originalShaderId = this.ShaderAssetId;
        var originalScreenSpaceShaderId = this.ScreenSpaceShaderShaderAssetId;
        var originalBlendState = this._currentBlendState;
        this._currentBlendState = this.Value.GetRenderPriorityBlendStateType(this.SelectedPriority);
        var originalShareShaderWithScreenSpace = this._shareShaderWithScreenSpace;
        this._shareShaderWithScreenSpace = this.Value.CheckIfRenderPriorityShaderSharedWithScreenSpace(this.SelectedPriority);
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
        this.RaisePropertyChanged(ShareShaderWithScreenSpaceProperty, originalShareShaderWithScreenSpace, this._shareShaderWithScreenSpace);
        this.RaisePropertyChanged(ShaderAssetIdProperty, originalShaderId, this.ShaderAssetId);
        this.RaisePropertyChanged(ScreenSpaceShaderAssetIdProperty, originalScreenSpaceShaderId, this.ScreenSpaceShaderShaderAssetId);
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
}