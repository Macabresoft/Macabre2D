namespace Macabresoft.MonoGame.Core {

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
            this.RaisePropertyChanged(false, propertyName);
        }

        public void RaisePropertyChanged(bool forceEvent, [CallerMemberName] string propertyName = "") {
            if (forceEvent || MacabreGame.Instance?.IsDesignMode != false) {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
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
            return this.Set(ref field, value, false, propertyName);
        }

        /// <summary>
        /// Sets the specified field.
        /// </summary>
        /// <typeparam name="T">The type being set.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="forcePropertyChangedEvent">
        /// if set to <c>true</c> the <see cref="PropertyChanged"/> even will be raised.
        /// </param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>A value indicating whether or not the value was successfully set.</returns>
        protected virtual bool Set<T>(ref T field, T value, bool forcePropertyChangedEvent, [CallerMemberName] string propertyName = "") {
            var result = false;
            if (!object.Equals(field, value)) {
                field = value;
                this.RaisePropertyChanged(forcePropertyChangedEvent, propertyName);
                result = true;
            }

            return result;
        }
    }
}