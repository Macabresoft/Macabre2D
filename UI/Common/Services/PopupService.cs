namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// Interface for a service which handles popups.
    /// </summary>
    public interface IPopupService {
        /// <summary>
        /// The current popup.
        /// </summary>
        IPopup CurrentPopup { get; }

        /// <summary>
        /// A value indicating whether or not a popup is active.
        /// </summary>
        bool IsPopupActive { get; }

        /// <summary>
        /// Shows a popup of the specified type.
        /// </summary>
        /// <param name="popup">The popup.</param>
        /// <typeparam name="T">The type of popup.</typeparam>
        /// <returns>A value indicating whether the popup result was success; otherwise, the result was cancellation.</returns>
        bool ShowPopup<T>(out T popup) where T : class, IPopup;

        /// <summary>
        /// Tries to close the current popup.
        /// </summary>
        /// <param name="popup">The closed popup.</param>
        /// <returns>A value indicating whether or not the popup was closed.</returns>
        bool TryCloseCurrentPopup(out IPopup popup);
    }

    /// <summary>
    /// Service which handles popups.
    /// </summary>
    public class PopupService : ReactiveObject, IPopupService {
        private readonly IUnityContainer _unityContainer;
        private IPopup _currentPopup;

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupService" /> class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public PopupService(IUnityContainer unityContainer) {
            this._unityContainer = unityContainer ?? throw new ArgumentNullException(nameof(unityContainer));
        }

        /// <inheritdoc />
        public bool IsPopupActive => this.CurrentPopup != null;

        /// <inheritdoc />
        public IPopup CurrentPopup {
            get => this._currentPopup;
            private set {
                this.RaiseAndSetIfChanged(ref this._currentPopup, value);
                this.RaisePropertyChanged(nameof(this.IsPopupActive));
            }
        }

        /// <inheritdoc />
        public bool ShowPopup<T>(out T popup) where T : class, IPopup {
            var result = false;
            if (!this.IsPopupActive) {
                popup = this._unityContainer.Resolve<T>();
                if (popup != null) {
                    this.CurrentPopup = popup;
                    result = true;
                }
            }
            else {
                popup = null;
            }

            return result;
        }

        /// <inheritdoc />
        public bool TryCloseCurrentPopup(out IPopup popup) {
            popup = this.CurrentPopup;
            if (popup != null && popup.TryClose()) {
                this.CurrentPopup = null;
            }

            return this.CurrentPopup == null;
        }
    }
}