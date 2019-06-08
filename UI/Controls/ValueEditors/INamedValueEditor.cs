namespace Macabre2D.UI.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using System;

    public interface INamedValueEditor {
        string PropertyName { get; }

        string Title { get; }
    }

    public interface INamedValueEditor<T> : INamedValueEditor {
        T Value { get; set; }

        RelayCommand<EditableValueChangedEventArgs<T>> ValueChangedCommand { get; set; }
    }

    public sealed class EditableValueChangedEventArgs<T> : EventArgs {

        public EditableValueChangedEventArgs(T newValue, T oldValue, string propertyName) {
            this.NewValue = newValue;
            this.OldValue = oldValue;
            this.PropertyName = propertyName;
        }

        public T NewValue { get; }
        public T OldValue { get; }
        public string PropertyName { get; }
    }
}