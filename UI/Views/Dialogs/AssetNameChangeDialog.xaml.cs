namespace Macabre2D.UI.Views.Dialogs {

    using Macabre2D.UI.ViewModels.Dialogs;

    public partial class AssetNameChangeDialog {

        public AssetNameChangeDialog(AssetNameChangeViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += ViewModel_Finished;
            this.InitializeComponent();
        }

        public AssetNameChangeViewModel ViewModel {
            get {
                return this.DataContext as AssetNameChangeViewModel;
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