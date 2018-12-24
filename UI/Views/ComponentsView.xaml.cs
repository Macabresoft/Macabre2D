namespace Macabre2D.UI.Views {

    using Macabre2D.UI.Common;
    using Macabre2D.UI.ViewModels;
    using System.Windows.Controls;

    public partial class ComponentsView : UserControl {

        public ComponentsView() {
            this.DataContext = ViewContainer.Resolve<ComponentsViewModel>();
            InitializeComponent();
        }
    }
}