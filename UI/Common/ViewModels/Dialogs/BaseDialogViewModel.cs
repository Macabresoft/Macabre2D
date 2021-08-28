namespace Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs {
    using System;
    using System.Windows.Input;
    using Macabresoft.Core;
    using ReactiveUI;

    /// <summary>
    /// A base dialog view models.
    /// </summary>
    public class BaseDialogViewModel : ViewModelBase {
        /// <summary>
        /// An event that occurs when the dialog should close.
        /// </summary>
        public EventHandler<bool> CloseRequested;

        private bool _isOkEnabled = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDialogViewModel" /> class.
        /// </summary>
        protected BaseDialogViewModel() {
            this.CancelCommand = ReactiveCommand.Create(this.OnCancel);
            this.OkCommand = ReactiveCommand.Create(
                this.OnOk,
                this.WhenAny(x => x.IsOkEnabled, y => y.Value));
        }

        /// <summary>
        /// A command which cancels the dialog.
        /// </summary>
        public ICommand CancelCommand { get; }

        /// <summary>
        /// A command which provides the results of the dialog.
        /// </summary>
        public ICommand OkCommand { get; }

        /// <summary>
        /// Gets a value indicating whether or not the 'Ok' button is enabled.
        /// </summary>
        public bool IsOkEnabled {
            get => this._isOkEnabled;
            set => this.RaiseAndSetIfChanged(ref this._isOkEnabled, value);
        }

        /// <summary>
        /// Called when a user has regrets and sheepishly cancels.
        /// </summary>
        protected virtual void OnCancel() {
            this.RequestClose(false);
        }

        /// <summary>
        /// Called when the user smashes that OK button.
        /// </summary>
        protected virtual void OnOk() {
            this.RequestClose(true);
        }

        private void RequestClose(bool isCancel) {
            this.CloseRequested.SafeInvoke(this, !isCancel);
        }
    }
}