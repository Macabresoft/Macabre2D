namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueEditors.Framework {
    using System;
    using System.IO;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media.Imaging;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Services;

    public class SpriteReferenceEditor : ValueEditorControl<SpriteReference> {
        public static readonly DirectProperty<SpriteReferenceEditor, SpriteDisplayModel> SpriteProperty =
            AvaloniaProperty.RegisterDirect<SpriteReferenceEditor, SpriteDisplayModel>(
                nameof(Sprite),
                editor => editor.Sprite);

        private readonly IAssetManager _assetManager = Resolver.Resolve<IAssetManager>();
        private readonly IDialogService _dialogService = Resolver.Resolve<IDialogService>();
        private readonly IFileSystemService _fileSystem = Resolver.Resolve<IFileSystemService>();
        private readonly IPathService _pathService = Resolver.Resolve<IPathService>();

        private SpriteDisplayModel _sprite;

        public SpriteReferenceEditor() {
            this.InitializeComponent();
        }

        public SpriteDisplayModel Sprite {
            get => this._sprite;
            private set => this.SetAndRaise(SpriteProperty, ref this._sprite, value);
        }

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            base.Initialize(value, valueType, valuePropertyName, title, owner);
            this.ResetBitmap();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }


        private void ResetBitmap() {
            if (this.Value != null && this._assetManager.TryGetMetadata(this.Value.ContentId, out var metadata) && metadata != null) {
                var contentPath = Path.Combine(this._pathService.ContentDirectoryPath, $"{metadata.GetContentPath()}{metadata.ContentFileExtension}");
                if (this._fileSystem.DoesFileExist(contentPath)) {
                    var bitmap = new Bitmap(contentPath);
                    if (bitmap.PixelSize != PixelSize.Empty) {
                        this.Sprite = new SpriteDisplayModel(bitmap, this.Value.SpriteIndex, this.Value.Asset);
                    }
                }
            }
            else {
                this.Sprite = null;
            }
        }
    }
}