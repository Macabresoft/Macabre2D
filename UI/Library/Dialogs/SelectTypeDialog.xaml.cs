namespace Macabre2D.UI.Library.Dialogs {

    using System.Windows;

    public partial class SelectTypeDialog {

        public static readonly DependencyProperty ShowNameTextBoxProperty = DependencyProperty.Register(
            nameof(ShowNameTextBox),
            typeof(bool),
            typeof(SelectTypeDialog),
            new PropertyMetadata(false));

        public SelectTypeDialog(SelectTypeViewModel viewModel) {
            this.ViewModel = viewModel;
            viewModel.Finished += this.ViewModel_Finished;
            this.InitializeComponent();
        }

        public bool ShowNameTextBox {
            get { return (bool)this.GetValue(ShowNameTextBoxProperty); }
            set { this.SetValue(ShowNameTextBoxProperty, value); }
        }

        public SelectTypeViewModel ViewModel {
            get {
                return this.DataContext as SelectTypeViewModel;
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