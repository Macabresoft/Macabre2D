namespace Macabresoft.Macabre2D.UI.Editor {
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media.Imaging;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common;
    using ReactiveUI;
    using Unity;

    public class AutoTileSetEditor : ValueEditorControl<AutoTileSetReference> {
        public static readonly DirectProperty<AutoTileSetEditor, Bitmap> BitmapProperty =
            AvaloniaProperty.RegisterDirect<AutoTileSetEditor, Bitmap>(
                nameof(Bitmap),
                editor => editor.Bitmap);

        public static readonly DirectProperty<AutoTileSetEditor, ICommand> ClearCommandProperty =
            AvaloniaProperty.RegisterDirect<AutoTileSetEditor, ICommand>(
                nameof(ClearCommand),
                editor => editor.ClearCommand);

        public static readonly DirectProperty<AutoTileSetEditor, string> PathTextProperty =
            AvaloniaProperty.RegisterDirect<AutoTileSetEditor, string>(
                nameof(PathText),
                editor => editor.PathText);

        public static readonly DirectProperty<AutoTileSetEditor, ICommand> SelectTileSetCommandProperty =
            AvaloniaProperty.RegisterDirect<AutoTileSetEditor, ICommand>(
                nameof(SelectTileSetCommand),
                editor => editor.SelectTileSetCommand);

        public static readonly DirectProperty<AutoTileSetEditor, AutoTileMap> TileMapProperty =
            AvaloniaProperty.RegisterDirect<AutoTileSetEditor, AutoTileMap>(
                nameof(TileMap),
                editor => editor.TileMap);

        private readonly IAssetManager _assetManager;
        private readonly IDialogService _dialogService;
        private readonly IFileSystemService _fileSystem;
        private readonly IPathService _pathService;
        private readonly IUndoService _undoService;
        private Bitmap _bitmap;

        private ICommand _clearCommand;
        private string _pathText;

        public AutoTileSetEditor() : this(
            Resolver.Resolve<IAssetManager>(),
            Resolver.Resolve<IDialogService>(),
            Resolver.Resolve<IFileSystemService>(),
            Resolver.Resolve<IPathService>(),
            Resolver.Resolve<IUndoService>()) {
        }

        [InjectionConstructor]
        public AutoTileSetEditor(
            IAssetManager assetManager,
            IDialogService dialogService,
            IFileSystemService fileSystem,
            IPathService pathService,
            IUndoService undoService) {
            this._assetManager = assetManager;
            this._dialogService = dialogService;
            this._fileSystem = fileSystem;
            this._pathService = pathService;
            this._undoService = undoService;

            this.SelectTileSetCommand = ReactiveCommand.CreateFromTask(this.SelectTileSet);
            this.InitializeComponent();
        }

        public ICommand SelectTileSetCommand { get; }

        public AutoTileMap TileMap => this.Owner as AutoTileMap;

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

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            base.Initialize(value, valueType, valuePropertyName, title, owner);
            this.RaisePropertyChanged(TileMapProperty, null, new BindingValue<AutoTileMap>(this.TileMap));
        }

        protected override void OnValueChanged() {
            base.OnValueChanged();

            if (this.Value != null) {
                this.ClearCommand = ReactiveCommand.Create(
                    this.ClearTileSet,
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

        private void ClearTileSet() {
            var asset = this.Value.Asset;
            var tileSetId = this.Value.PackagedAssetId;

            if (asset != null) {
                this._undoService.Do(
                    () => this.Value.Clear(),
                    () => {
                        this.Value.Initialize(asset);
                        this.Value.PackagedAssetId = tileSetId;
                    });
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void ResetBitmap() {
            this.PathText = null;
            this.Bitmap = null;

            if (this.Value?.PackagedAsset != null &&
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

        private async Task SelectTileSet() {
            var (spriteSheet, tileSetId) = await this._dialogService.OpenSpriteSheetAssetSelectionDialog<AutoTileSet>();
            if (spriteSheet != null) {
                var originalAsset = this.Value.Asset;
                var originalTileSetId = this.Value.PackagedAssetId;
                this._undoService.Do(
                    () => {
                        this.Value.Initialize(spriteSheet);
                        this.Value.PackagedAssetId = tileSetId;
                        this.ResetBitmap();
                    },
                    () => {
                        if (originalAsset != null) {
                            this.Value.PackagedAssetId = originalTileSetId;
                            this.Value.Initialize(originalAsset);
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
}