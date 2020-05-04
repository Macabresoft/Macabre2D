namespace Macabre2D.UI.GameEditorLibrary.Dialogs {

    using Macabre2D.UI.CommonLibrary.Models;

    public partial class SaveDiscardCancelDialog {

        public SaveDiscardCancelDialog(SaveDiscardCancelViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += this.ViewModel_Finished;
            this.InitializeComponent();
        }

        public SaveDiscardCancelResult Result { get; private set; }

        public SaveDiscardCancelViewModel ViewModel {
            get {
                return this.DataContext as SaveDiscardCancelViewModel;
            }

            set {
                if (this.DataContext != value) {
                    this.DataContext = value;
                }
            }
        }

        private void ViewModel_Finished(object sender, SaveDiscardCancelResult e) {
            this.DialogResult = true;
            this.Result = e;
            this.Close();
        }
    }
}