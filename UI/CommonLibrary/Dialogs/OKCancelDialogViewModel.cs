namespace Macabre2D.UI.CommonLibrary.Dialogs {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Models.Validation;
    using System;
    using System.Windows.Input;

    public class OKCancelDialogViewModel : ValidationModel {
        protected readonly RelayCommand _cancelCommand;
        protected readonly RelayCommand _okCommand;

        public OKCancelDialogViewModel() {
            this._cancelCommand = new RelayCommand(() => this.OnFinished(false), this.CanExecuteCancelCommand);
            this._okCommand = new RelayCommand(() => this.OnFinished(true), this.CanExecuteOKCommand);
        }

        public event EventHandler<bool> Finished;

        public ICommand CancelCommand {
            get {
                return this._cancelCommand;
            }
        }

        public ICommand OKCommand {
            get {
                return this._okCommand;
            }
        }

        protected virtual bool CanExecuteCancelCommand() {
            return true;
        }

        protected virtual bool CanExecuteOKCommand() {
            return !this.HasErrors;
        }

        private void OnFinished(bool wasOk) {
            this.Finished.SafeInvoke(this, wasOk);
            this.Finished = null;
        }
    }
}