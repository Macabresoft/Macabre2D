namespace Macabre2D.UI.Views.Dialogs {

    using Macabre2D.UI.ViewModels.Dialogs;

    public partial class NameChangeDialog {

        public NameChangeDialog(NameChangeViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += ViewModel_Finished;
            this.InitializeComponent();
        }

        public NameChangeViewModel ViewModel {
            get {
                return this.DataContext as NameChangeViewModel;
            }

            set {
                if (this.DataContext == null) {
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