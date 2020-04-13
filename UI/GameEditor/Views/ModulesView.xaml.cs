namespace Macabre2D.UI.GameEditor.Views {

    using Macabre2D.UI.Library.Common;
    using Macabre2D.UI.GameEditor.ViewModels;
    using System.Windows.Controls;

    public partial class ModulesView : UserControl {

        public ModulesView() {
            this.DataContext = ViewContainer.Resolve<ModulesViewModel>();
            InitializeComponent();
        }
    }
}