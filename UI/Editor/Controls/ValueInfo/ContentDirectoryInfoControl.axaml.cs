namespace Macabresoft.Macabre2D.UI.Editor.Controls.ValueInfo {
    using System;
    using System.IO;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Macabresoft.Macabre2D.UI.Common;

    public class ContentDirectoryInfoControl : ValueControl<ContentDirectory> {
        public static readonly DirectProperty<ContentDirectoryInfoControl, DirectoryInfo> DirectoryInfoProperty =
            AvaloniaProperty.RegisterDirect<ContentDirectoryInfoControl, DirectoryInfo>(
                nameof(DirectoryInfo),
                control => control.DirectoryInfo);

        private DirectoryInfo _directoryInfo;

        public ContentDirectoryInfoControl() {
            this.InitializeComponent();
        }

        public DirectoryInfo DirectoryInfo {
            get => this._directoryInfo;
            private set => this.SetAndRaise(DirectoryInfoProperty, ref this._directoryInfo, value);
        }

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            base.Initialize(value, valueType, valuePropertyName, title, owner);

            if (owner is ContentDirectory directory) {
                this.DirectoryInfo = new DirectoryInfo(directory.GetFullPath());
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}