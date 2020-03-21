namespace Macabre2D.Engine.Windows.Views.Dialogs {

    using Macabre2D.Engine.Windows.ViewModels.Dialogs;

    public partial class SaveAssetAsDialog {

        public SaveAssetAsDialog(SaveAssetAsViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += this.ViewModel_Finished;
            this.InitializeComponent();
        }

        public SaveAssetAsViewModel ViewModel {
            get {
                return this.DataContext as SaveAssetAsViewModel;
            }

            set {
                this.DataContext = value;
            }
        }

        private void ViewModel_Finished(object sender, bool e) {
            this.DialogResult = e;
            this.Close();
        }
    }
}