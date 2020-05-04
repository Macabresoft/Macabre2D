namespace Macabre2D.UI.GameEditorLibrary.Dialogs {

    public partial class SelectModuleDialog {

        public SelectModuleDialog(SelectModuleViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += this.ViewModel_Finished;
            this.InitializeComponent();
        }

        public SelectModuleViewModel ViewModel {
            get {
                return this.DataContext as SelectModuleViewModel;
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