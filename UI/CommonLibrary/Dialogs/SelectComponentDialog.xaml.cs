namespace Macabre2D.UI.CommonLibrary.Dialogs {

    public partial class SelectComponentDialog {

        public SelectComponentDialog(SelectComponentViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += this.ViewModel_Finished;
            this.InitializeComponent();
        }

        public SelectComponentViewModel ViewModel {
            get {
                return this.DataContext as SelectComponentViewModel;
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