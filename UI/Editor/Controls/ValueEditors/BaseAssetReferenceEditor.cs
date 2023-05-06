namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using ReactiveUI;
using Unity;

public abstract class BaseAssetReferenceEditor<TAssetReference, TAsset> : ValueEditorControl<TAssetReference> where TAssetReference : class, IAssetReference<TAsset> where TAsset : class, IAsset {
    public static readonly DirectProperty<BaseAssetReferenceEditor<TAssetReference, TAsset>, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<BaseAssetReferenceEditor<TAssetReference, TAsset>, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<BaseAssetReferenceEditor<TAssetReference, TAsset>, string> PathTextProperty =
        AvaloniaProperty.RegisterDirect<BaseAssetReferenceEditor<TAssetReference, TAsset>, string>(
            nameof(PathText),
            editor => editor.PathText);

    public static readonly DirectProperty<BaseAssetReferenceEditor<TAssetReference, TAsset>, ICommand> SelectCommandProperty =
        AvaloniaProperty.RegisterDirect<BaseAssetReferenceEditor<TAssetReference, TAsset>, ICommand>(
            nameof(SelectCommand),
            editor => editor.SelectCommand);


    private ICommand _clearCommand;
    private string _pathText;

    public BaseAssetReferenceEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ILocalDialogService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public BaseAssetReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ILocalDialogService dialogService,
        IUndoService undoService) : base(dependencies) {
        this.AssetManager = assetManager;
        this.DialogService = dialogService;
        this.UndoService = undoService;

        this.SelectCommand = ReactiveCommand.CreateFromTask(this.Select);
        this.ResetPath();
    }

    public ICommand SelectCommand { get; }

    public ICommand ClearCommand {
        get => this._clearCommand;
        private set => this.SetAndRaise(ClearCommandProperty, ref this._clearCommand, value);
    }

    public string PathText {
        get => this._pathText;
        private set => this.SetAndRaise(PathTextProperty, ref this._pathText, value);
    }

    protected IAssetManager AssetManager { get; }

    protected ILocalDialogService DialogService { get; }

    protected IUndoService UndoService { get; }

    protected override void OnValueChanged() {
        base.OnValueChanged();

        if (this.Value != null) {
            this.ClearCommand = ReactiveCommand.Create(
                this.Clear,
                this.Value.WhenAny(x => x.ContentId, y => y.Value != Guid.Empty));

            this.ResetPath();
            this.Value.PropertyChanged += this.Value_PropertyChanged;
        }
    }

    protected override void OnValueChanging() {
        base.OnValueChanging();

        if (this.Value != null) {
            this.Value.PropertyChanged -= this.Value_PropertyChanged;
        }
    }

    protected virtual async Task Select() {
        var contentNode = await this.DialogService.OpenAssetSelectionDialog(typeof(AudioClipAsset), false);
        if (contentNode is ContentFile { Asset: TAsset newAsset }) {
            var originalAsset = this.Value.Asset;
            this.UndoService.Do(
                () => { this.Value.LoadAsset(newAsset); },
                () =>
                {
                    if (originalAsset != null) {
                        this.Value.LoadAsset(originalAsset);
                    }
                    else {
                        this.Value.Clear();
                    }
                });
        }
    }

    private void Clear() {
        var asset = this.Value.Asset;

        if (asset != null) {
            this.UndoService.Do(
                () => this.Value.Clear(),
                () => { this.Value.LoadAsset(asset); });
        }
    }

    private void ResetPath() {
        this.PathText = null;

        if (this.AssetManager != null &&
            this.Value?.Asset != null &&
            this.Value.ContentId != Guid.Empty &&
            this.AssetManager.TryGetMetadata(this.Value.ContentId, out var metadata) &&
            metadata != null) {
            this.PathText = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
        }
    }

    private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(AudioClipReference.ContentId)) {
            this.ResetPath();
        }
    }
}