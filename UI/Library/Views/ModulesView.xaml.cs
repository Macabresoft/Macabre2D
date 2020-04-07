namespace Macabre2D.UI.Library.Views {

    using Macabre2D.UI.Library.Common;
    using Macabre2D.UI.Library.ViewModels;
    using System.Windows.Controls;

    public partial class ModulesView : UserControl {

        public ModulesView() {
            this.DataContext = ViewContainer.Resolve<ModulesViewModel>();
            InitializeComponent();
        }
    }
}