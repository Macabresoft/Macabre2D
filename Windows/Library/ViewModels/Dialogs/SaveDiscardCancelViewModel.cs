namespace Macabre2D.Engine.Windows.ViewModels.Dialogs {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.Engine.Windows.Models;
    using Macabre2D.Engine.Windows.ServiceInterfaces;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public sealed class SaveDiscardCancelViewModel : NotifyPropertyChanged {
        private readonly IProjectService _projectService;

        public SaveDiscardCancelViewModel(IProjectService projectService) {
            this._projectService = projectService;

            this.CancelCommand = new RelayCommand(() => this.Finished.SafeInvoke(this, SaveDiscardCancelResult.Cancel));
            this.DiscardCommand = new RelayCommand(() => this.Finished.SafeInvoke(this, SaveDiscardCancelResult.Discard));
            this.SaveCommand = new RelayCommand(async () => await this.Save());
        }

        public event EventHandler<SaveDiscardCancelResult> Finished;

        public ICommand CancelCommand { get; }

        public ICommand DiscardCommand { get; }

        public DateTime LastTimeSaved {
            get {
                return this._projectService.CurrentProject.LastTimeSaved;
            }
        }

        public ICommand SaveCommand { get; }

        private async Task Save() {
            if (this._projectService.HasChanges) {
                await this._projectService.SaveProject();
            }

            this.Finished.SafeInvoke(this, SaveDiscardCancelResult.Save);
        }
    }
}