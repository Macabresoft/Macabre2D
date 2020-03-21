namespace Macabre2D.Engine.Windows.Views {

    using Macabre2D.Engine.Windows.Common;
    using Macabre2D.Engine.Windows.ViewModels;
    using System.Windows.Controls;

    public partial class ProjectView : UserControl {

        public ProjectView() {
            this.DataContext = ViewContainer.Resolve<ProjectViewModel>();
            this.InitializeComponent();
        }
    }
}