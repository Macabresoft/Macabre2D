namespace Macabre2D.UI.Views.Dialogs {

    using Macabre2D.UI.ViewModels.Dialogs;

    public partial class EditTileSetDialog {

        public EditTileSetDialog(EditTileSetViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += this.ViewModel_Finished;
            this.InitializeComponent();
        }

        public EditTileSetViewModel ViewModel {
            get {
                return this.DataContext as EditTileSetViewModel;
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