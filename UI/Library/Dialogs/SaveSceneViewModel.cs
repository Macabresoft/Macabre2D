namespace Macabre2D.UI.Library.Dialogs {

    using Macabre2D.UI.Library.Models;
    using Macabre2D.UI.Library.Models.Validation;
    using System.IO;

    public sealed class SaveSceneViewModel : OKCancelDialogViewModel {
        private string _fileName;
        private Asset _selectedAsset;

        public SaveSceneViewModel(Project project) : base() {
            this.Project = project;
        }

        [RequiredValidation(FieldName = "Name")]
        [FileNameValidation]
        public string FileName {
            get {
                return this._fileName;
            }

            set {
                if (this.Set(ref this._fileName, value)) {
                    this._okCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public Project Project { get; }

        [RequiredValidation]
        public Asset SelectedAsset {
            get {
                return this._selectedAsset;
            }

            set {
                if (this.Set(ref this._selectedAsset, value) &&
                    this._selectedAsset != null &&
                    typeof(SceneAsset).IsAssignableFrom(this._selectedAsset.GetType())) {
                    this.FileName = Path.GetFileNameWithoutExtension(this._selectedAsset.Name);
                }
            }
        }
    }
}