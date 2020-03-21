namespace Macabre2D.Engine.Windows.Views {

    using Macabre2D.Engine.Windows.Common;
    using Macabre2D.Engine.Windows.ViewModels;
    using System.Windows.Controls;

    public partial class AssetsView : UserControl {

        public AssetsView() {
            this.DataContext = ViewContainer.Resolve<AssetsViewModel>();
            this.InitializeComponent();
        }
    }
}