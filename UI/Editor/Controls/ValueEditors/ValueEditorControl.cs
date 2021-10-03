namespace Macabresoft.Macabre2D.UI.Editor.Controls.ValueEditors {
    using System;
    using System.ComponentModel;
    using Avalonia;
    using Macabresoft.Core;
    using Macabresoft.Macabre2D.UI.Common;

    public abstract class ValueEditorControl<T> : ValueControl<T>, IValueEditor<T> {
        public static readonly StyledProperty<bool> UpdateOnLostFocusProperty =
            AvaloniaProperty.Register<ValueEditorControl<T>, bool>(nameof(UpdateOnLostFocus), true);

        public event EventHandler<ValueChangedEventArgs<object>> ValueChanged;

        public bool UpdateOnLostFocus {
            get => this.GetValue(UpdateOnLostFocusProperty);
            set => this.SetValue(UpdateOnLostFocusProperty, value);
        }

        public string ValuePropertyName { get; private set; }

        protected bool IgnoreUpdates { get; set; }

        public override void Initialize(object value, Type valueType, string valuePropertyName, string title, object owner) {
            base.Initialize(value, valueType, valuePropertyName, title, owner);

            this.ValuePropertyName = valuePropertyName;

            if (this.Owner is INotifyPropertyChanged notifyPropertyChanged) {
                notifyPropertyChanged.PropertyChanged += this.Owner_PropertyChanged;
            }
        }

        public void SetValue(object newValue) {
            if (newValue is T typedNewValue) {
                this.Value = typedNewValue;
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e) {
            base.OnDetachedFromVisualTree(e);

            this.ValuePropertyName = null;

            if (this.Owner is INotifyPropertyChanged notifyPropertyChanged) {
                notifyPropertyChanged.PropertyChanged -= this.Owner_PropertyChanged;
            }
        }

        protected override void OnValueChanged() {
            if (!this.IgnoreUpdates && this.Owner != null && !string.IsNullOrEmpty(this.ValuePropertyName)) {
                var originalValue = this.Owner.GetPropertyValue(this.ValuePropertyName);
                this.ValueChanged.SafeInvoke(this, new ValueChangedEventArgs<object>(originalValue, this.Value));
            }
        }

        protected void RaiseValueChanged(object sender, ValueChangedEventArgs<object> e) {
            this.ValueChanged.SafeInvoke(sender, e);
        }

        protected void SetEditorValue(T originalValue, T updatedValue) {
            this.Value = updatedValue;
            this.RaiseValueChanged(this, new ValueChangedEventArgs<object>(originalValue, updatedValue));
        }

        private void Owner_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == this.ValuePropertyName) {
                try {
                    this.IgnoreUpdates = true;
                    this.Value = (T)this.Owner.GetPropertyValue(this.ValuePropertyName);
                }
                finally {
                    this.IgnoreUpdates = false;
                }
            }
        }
    }
}