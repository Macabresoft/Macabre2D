namespace Macabre2D.UI.GameEditor.Views {

    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.GameEditor.ViewModels;
    using System.Windows.Controls;

    public partial class AssetsView : UserControl {

        public AssetsView() {
            this.DataContext = ViewContainer.Resolve<AssetsViewModel>();
            this.InitializeComponent();
        }
    }
}