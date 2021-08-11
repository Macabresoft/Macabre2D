namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueInfo {
    using System;
    using System.IO;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media.Imaging;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Services;

    public class SpriteSheetControl : ValueControl<SpriteSheet> {
        public static readonly DirectProperty<SpriteSheetControl, FileInfo> FileInfoProperty =
            AvaloniaProperty.RegisterDirect<SpriteSheetControl, FileInfo>(
                nameof(FileInfo),
                editor => editor.FileInfo);
        
        public static readonly DirectProperty<SpriteSheetControl, Bitmap> BitmapProperty =
            AvaloniaProperty.RegisterDirect<SpriteSheetControl, Bitmap>(
                nameof(Bitmap),
                editor => editor.Bitmap);

        private FileInfo _fileInfo;
        private Bitmap _bitmap;

        public SpriteSheetControl() {
            this.InitializeComponent();
        }

        public FileInfo FileInfo {
            get => this._fileInfo;
            private set => this.SetAndRaise(FileInfoProperty, ref this._fileInfo, value);
        }

        public Bitmap Bitmap {
            get => this._bitmap;
            private set => this.SetAndRaise(BitmapProperty, ref this._bitmap, value);
        }

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            base.Initialize(value, valueType, valuePropertyName, title, owner);

            if (owner is ContentFile file) {
                this.FileInfo = new FileInfo(file.GetFullPath());
                if (this.FileInfo.Exists) {
                    this.Bitmap = new Bitmap(this.FileInfo.FullName);
                }
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}