namespace Macabre2D.UI.GameEditor.Views {

    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.GameEditor.ViewModels;
    using System.Windows.Controls;

    public partial class ComponentsView : UserControl {

        public ComponentsView() {
            this.DataContext = ViewContainer.Resolve<ComponentsViewModel>();
            InitializeComponent();
        }
    }
}