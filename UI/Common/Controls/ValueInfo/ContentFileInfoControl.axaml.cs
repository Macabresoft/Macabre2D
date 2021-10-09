namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using System.IO;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;

    public class ContentFileInfoControl : ValueControl<ContentFile> {
        public static readonly DirectProperty<ContentFileInfoControl, FileInfo> FileInfoProperty =
            AvaloniaProperty.RegisterDirect<ContentFileInfoControl, FileInfo>(
                nameof(FileInfo),
                control => control.FileInfo);

        private FileInfo _fileInfo;

        public ContentFileInfoControl() {
            this.InitializeComponent();
        }

        public FileInfo FileInfo {
            get => this._fileInfo;
            private set => this.SetAndRaise(FileInfoProperty, ref this._fileInfo, value);
        }

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            base.Initialize(value, valueType, valuePropertyName, title, owner);

            if (owner is ContentFile file) {
                this.FileInfo = new FileInfo(file.GetFullPath());
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}