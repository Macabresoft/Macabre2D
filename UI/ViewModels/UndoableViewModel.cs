namespace Macabre2D.UI.ViewModels {

    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A view model that supports undoing property changed events.
    /// </summary>
    /// <seealso cref="NotifyPropertyChanged"/>
    public class UndoableViewModel : NotifyPropertyChanged {
        private readonly IUndoService _undoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoableViewModel"/> class.
        /// </summary>
        /// <param name="undoService">The undo service.</param>
        public UndoableViewModel(IUndoService undoService) {
            this._undoService = undoService;
        }

        /// <summary>
        /// Sets the field with undo, also notifying of property changed.
        /// </summary>
        /// <typeparam name="T">The type to set.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>A value indicating whether or not the set actually occurred.</returns>
        protected virtual bool SetWithUndo<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
            if (this.Set(ref field, value, propertyName)) {
            }

            return false;
        }
    }
}