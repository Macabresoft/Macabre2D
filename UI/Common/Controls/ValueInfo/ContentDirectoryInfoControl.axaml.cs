namespace Macabresoft.Macabre2D.UI.Common;

using System.IO;
using Avalonia;
using Avalonia.Markup.Xaml;
using Unity;

public partial class ContentDirectoryInfoControl : ValueControl<ContentDirectory> {
    public static readonly DirectProperty<ContentDirectoryInfoControl, DirectoryInfo> DirectoryInfoProperty =
        AvaloniaProperty.RegisterDirect<ContentDirectoryInfoControl, DirectoryInfo>(
            nameof(DirectoryInfo),
            control => control.DirectoryInfo);

    public ContentDirectoryInfoControl() : base() {
    }

    [InjectionConstructor]
    public ContentDirectoryInfoControl(ValueControlDependencies dependencies) : base(dependencies) {
        if (this.Owner is ContentDirectory directory) {
            this.DirectoryInfo = new DirectoryInfo(directory.GetFullPath());
        }

        this.InitializeComponent();
    }

    public DirectoryInfo DirectoryInfo { get; }
}