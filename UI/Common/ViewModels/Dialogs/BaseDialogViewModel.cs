namespace Macabresoft.Macabre2D.UI.Common.ViewModels {
    using System;
    using System.Reactive;
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
            this.CancelCommand = ReactiveCommand.Create<Unit, Unit>(x => this.RequestClose(true));
            this.OkCommand = ReactiveCommand.Create<Unit, Unit>(
                x => this.OnOk(),
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
        /// Called when the 'Ok' button is pressed.
        /// </summary>
        /// <returns>A unit.</returns>
        protected virtual Unit OnOk() {
            return this.RequestClose(false);
        }

        /// <summary>
        /// Requests the dialog to be closed.
        /// </summary>
        /// <param name="isCancel"></param>
        /// <returns></returns>
        protected Unit RequestClose(bool isCancel) {
            this.CloseRequested.SafeInvoke(this, !isCancel);
            return Unit.Default;
        }
    }
}