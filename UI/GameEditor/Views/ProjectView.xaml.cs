namespace Macabre2D.UI.GameEditor.Views {

    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.GameEditor.ViewModels;
    using System.Windows.Controls;

    public partial class ProjectView : UserControl {

        public ProjectView() {
            this.DataContext = ViewContainer.Resolve<ProjectViewModel>();
            this.InitializeComponent();
        }
    }
}