namespace Macabre2D.UI.Controls.ValueEditors {

    using GalaSoft.MvvmLight.CommandWpf;
    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models;
    using Macabre2D.UI.ServiceInterfaces;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    public class NamedValueEditor<T> : UserControl, INamedValueEditor<T>, INotifyPropertyChanged {

        public static readonly DependencyProperty OwnerProperty = DependencyProperty.Register(
            nameof(Owner),
            typeof(object),
            typeof(NamedValueEditor<T>),
            new PropertyMetadata());

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

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value),
            typeof(T),
            typeof(NamedValueEditor<T>),
            new PropertyMetadata(default(T), new PropertyChangedCallback(OnValueChanged)));

        protected readonly ISceneService _sceneService = ViewContainer.Resolve<ISceneService>();
        protected readonly IUndoService _undoService = ViewContainer.Resolve<IUndoService>();

        public NamedValueEditor() {
            this.ValueChangedCommand = new RelayCommand<EditableValueChangedEventArgs<T>>(e => this.UpdateProperty(e.PropertyName, e.OldValue, e.NewValue, this.Owner, this), e => this.IsLoaded, true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public object Owner {
            get { return this.GetValue(OwnerProperty); }
            set { this.SetValue(OwnerProperty, value); }
        }

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

        public RelayCommand<EditableValueChangedEventArgs<T>> ValueChangedCommand { get; private set; }

        protected virtual void OnValueChanged(T newValue, T oldValue, DependencyObject d) {
            if (d is INamedValueEditor<T> editor) {
                var valueChangedEventArgs = new EditableValueChangedEventArgs<T>(newValue, oldValue, editor.PropertyName);
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
                    newValue = default;
                }

                if (e.OldValue is T) {
                    oldValue = (T)e.OldValue;
                }
                else {
                    oldValue = default;
                }

                editor.OnValueChanged(newValue, oldValue, d);
                await editor.OnValueChangedAsync(newValue, oldValue, d);
            }
        }

        private void UpdateProperty<T>(string propertyPath, T originalValue, T newValue, object objectToUpdate, INamedValueEditor<T> editor) {
            if (objectToUpdate != null && ((originalValue == null && newValue != null) || !originalValue.Equals(newValue))) {
                var undoCommand = new UndoCommand(
                    () => {
                        this.UpdatePropertyWithNotification(propertyPath, newValue, objectToUpdate);
                        editor.Value = newValue;
                    },
                    () => {
                        this.UpdatePropertyWithNotification(propertyPath, originalValue, objectToUpdate);
                        editor.Value = originalValue;
                    });

                this._undoService.Do(undoCommand);
            }
        }

        private void UpdatePropertyWithNotification(string propertyPath, object value, object objectToUpdate) {
            objectToUpdate.SetProperty(propertyPath, value);
            this._sceneService.HasChanges = true;
            if (objectToUpdate is NotifyPropertyChanged notifyPropertyChanged) {
                notifyPropertyChanged.RaisePropertyChanged(propertyPath);
            }
        }
    }
}