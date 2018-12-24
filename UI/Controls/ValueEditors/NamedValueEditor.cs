namespace Macabre2D.UI.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    public class NamedValueEditor<T> : UserControl, INamedValueEditor<T>, INotifyPropertyChanged {

        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(
            nameof(PropertyName),
            typeof(string),
            typeof(NamedValueEditor<T>),
            new PropertyMetadata());

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(NamedValueEditor<T>),
            new PropertyMetadata());

        public static readonly DependencyProperty ValueChangedCommandProperty = DependencyProperty.Register(
            nameof(ValueChangedCommand),
            typeof(RelayCommand<EditableValueChangedEventArgs<T>>),
            typeof(NamedValueEditor<T>),
            new PropertyMetadata());

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(T),
            typeof(NamedValueEditor<T>),
            new PropertyMetadata(default(T), new PropertyChangedCallback(OnValueChanged)));

        public event PropertyChangedEventHandler PropertyChanged;

        public string PropertyName {
            get { return (string)this.GetValue(PropertyNameProperty); }
            set { this.SetValue(PropertyNameProperty, value); }
        }

        public string Title {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        public T Value {
            get { return (T)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        public RelayCommand<EditableValueChangedEventArgs<T>> ValueChangedCommand {
            get { return (RelayCommand<EditableValueChangedEventArgs<T>>)this.GetValue(ValueChangedCommandProperty); }
            set { this.SetValue(ValueChangedCommandProperty, value); }
        }

        protected virtual void OnValueChanged(T newValue, T oldValue, DependencyObject d) {
            if (d is INamedValueEditor<T> editor) {
                var valueChangedEventArgs = new EditableValueChangedEventArgs<T>(editor.Value, editor.PropertyName);
                if (editor.ValueChangedCommand != null && editor.ValueChangedCommand.CanExecute(valueChangedEventArgs)) {
                    editor.ValueChangedCommand.Execute(valueChangedEventArgs);
                }
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        protected virtual async Task OnValueChangedAsync(T newValue, T oldValue, DependencyObject d) {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            return;
        }

        protected void RaisePropertyChanged(string propertyName) {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static async void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is NamedValueEditor<T> editor) {
                T newValue;
                T oldValue;

                if (e.NewValue is T) {
                    newValue = (T)e.NewValue;
                }
                else {
                    newValue = default(T);
                }

                if (e.OldValue is T) {
                    oldValue = (T)e.OldValue;
                }
                else {
                    oldValue = default(T);
                }

                editor.OnValueChanged(newValue, oldValue, d);

                var valueChangedEventArgs = new EditableValueChangedEventArgs<T>(editor.Value, editor.PropertyName);
                if (editor.ValueChangedCommand != null && editor.ValueChangedCommand.CanExecute(valueChangedEventArgs)) {
                    editor.ValueChangedCommand.Execute(valueChangedEventArgs);
                }

                await editor.OnValueChangedAsync(newValue, oldValue, d);
            }
        }
    }
}