namespace Macabre2D.UI.Views {

    using Macabre2D.UI.Common;
    using Macabre2D.UI.ViewModels;
    using System.Windows.Controls;
    using Unity;

    public partial class ProjectView : UserControl {

        public ProjectView() {
            this.DataContext = ViewContainer.Instance.Resolve<ProjectViewModel>();
            InitializeComponent();
        }
    }
}