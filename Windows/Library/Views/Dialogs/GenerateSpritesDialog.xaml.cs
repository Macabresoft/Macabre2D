namespace Macabre2D.Engine.Windows.Views.Dialogs {

    using Macabre2D.Engine.Windows.ViewModels.Dialogs;

    public partial class GenerateSpritesDialog {

        public GenerateSpritesDialog(GenerateSpritesViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += this.ViewModel_Finished;
            this.InitializeComponent();
        }

        public GenerateSpritesViewModel ViewModel {
            get {
                return this.DataContext as GenerateSpritesViewModel;
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