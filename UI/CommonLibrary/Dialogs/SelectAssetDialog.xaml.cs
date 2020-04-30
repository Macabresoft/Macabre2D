namespace Macabre2D.UI.CommonLibrary.Dialogs {

    public partial class SelectAssetDialog {

        public SelectAssetDialog(SelectAssetViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += this.ViewModel_Finished;
            this.InitializeComponent();
        }

        public SelectAssetViewModel ViewModel {
            get {
                return this.DataContext as SelectAssetViewModel;
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