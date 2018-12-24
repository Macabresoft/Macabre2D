namespace Macabre2D.UI.ViewModels.Dialogs {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.Models.Validation;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Windows.Input;

    public sealed class CreateProjectViewModel : OKCancelDialogViewModel {
        private readonly RelayCommand _browseCommand;
        private readonly IDialogService _dialogService;
        private string _filePath;

        public CreateProjectViewModel(IDialogService dialogService) : base() {
            this._browseCommand = new RelayCommand(this.Browse);
            this.Project.PropertyChanged += this.Project_PropertyChanged;
            this._dialogService = dialogService;
        }

        public ICommand BrowseCommand {
            get {
                return this._browseCommand;
            }
        }

        [PathValidation]
        [RequiredValidation(FieldName = "File Path")]
        public string FilePath {
            get {
                return this._filePath;
            }

            set {
                if (this.Set(ref this._filePath, value)) {
                    this._okCommand.RaiseCanExecuteChanged();
                }
            }
        }

        [ValidateModel]
        public Project Project { get; } = new Project(BuildPlatform.DesktopGL);

        protected override bool CanExecuteOKCommand() {
            return base.CanExecuteOKCommand() && !this.Project.HasErrors;
        }

        private void Browse() {
            if (this._dialogService.ShowFolderBrowser(out string path)) {
                this.FilePath = path;
            }
        }

        private void Project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this._okCommand.RaiseCanExecuteChanged();
        }
    }
}