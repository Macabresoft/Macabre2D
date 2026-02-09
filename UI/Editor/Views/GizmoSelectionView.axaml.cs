namespace Macabre2D.UI.Editor;

using Avalonia.Controls;
using Macabre2D.UI.Common;

public partial class GizmoSelectionView : UserControl {
    public GizmoSelectionView() {
        this.ViewModel = Resolver.Resolve<GizmoSelectionViewModel>();
        this.InitializeComponent();
    }

    public GizmoSelectionViewModel ViewModel { get; }
}