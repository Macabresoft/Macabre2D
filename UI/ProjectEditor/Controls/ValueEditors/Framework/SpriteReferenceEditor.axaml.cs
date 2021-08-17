namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueEditors.Framework {
    using System;
    using System.ComponentModel;
    using System.IO;
    using Avalonia;
    using Avalonia.Data;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media.Imaging;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Services;

    public class SpriteReferenceEditor : ValueEditorControl<SpriteReference> {
        public static readonly DirectProperty<SpriteReferenceEditor, string> ContentPathProperty =
            AvaloniaProperty.RegisterDirect<SpriteReferenceEditor, string>(
                nameof(ContentPath),
                editor => editor.ContentPath);

        public static readonly DirectProperty<SpriteReferenceEditor, BaseSpriteEntity> RenderEntityProperty =
            AvaloniaProperty.RegisterDirect<SpriteReferenceEditor, BaseSpriteEntity>(
                nameof(RenderEntity),
                editor => editor.RenderEntity);

        public static readonly DirectProperty<SpriteReferenceEditor, SpriteDisplayModel> SpriteProperty =
            AvaloniaProperty.RegisterDirect<SpriteReferenceEditor, SpriteDisplayModel>(
                nameof(Sprite),
                editor => editor.Sprite);

        private readonly IAssetManager _assetManager = Resolver.Resolve<IAssetManager>();
        private readonly IDialogService _dialogService = Resolver.Resolve<IDialogService>();
        private readonly IFileSystemService _fileSystem = Resolver.Resolve<IFileSystemService>();
        private readonly IPathService _pathService = Resolver.Resolve<IPathService>();

        private string _contentPath;
        private SpriteDisplayModel _sprite;

        public SpriteReferenceEditor() {
            this.InitializeComponent();
        }

        public BaseSpriteEntity RenderEntity => this.Owner as BaseSpriteEntity;

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

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void ResetBitmap() {
            if (this.Value != null && this._assetManager.TryGetMetadata(this.Value.ContentId, out var metadata) && metadata != null) {
                this.ContentPath = $"{metadata.GetContentPath()}{metadata.ContentFileExtension}";
                var fullPath = Path.Combine(this._pathService.ContentDirectoryPath, this.ContentPath);
                if (this._fileSystem.DoesFileExist(fullPath)) {
                    var bitmap = new Bitmap(fullPath);
                    if (bitmap.PixelSize != PixelSize.Empty) {
                        this.Sprite = new SpriteDisplayModel(bitmap, this.Value.SpriteIndex, this.Value.Asset);
                    }
                }
            }
            else {
                this.Sprite = null;
            }
        }

        private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(SpriteReference.ContentId) or nameof(SpriteReference.SpriteIndex)) {
                this.ResetBitmap();
            }
        }
    }
}