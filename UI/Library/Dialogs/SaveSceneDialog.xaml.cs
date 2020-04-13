namespace Macabre2D.UI.Library.Dialogs {

    using Macabre2D.UI.Library.Dialogs;

    public partial class SaveSceneDialog {

        public SaveSceneDialog(SaveSceneViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += this.ViewModel_Finished;
            this.InitializeComponent();
        }

        public SaveSceneViewModel ViewModel {
            get {
                return this.DataContext as SaveSceneViewModel;
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