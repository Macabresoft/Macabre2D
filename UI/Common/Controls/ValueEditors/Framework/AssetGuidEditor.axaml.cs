namespace Macabre2D.UI.Common;

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabre2D.Common;
using Macabre2D.Framework;
using Macabresoft.AvaloniaEx;
using ReactiveUI;
using Unity;

public partial class AssetGuidEditor : ValueEditorControl<Guid> {

    /// <summary>
    /// Avalonia property for the fallback background property.
    /// </summary>
    public static readonly StyledProperty<Type> AssetTypeProperty = AvaloniaProperty.Register<AssetGuidEditor, Type>(
        nameof(AssetType));

    public static readonly DirectProperty<AssetGuidEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<AssetGuidEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<AssetGuidEditor, string> PathTextProperty =
        AvaloniaProperty.RegisterDirect<AssetGuidEditor, string>(
            nameof(PathText),
            editor => editor.PathText);

    public static readonly DirectProperty<AssetGuidEditor, ICommand> SelectCommandProperty =
        AvaloniaProperty.RegisterDirect<AssetGuidEditor, ICommand>(
            nameof(SelectCommand),
            editor => editor.SelectCommand);

    private readonly IAssetManager _assetManager;
    private readonly ICommonDialogService _dialogService;
    private readonly IUndoService _undoService;

    public AssetGuidEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public AssetGuidEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ICommonDialogService dialogService,
        IUndoService undoService) : base(dependencies) {
        this._assetManager = assetManager;
        this._dialogService = dialogService;
        this._undoService = undoService;

        this.ClearCommand = ReactiveCommand.Create(
            this.Clear,
            this.WhenAny(x => x.Value, y => y.Value != Guid.Empty));
        this.SelectCommand = ReactiveCommand.CreateFromTask(this.Select);

        if (dependencies is { Owner: AssetReference assetReference, ValuePropertyName: nameof(SpriteSheet.ContentId) }) {
            this.AssetType = assetReference.AssetType;
        }
        else if (dependencies?.Owner?.GetType() is { } ownerType) {
            var members = ownerType.GetMember(dependencies.ValuePropertyName);
            if (members.FirstOrDefault() is { } info) {
                if (info.GetCustomAttribute<AssetGuidAttribute>() is { } attribute) {
                    this.AssetType = attribute.AssetType;
                }
                else if (info.GetCustomAttribute<SceneGuidAttribute>() != null) {
                    this.AssetType = typeof(SceneAsset);
                }
                else if (info.GetCustomAttribute<SpriteSheetGuidAttribute>() != null) {
                    this.AssetType = typeof(SpriteSheet);
                }
                else if (info.GetCustomAttribute<ShaderGuidAttribute>() != null) {
                    this.AssetType = typeof(ShaderAsset);
                }
                else if (info.GetCustomAttribute<PrefabGuidAttribute>() != null) {
                    this.AssetType = typeof(PrefabAsset);
                }
            }
        }

        this.ResetPath();
        this.InitializeComponent();
    }


    public ICommand ClearCommand { get; }

    public ICommand SelectCommand { get; }

    /// <summary>
    /// Gets or sets the asset type.
    /// </summary>
    public Type AssetType {
        get => this.GetValue(AssetTypeProperty);
        set => this.SetValue(AssetTypeProperty, value);
    }

    public string PathText {
        get;
        private set => this.SetAndRaise(PathTextProperty, ref field, value);
    }

    protected override void OnValueChanged(AvaloniaPropertyChangedEventArgs<Guid> args) {
        base.OnValueChanged(args);
        this.ResetPath();
    }

    private void Clear() {
        var originalValue = this.Value;

        if (originalValue != Guid.Empty) {
            this._undoService.Do(
                () => this.Value = Guid.Empty,
                () => this.Value = originalValue);
        }
    }

    private void ResetPath() {
        this.PathText = null;

        if (this._assetManager != null &&
            this.Value != Guid.Empty &&
            this._assetManager.TryGetMetadata(this.Value, out var metadata)) {
            this.PathText = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
        }
    }

    private async Task Select() {
        var contentNode = await this._dialogService.OpenContentSelectionDialog(this.AssetType, false, this.Title);
        if (contentNode is ContentFile { Metadata: { } metadata }) {
            var originalId = this.Value;
            var contentId = metadata.ContentId;
            this._undoService.Do(
                () => { this.Value = contentId; },
                () => { this.Value = originalId; });
        }
    }
}