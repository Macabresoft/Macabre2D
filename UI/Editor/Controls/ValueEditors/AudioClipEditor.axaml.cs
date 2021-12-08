namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using ReactiveUI;
using Unity;

public class AudioClipEditor : ValueEditorControl<AudioClipReference> {
    public static readonly DirectProperty<AudioClipEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<AudioClipEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<AudioClipEditor, string> PathTextProperty =
        AvaloniaProperty.RegisterDirect<AudioClipEditor, string>(
            nameof(PathText),
            editor => editor.PathText);

    public static readonly DirectProperty<AudioClipEditor, ICommand> SelectCommandProperty =
        AvaloniaProperty.RegisterDirect<AudioClipEditor, ICommand>(
            nameof(SelectCommand),
            editor => editor.SelectCommand);

    private readonly IAssetManager _assetManager;
    private readonly ILocalDialogService _dialogService;
    private readonly IUndoService _undoService;

    private ICommand _clearCommand;
    private string _pathText;

    public AudioClipEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ILocalDialogService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public AudioClipEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ILocalDialogService dialogService,
        IUndoService undoService) : base(dependencies) {
        this._assetManager = assetManager;
        this._dialogService = dialogService;
        this._undoService = undoService;

        this.SelectCommand = ReactiveCommand.CreateFromTask(this.Select);
        this.ResetPath();
        this.InitializeComponent();
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

    private void Clear() {
        var asset = this.Value.Asset;

        if (asset != null) {
            this._undoService.Do(
                () => this.Value.Clear(),
                () => { this.Value.Initialize(asset); });
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void ResetPath() {
        this.PathText = null;

        if (this._assetManager != null &&
            this.Value?.Asset != null &&
            this.Value.ContentId != Guid.Empty &&
            this._assetManager.TryGetMetadata(this.Value.ContentId, out var metadata) &&
            metadata != null) {
            this.PathText = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
        }
    }

    private async Task Select() {
        var contentNode = await this._dialogService.OpenAssetSelectionDialog(typeof(AudioClip), false);
        if (contentNode is ContentFile file) {
            var originalAsset = this.Value.Asset;
            var newAsset = file.Asset as AudioClip;
            this._undoService.Do(
                () => {
                    if (newAsset != null) {
                        this.Value.Initialize(newAsset);
                    }
                    else {
                        this.Value.Clear();
                    }
                },
                () => {
                    if (originalAsset != null) {
                        this.Value.Initialize(originalAsset);
                    }
                    else {
                        this.Value.Clear();
                    }
                });
        }
    }

    private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(AudioClipReference.ContentId)) {
            this.ResetPath();
        }
    }
}