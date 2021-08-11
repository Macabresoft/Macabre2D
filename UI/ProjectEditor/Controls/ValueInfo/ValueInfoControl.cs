namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueInfo {
    using System;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Data;
    using Macabresoft.Macabre2D.UI.Common.Models;

    public class ValueInfoControl<T> : UserControl, IValueInfo<T> {
        public static readonly DirectProperty<ValueInfoControl<T>, ValueControlCollection> CollectionProperty =
            AvaloniaProperty.RegisterDirect<ValueInfoControl<T>, ValueControlCollection>(
                nameof(Collection),
                editor => editor.Collection,
                (editor, value) => editor.Collection = value);

        public static readonly StyledProperty<object> OwnerProperty =
            AvaloniaProperty.Register<ValueInfoControl<T>, object>(nameof(Owner));

        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<ValueInfoControl<T>, string>(nameof(Title));

        public static readonly StyledProperty<T> ValueProperty =
            AvaloniaProperty.Register<ValueInfoControl<T>, T>(nameof(Value), notifying: OnValueChanging, defaultBindingMode: BindingMode.TwoWay);

        public static readonly StyledProperty<string> CategoryProperty =
            AvaloniaProperty.Register<ValueInfoControl<T>, string>(nameof(Category));


        private ValueControlCollection _collection;

        public string Category {
            get => this.GetValue(CategoryProperty);
            set => this.SetValue(CategoryProperty, value);
        }

        public ValueControlCollection Collection {
            get => this._collection;
            set => this.SetAndRaise(CollectionProperty, ref this._collection, value);
        }

        public object Owner {
            get => this.GetValue(OwnerProperty);
            set => this.SetValue(OwnerProperty, value);
        }

        public string Title {
            get => this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        public T Value {
            get => this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        public virtual void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            if (value is T typedValue) {
                this.Value = typedValue;
            }

            this.Owner = owner;
            this.Title = title;
        }

        protected virtual void OnValueChanged(T updatedValue) {
        }

        private static void OnValueChanging(IAvaloniaObject control, bool isBeforeChange) {
            if (!isBeforeChange && control is ValueInfoControl<T> valueControl) {
                valueControl.OnValueChanged(valueControl.Value);
            }
        }
    }
}