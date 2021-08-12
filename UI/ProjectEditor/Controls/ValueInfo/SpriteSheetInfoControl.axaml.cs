namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueInfo {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Avalonia.Media.Imaging;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;

    public class SpriteSheetInfoControl : ValueControl<SpriteSheet> {
        public static readonly DirectProperty<SpriteSheetInfoControl, Bitmap> BitmapProperty =
            AvaloniaProperty.RegisterDirect<SpriteSheetInfoControl, Bitmap>(
                nameof(Bitmap),
                control => control.Bitmap);

        public static readonly DirectProperty<SpriteSheetInfoControl, FileInfo> FileInfoProperty =
            AvaloniaProperty.RegisterDirect<SpriteSheetInfoControl, FileInfo>(
                nameof(FileInfo),
                control => control.FileInfo);

        private readonly ObservableCollectionExtended<object> _gridItems = new();

        private Bitmap _bitmap;
        private FileInfo _fileInfo;

        public SpriteSheetInfoControl() {
            this.DataContext = this;
            this.InitializeComponent();
        }
        
        public Bitmap Bitmap {
            get => this._bitmap;
            private set => this.SetAndRaise(BitmapProperty, ref this._bitmap, value);
        }

        public FileInfo FileInfo {
            get => this._fileInfo;
            private set => this.SetAndRaise(FileInfoProperty, ref this._fileInfo, value);
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

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnDetachedFromVisualTree(e);
            this.Bitmap?.Dispose();
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}