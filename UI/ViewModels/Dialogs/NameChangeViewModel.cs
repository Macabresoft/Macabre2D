namespace Macabre2D.UI.ViewModels.Dialogs {

    public sealed class NameChangeViewModel : OKCancelDialogViewModel {
        private string _newName;

        public NameChangeViewModel(string originalName, string extension) {
            this.Extension = string.IsNullOrWhiteSpace(extension) ? null : extension;
            this.NewName = originalName;
        }

        public string Extension { get; }

        public string NewName {
            get {
                return this._newName;
            }

            set {
                this.Set(ref this._newName, value);
            }
        }
    }
}