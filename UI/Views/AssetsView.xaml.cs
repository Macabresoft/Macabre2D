namespace Macabre2D.UI.Views {

    using Macabre2D.UI.Common;
    using Macabre2D.UI.ViewModels;
    using System.Windows.Controls;

    public partial class AssetsView : UserControl {

        public AssetsView() {
            this.DataContext = ViewContainer.Resolve<AssetsViewModel>();
            this.InitializeComponent();
        }
    }
}