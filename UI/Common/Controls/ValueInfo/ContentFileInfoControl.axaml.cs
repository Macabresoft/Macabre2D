namespace Macabresoft.Macabre2D.UI.Common {
    using System.IO;
    using Avalonia;
    using Avalonia.Markup.Xaml;
    using Unity;

    public class ContentFileInfoControl : ValueControl<ContentFile> {
        public static readonly DirectProperty<ContentFileInfoControl, FileInfo> FileInfoProperty =
            AvaloniaProperty.RegisterDirect<ContentFileInfoControl, FileInfo>(
                nameof(FileInfo),
                control => control.FileInfo);

        public ContentFileInfoControl() : base() {
        }

        [InjectionConstructor]
        public ContentFileInfoControl(ValueControlDependencies dependencies) : base(dependencies) {
            if (dependencies.Owner is ContentFile file) {
                this.FileInfo = new FileInfo(file.GetFullPath());
            }

            this.InitializeComponent();
        }

        public FileInfo FileInfo { get; }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }
    }
}