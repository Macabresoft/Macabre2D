namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using ReactiveUI;
using Unity;

public partial class RenderSettingsEditor : ValueEditorControl<RenderSettings> {

    public static readonly DirectProperty<RenderSettingsEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<RenderSettingsEditor, Color> CurrentColorProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, Color>(
            nameof(CurrentColor),
            editor => editor.CurrentColor,
            (editor, value) => editor.CurrentColor = value);

    public static readonly DirectProperty<RenderSettingsEditor, bool> HasShaderProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, bool>(
            nameof(HasShader),
            editor => editor.HasShader);

    public static readonly DirectProperty<RenderSettingsEditor, bool> IsOverrideEnabledProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, bool>(
            nameof(IsOverrideEnabled),
            editor => editor.IsOverrideEnabled,
            (editor, value) => editor.IsOverrideEnabled = value);

    public static readonly DirectProperty<RenderSettingsEditor, string> ShaderPathTextProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, string>(
            nameof(ShaderPathText),
            editor => editor.ShaderPathText);

    public static readonly DirectProperty<RenderSettingsEditor, ICommand> SelectCommandProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, ICommand>(
            nameof(SelectCommand),
            editor => editor.SelectCommand);

    public static readonly DirectProperty<RenderSettingsEditor, RenderPriority> SelectedPriorityProperty =
        AvaloniaProperty.RegisterDirect<RenderSettingsEditor, RenderPriority>(
            nameof(SelectedPriority),
            editor => editor.SelectedPriority,
            (editor, value) => editor.SelectedPriority = value);

    private readonly IAssetManager _assetManager;
    private readonly ICommonDialogService _dialogService;

    private readonly IUndoService _undoService;
    private Color _currentColor;
    private bool _hasShader;
    private bool _isOverrideEnabled;
    private string _shaderPathText;
    private RenderPriority _selectedPriority;

    public RenderSettingsEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public RenderSettingsEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ICommonDialogService dialogService,
        IUndoService undoService) : base(dependencies) {
        this._assetManager = assetManager;
        this._dialogService = dialogService;
        this._undoService = undoService;
        var priorities = Enum.GetValues<RenderPriority>().ToList();
        this.Priorities = priorities;

        this.ClearCommand = ReactiveCommand.Create(this.Clear, this.WhenAny(x => x.HasShader, y => y.Value));
        this.SelectCommand = ReactiveCommand.CreateFromTask(this.SelectShader);

        this.InitializeComponent();
    }

    public ICommand ClearCommand { get; }

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

    public bool HasShader {
        get => this._hasShader;
        set {
            this._hasShader = value;
            this.RaisePropertyChanged(HasShaderProperty, !this._hasShader, this._hasShader);
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

    public string ShaderPathText {
        get => this._shaderPathText;
        private set => this.SetAndRaise(ShaderPathTextProperty, ref this._shaderPathText, value);
    }

    public IReadOnlyCollection<RenderPriority> Priorities { get; }

    public ICommand SelectCommand { get; }

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

    private void Clear() {
        var priority = this.SelectedPriority;
        if (this.Value.TryGetShaderIdForRenderPriority(priority, out var originalId)) {
            this.SetShader(priority, originalId, Guid.Empty);
        }
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

        this.ResetPath();
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

    private void ResetPath() {
        this.ShaderPathText = null;

        if (this._assetManager != null &&
            this.Value.TryGetShaderIdForRenderPriority(this.SelectedPriority, out var shaderId) &&
            this._assetManager.TryGetMetadata(shaderId, out var metadata)) {
            this.ShaderPathText = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
            this.HasShader = true;
        }
        else {
            this.HasShader = false;
        }
    }

    private async Task SelectShader() {
        var contentNode = await this._dialogService.OpenContentSelectionDialog(typeof(ShaderAsset), false, this.Title);
        if (contentNode is ContentFile { Metadata: { } metadata }) {
            var priority = this.SelectedPriority;
            this.Value.TryGetShaderIdForRenderPriority(priority, out var originalId);
            var newId = metadata.ContentId;
            this.SetShader(priority, originalId, newId);
        }
    }

    private void SetShader(RenderPriority priority, Guid originalId, Guid newId) {
        this._undoService.Do(() =>
        {
            this.Value.SetRenderPriorityShader(priority, newId);
            this.ResetPath();
        }, () =>
        {
            this.Value.SetRenderPriorityShader(priority, originalId);
            this.ResetPath();
        });
    }
}