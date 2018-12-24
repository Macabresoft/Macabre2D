namespace Macabre2D.UI.Views {

    using Macabre2D.UI.Common;
    using Macabre2D.UI.ViewModels;
    using System.Windows.Controls;

    public partial class ModulesView : UserControl {

        public ModulesView() {
            this.DataContext = ViewContainer.Resolve<ModulesViewModel>();
            InitializeComponent();
        }
    }
}