namespace Macabre2D.UI.Library.Views.Dialogs {

    using Macabre2D.UI.Library.ViewModels.Dialogs;

    /// <summary>
    /// Interaction logic for SelectProjectDialog.xaml
    /// </summary>
    public partial class SelectProjectDialog {

        public SelectProjectDialog(SelectProjectViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += this.ViewModel_Finished;
            this.InitializeComponent();
        }

        public SelectProjectViewModel ViewModel {
            get {
                return this.DataContext as SelectProjectViewModel;
            }

            set {
                if (this.DataContext != value) {
                    this.DataContext = value;
                }
            }
        }

        private void ViewModel_Finished(object sender, bool e) {
            this.DialogResult = e;
            this.Close();
        }
    }
}