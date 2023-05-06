namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using ReactiveUI;
using Unity;

public class SpriteSheetFontEditor : ValueEditorControl<SpriteSheetFontReference> {
    public static readonly DirectProperty<SpriteSheetFontEditor, Bitmap> BitmapProperty =
        AvaloniaProperty.RegisterDirect<SpriteSheetFontEditor, Bitmap>(
            nameof(Bitmap),
            editor => editor.Bitmap);

    public static readonly DirectProperty<SpriteSheetFontEditor, ICommand> ClearCommandProperty =
        AvaloniaProperty.RegisterDirect<SpriteSheetFontEditor, ICommand>(
            nameof(ClearCommand),
            editor => editor.ClearCommand);

    public static readonly DirectProperty<SpriteSheetFontEditor, string> PathTextProperty =
        AvaloniaProperty.RegisterDirect<SpriteSheetFontEditor, string>(
            nameof(PathText),
            editor => editor.PathText);

    public static readonly DirectProperty<SpriteSheetFontEditor, ICommand> SelectAssetCommandProperty =
        AvaloniaProperty.RegisterDirect<SpriteSheetFontEditor, ICommand>(
            nameof(SelectAssetCommand),
            editor => editor.SelectAssetCommand);

    private readonly IAssetManager _assetManager;
    private readonly ILocalDialogService _dialogService;
    private readonly IFileSystemService _fileSystem;
    private readonly IPathService _pathService;
    private readonly IUndoService _undoService;
    private Bitmap _bitmap;

    private ICommand _clearCommand;
    private string _pathText;

    public SpriteSheetFontEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ILocalDialogService>(),
        Resolver.Resolve<IFileSystemService>(),
        Resolver.Resolve<IPathService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public SpriteSheetFontEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ILocalDialogService dialogService,
        IFileSystemService fileSystem,
        IPathService pathService,
        IUndoService undoService) : base(dependencies) {
        this._assetManager = assetManager;
        this._dialogService = dialogService;
        this._fileSystem = fileSystem;
        this._pathService = pathService;
        this._undoService = undoService;

        this.SelectAssetCommand = ReactiveCommand.CreateFromTask(this.SelectAsset);
        this.ResetBitmap();
        this.InitializeComponent();
    }

    public Bitmap Bitmap {
        get => this._bitmap;
        private set => this.SetAndRaise(BitmapProperty, ref this._bitmap, value);
    }

    public ICommand ClearCommand {
        get => this._clearCommand;
        private set => this.SetAndRaise(ClearCommandProperty, ref this._clearCommand, value);
    }

    public string PathText {
        get => this._pathText;
        private set => this.SetAndRaise(PathTextProperty, ref this._pathText, value);
    }

    public ICommand SelectAssetCommand { get; }
    
    protected override void OnValueChanged() {
        base.OnValueChanged();

        if (this.Value != null) {
            this.ClearCommand = ReactiveCommand.Create(
                this.ClearAsset,
                this.Value.WhenAny(x => x.ContentId, y => y.Value != Guid.Empty));

            this.ResetBitmap();
            this.Value.PropertyChanged += this.Value_PropertyChanged;
        }
    }

    protected override void OnValueChanging() {
        base.OnValueChanging();

        if (this.Value != null) {
            this.Value.PropertyChanged -= this.Value_PropertyChanged;
        }
    }

    private void ClearAsset() {
        var asset = this.Value.Asset;
        var assetId = this.Value.PackagedAssetId;

        if (asset != null) {
            this._undoService.Do(
                () => this.Value.Clear(),
                () =>
                {
                    this.Value.LoadAsset(asset);
                    this.Value.PackagedAssetId = assetId;
                });
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void ResetBitmap() {
        this.PathText = null;
        this.Bitmap = null;

        if (this._assetManager != null &&
            this.Value?.PackagedAsset != null &&
            this.Value.ContentId != Guid.Empty &&
            this._assetManager.TryGetMetadata(this.Value.ContentId, out var metadata) &&
            metadata != null) {
            var fileName = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
            this.PathText = $"{this.Value.PackagedAsset.Name} ({fileName})";
            var fullPath = Path.Combine(this._pathService.ContentDirectoryPath, fileName);
            if (this._fileSystem.DoesFileExist(fullPath)) {
                var bitmap = new Bitmap(fullPath);
                if (bitmap.PixelSize != PixelSize.Empty) {
                    this.Bitmap = bitmap;
                }
            }
        }
    }

    private async Task SelectAsset() {
        var (spriteSheet, assetId) = await this._dialogService.OpenSpriteSheetAssetSelectionDialog<SpriteSheetFont>();
        if (spriteSheet != null) {
            var originalAsset = this.Value.Asset;
            var originalAssetId = this.Value.PackagedAssetId;
            this._undoService.Do(
                () =>
                {
                    this.Value.LoadAsset(spriteSheet);
                    this.Value.PackagedAssetId = assetId;
                    this.ResetBitmap();
                },
                () =>
                {
                    if (originalAsset != null) {
                        this.Value.PackagedAssetId = originalAssetId;
                        this.Value.LoadAsset(originalAsset);
                        this.ResetBitmap();
                    }
                    else {
                        this.Value.Clear();
                    }
                });
        }
    }

    private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(SpriteReference.ContentId) or nameof(SpriteReference.SpriteIndex) or nameof(SpriteSheet.Rows) or nameof(SpriteSheet.Columns)) {
            this.ResetBitmap();
        }
    }
}