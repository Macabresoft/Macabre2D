namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia.Controls;
using Macabresoft.Macabre2D.UI.Common;

public partial class GizmoSelectionView : UserControl {
    public GizmoSelectionView() {
        this.ViewModel = Resolver.Resolve<GizmoSelectionViewModel>();
        this.InitializeComponent();
    }

    public GizmoSelectionViewModel ViewModel { get; }
}