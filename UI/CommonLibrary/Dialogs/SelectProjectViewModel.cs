namespace Macabre2D.UI.CommonLibrary.Dialogs {

    using Macabre2D.UI.CommonLibrary.Services;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class SelectProjectViewModel : OKCancelDialogViewModel {
        private FileInfo _selectedProjectFile;

        public SelectProjectViewModel(IProjectService projectService) {
            var projectFiles = new List<FileInfo>();
            var projectFile = new FileInfo(projectService.GetPathToProject());
            var autoSaveFiles = projectService.GetAutoSaveFiles().Select(x => new FileInfo(x));
            projectFiles.Add(projectFile);
            projectFiles.AddRange(autoSaveFiles);
            this.ProjectFiles = projectFiles;
        }

        public IReadOnlyCollection<FileInfo> ProjectFiles { get; }

        public FileInfo SelectedProjectFile {
            get {
                return this._selectedProjectFile;
            }

            set {
                if (this.Set(ref this._selectedProjectFile, value)) {
                    this._okCommand.RaiseCanExecuteChanged();
                }
            }
        }

        protected override bool CanExecuteOKCommand() {
            return this.SelectedProjectFile != null;
        }
    }
}