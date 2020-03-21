namespace Macabre2D.Engine.Windows.Views {

    using Macabre2D.Engine.Windows.Common;
    using Macabre2D.Engine.Windows.ViewModels;
    using System.Windows.Controls;

    public partial class ModulesView : UserControl {

        public ModulesView() {
            this.DataContext = ViewContainer.Resolve<ModulesViewModel>();
            InitializeComponent();
        }
    }
}