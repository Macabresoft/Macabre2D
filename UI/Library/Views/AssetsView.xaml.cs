namespace Macabre2D.UI.Library.Views {

    using Macabre2D.UI.Library.Common;
    using Macabre2D.UI.Library.ViewModels;
    using System.Windows.Controls;

    public partial class AssetsView : UserControl {

        public AssetsView() {
            this.DataContext = ViewContainer.Resolve<AssetsViewModel>();
            this.InitializeComponent();
        }
    }
}