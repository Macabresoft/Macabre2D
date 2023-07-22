namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.UI.Common;

public partial class ProjectEditorView : UserControl {
    public ProjectEditorView() {
        this.ViewModel = Resolver.Resolve<ProjectEditorViewModel>();
        this.InitializeComponent();
    }

    public ProjectEditorViewModel ViewModel { get; }

    private void OverallGrid_OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e) {
        if (sender is Control control && e.Property.Name == nameof(control.Bounds)) {
            this.ViewModel.OverallSceneArea = control.Bounds;
        }
    }


    private void ViewablePanel_OnPropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e) {
        if (sender is Control control && e.Property.Name == nameof(control.Bounds)) {
            this.ViewModel.ViewableSceneArea = control.Bounds;
        }
    }
}