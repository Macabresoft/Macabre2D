namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueEditors.Framework {
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
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    public class SpriteReferenceEditor : ValueEditorControl<SpriteReference> {
        public static readonly DirectProperty<SpriteReferenceEditor, ICommand> ClearSpriteCommandProperty =
            AvaloniaProperty.RegisterDirect<SpriteReferenceEditor, ICommand>(
                nameof(ClearSpriteCommand),
                editor => editor.ClearSpriteCommand);

        public static readonly DirectProperty<SpriteReferenceEditor, string> ContentPathProperty =
            AvaloniaProperty.RegisterDirect<SpriteReferenceEditor, string>(
                nameof(ContentPath),
                editor => editor.ContentPath);

        public static readonly DirectProperty<SpriteReferenceEditor, BaseSpriteEntity> RenderEntityProperty =
            AvaloniaProperty.RegisterDirect<SpriteReferenceEditor, BaseSpriteEntity>(
                nameof(RenderEntity),
                editor => editor.RenderEntity);

        public static readonly DirectProperty<SpriteReferenceEditor, ICommand> SelectSpriteCommandProperty =
            AvaloniaProperty.RegisterDirect<SpriteReferenceEditor, ICommand>(
                nameof(SelectSpriteCommand),
                editor => editor.SelectSpriteCommand);

        public static readonly DirectProperty<SpriteReferenceEditor, SpriteDisplayModel> SpriteProperty =
            AvaloniaProperty.RegisterDirect<SpriteReferenceEditor, SpriteDisplayModel>(
                nameof(Sprite),
                editor => editor.Sprite);

        private readonly IAssetManager _assetManager;
        private readonly IDialogService _dialogService;
        private readonly IFileSystemService _fileSystem;
        private readonly IPathService _pathService;
        private readonly IUndoService _undoService;

        private ICommand _clearSpriteCommand;
        private string _contentPath;
        private SpriteDisplayModel _sprite;

        public SpriteReferenceEditor() : this(
            Resolver.Resolve<IAssetManager>(),
            Resolver.Resolve<IDialogService>(),
            Resolver.Resolve<IFileSystemService>(),
            Resolver.Resolve<IPathService>(),
            Resolver.Resolve<IUndoService>()) {
        }

        [InjectionConstructor]
        public SpriteReferenceEditor(
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
            
            this.SelectSpriteCommand = ReactiveCommand.CreateFromTask(this.SelectSprite);
            this.InitializeComponent();
        }

        public BaseSpriteEntity RenderEntity => this.Owner as BaseSpriteEntity;

        public ICommand SelectSpriteCommand { get; }

        public ICommand ClearSpriteCommand {
            get => this._clearSpriteCommand;
            private set => this.SetAndRaise(ClearSpriteCommandProperty, ref this._clearSpriteCommand, value);
        }

        public string ContentPath {
            get => this._contentPath;
            private set => this.SetAndRaise(ContentPathProperty, ref this._contentPath, value);
        }

        public SpriteDisplayModel Sprite {
            get => this._sprite;
            private set => this.SetAndRaise(SpriteProperty, ref this._sprite, value);
        }

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            base.Initialize(value, valueType, valuePropertyName, title, owner);
            this.RaisePropertyChanged(RenderEntityProperty, null, new BindingValue<BaseSpriteEntity>(this.RenderEntity));
        }

        protected override void OnValueChanged() {
            base.OnValueChanged();

            if (this.Value != null) {
                this.ClearSpriteCommand = ReactiveCommand.Create(
                    this.ClearSprite,
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

        private void ClearSprite() {
            var asset = this.Value.Asset;
            var spriteIndex = this.Value.SpriteIndex;

            if (asset != null) {
                this._undoService.Do(
                    () => this.Value.Clear(),
                    () => {
                        this.Value.SpriteIndex = spriteIndex;
                        this.Value.Initialize(asset);
                    });
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void ResetBitmap() {
            this.ContentPath = null;
            this.Sprite = null;

            if (this.Value != null &&
                this.Value.ContentId != Guid.Empty &&
                this._assetManager.TryGetMetadata(this.Value.ContentId, out var metadata) &&
                metadata != null) {
                this.ContentPath = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
                var fullPath = Path.Combine(this._pathService.ContentDirectoryPath, this.ContentPath);
                if (this._fileSystem.DoesFileExist(fullPath)) {
                    var bitmap = new Bitmap(fullPath);
                    if (bitmap.PixelSize != PixelSize.Empty) {
                        this.Sprite = new SpriteDisplayModel(bitmap, this.Value.SpriteIndex, this.Value.Asset);
                    }
                }
            }
        }

        private async Task SelectSprite() {
            var (spriteSheet, spriteIndex) = await this._dialogService.OpenSpriteSelectionDialog();
            if (spriteSheet != null) {
                var originalAsset = this.Value.Asset;
                var originalIndex = this.Value.SpriteIndex;
                this._undoService.Do(
                    () => {
                        this.Value.SpriteIndex = spriteIndex;
                        this.Value.Initialize(spriteSheet);
                    },
                    () => {
                        if (originalAsset != null) {
                            this.Value.SpriteIndex = originalIndex;
                            this.Value.Initialize(originalAsset);
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