namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueInfo {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media.Imaging;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;

    public class SpriteSheetInfoControl : ValueControl<SpriteSheet> {
        public static readonly DirectProperty<SpriteSheetInfoControl, Bitmap> BitmapProperty =
            AvaloniaProperty.RegisterDirect<SpriteSheetInfoControl, Bitmap>(
                nameof(Bitmap),
                control => control.Bitmap);

        public static readonly DirectProperty<SpriteSheetInfoControl, ContentFile> FileProperty =
            AvaloniaProperty.RegisterDirect<SpriteSheetInfoControl, ContentFile>(
                nameof(File),
                control => control.File);

        public static readonly DirectProperty<SpriteSheetInfoControl, ObservableCollection<SpriteDisplayModel>> SpritesProperty =
            AvaloniaProperty.RegisterDirect<SpriteSheetInfoControl, ObservableCollection<SpriteDisplayModel>>(
                nameof(Sprites),
                control => control.Sprites);

        private readonly ObservableCollectionExtended<SpriteDisplayModel> _sprites = new();
        private Bitmap _bitmap;
        private ContentFile _file;

        public SpriteSheetInfoControl() {
            this.DataContext = this;
            this.InitializeComponent();
        }

        public ObservableCollection<SpriteDisplayModel> Sprites => this._sprites;

        public Bitmap Bitmap {
            get => this._bitmap;
            private set => this.SetAndRaise(BitmapProperty, ref this._bitmap, value);
        }

        public ContentFile File {
            get => this._file;
            private set => this.SetAndRaise(FileProperty, ref this._file, value);
        }

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            base.Initialize(value, valueType, valuePropertyName, title, owner);

            if (owner is ContentFile file) {
                this.File = file;
                var fileInfo = new FileInfo(file.GetFullPath());
                if (fileInfo.Exists) {
                    this.Bitmap = new Bitmap(fileInfo.FullName);
                    this.Value.PropertyChanged += this.Value_PropertyChanged;
                    this.ResetSprites();
                }
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnDetachedFromVisualTree(e);
            if (this.Value != null) {
                this.Value.PropertyChanged -= this.Value_PropertyChanged;
            }

            foreach (var sprite in this._sprites) {
                sprite.Dispose();
            }

            this._sprites.Clear();
            this._bitmap?.Dispose();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void ResetSprites() {
            var sprites = new List<SpriteDisplayModel>();

            if (this._bitmap != null) {
                byte index = 0;
                for (var y = 0; y < this.Value.Rows; y++) {
                    for (var x = 0; x < this.Value.Columns; x++) {
                        sprites.Add(new SpriteDisplayModel(this._bitmap, index, this.Value));
                        index++;
                    }
                }
            }

            this._sprites.Reset(sprites);
        }

        private void Value_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(SpriteSheet.Columns) or nameof(SpriteSheet.Rows)) {
                this.ResetSprites();
            }
        }
    }
}