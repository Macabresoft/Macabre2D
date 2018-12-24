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

        public EditableValueChangedEventArgs(T value, string propertyName) {
            this.Value = value;
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public T Value { get; }
    }
}