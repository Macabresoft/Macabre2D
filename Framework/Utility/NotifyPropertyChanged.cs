namespace Macabre2D.Framework {

    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A base class that implements <see cref="INotifyPropertyChanged"/>
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged {

        /// <summary>
        /// Occurs when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public void RaisePropertyChanged([CallerMemberName] string propertyName = "") {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Disposes <see cref="PropertyChanged"/>.
        /// </summary>
        protected void DisposePropertyChanged() {
            this.PropertyChanged = null;
        }

        /// <summary>
        /// Sets the specified field.
        /// </summary>
        /// <typeparam name="T">The type being set.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>A value indicating whether or not the value was successfully set.</returns>
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
            return this.Set(ref field, value, MacabreGame.Instance?.IsDesignMode != false, propertyName);
        }

        /// <summary>
        /// Sets the specified field.
        /// </summary>
        /// <typeparam name="T">The type being set.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="raisePropertyChanged">
        /// if set to <c>true</c> the <see cref="PropertyChanged"/> even will be raised.
        /// </param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>A value indicating whether or not the value was successfully set.</returns>
        protected virtual bool Set<T>(ref T field, T value, bool raisePropertyChanged, [CallerMemberName] string propertyName = "") {
            var result = false;
            if (!object.Equals(field, value)) {
                field = value;

                if (raisePropertyChanged) {
                    this.RaisePropertyChanged(propertyName);
                }

                result = true;
            }

            return result;
        }
    }
}