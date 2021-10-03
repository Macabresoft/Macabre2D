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

    public class SpriteAnimationEditor : ValueEditorControl<SpriteAnimationReference> {
        public static readonly DirectProperty<SpriteAnimationEditor, SpriteAnimation> AnimationProperty =
            AvaloniaProperty.RegisterDirect<SpriteAnimationEditor, SpriteAnimation>(
                nameof(Animation),
                editor => editor.Animation);

        public static readonly DirectProperty<SpriteAnimationEditor, Bitmap> BitmapProperty =
            AvaloniaProperty.RegisterDirect<SpriteAnimationEditor, Bitmap>(
                nameof(Bitmap),
                editor => editor.Bitmap);

        public static readonly DirectProperty<SpriteAnimationEditor, ICommand> ClearCommandProperty =
            AvaloniaProperty.RegisterDirect<SpriteAnimationEditor, ICommand>(
                nameof(ClearCommand),
                editor => editor.ClearCommand);

        public static readonly DirectProperty<SpriteAnimationEditor, string> PathTextProperty =
            AvaloniaProperty.RegisterDirect<SpriteAnimationEditor, string>(
                nameof(PathText),
                editor => editor.PathText);

        public static readonly DirectProperty<SpriteAnimationEditor, ICommand> SelectAnimationCommandProperty =
            AvaloniaProperty.RegisterDirect<SpriteAnimationEditor, ICommand>(
                nameof(SelectAnimationCommand),
                editor => editor.SelectAnimationCommand);

        private readonly IAssetManager _assetManager;
        private readonly IDialogService _dialogService;
        private readonly IFileSystemService _fileSystem;
        private readonly IPathService _pathService;
        private readonly IUndoService _undoService;
        private Bitmap _bitmap;

        private ICommand _clearCommand;
        private string _pathText;

        public SpriteAnimationEditor() : this(
            Resolver.Resolve<IAssetManager>(),
            Resolver.Resolve<IDialogService>(),
            Resolver.Resolve<IFileSystemService>(),
            Resolver.Resolve<IPathService>(),
            Resolver.Resolve<IUndoService>()) {
        }

        [InjectionConstructor]
        public SpriteAnimationEditor(
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

            this.SelectAnimationCommand = ReactiveCommand.CreateFromTask(this.SelectAnimation);
            this.InitializeComponent();
        }

        public SpriteAnimation Animation => this.Owner as SpriteAnimation;

        public ICommand SelectAnimationCommand { get; }

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
            this.RaisePropertyChanged(AnimationProperty, null, new BindingValue<SpriteAnimation>(this.Animation));
        }

        protected override void OnValueChanged() {
            base.OnValueChanged();

            if (this.Value != null) {
                this.ClearCommand = ReactiveCommand.Create(
                    this.ClearAnimation,
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

        private void ClearAnimation() {
            var asset = this.Value.Asset;
            var animationId = this.Value.PackagedAssetId;

            if (asset != null) {
                this._undoService.Do(
                    () => this.Value.Clear(),
                    () => {
                        this.Value.Initialize(asset);
                        this.Value.PackagedAssetId = animationId;
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

        private async Task SelectAnimation() {
            var (spriteSheet, animationId) = await this._dialogService.OpenSpriteSheetAssetSelectionDialog<SpriteAnimation>();
            if (spriteSheet != null) {
                var originalAsset = this.Value.Asset;
                var originalAnimationId = this.Value.PackagedAssetId;
                this._undoService.Do(
                    () => {
                        this.Value.Initialize(spriteSheet);
                        this.Value.PackagedAssetId = animationId;
                        this.ResetBitmap();
                    },
                    () => {
                        if (originalAsset != null) {
                            this.Value.PackagedAssetId = originalAnimationId;
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