namespace Macabre2D.UI.Views {

    using Macabre2D.UI.Common;
    using Macabre2D.UI.ViewModels;
    using System.Windows.Controls;
    using Unity;

    public partial class StartupView : UserControl {

        public StartupView() {
            this.DataContext = ViewContainer.Instance.Resolve<StartupViewModel>();
            InitializeComponent();
        }
    }
}