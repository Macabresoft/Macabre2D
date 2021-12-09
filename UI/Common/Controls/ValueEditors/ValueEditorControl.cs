namespace Macabresoft.Macabre2D.UI.Common;

using System;
using System.ComponentModel;
using Avalonia;
using Macabresoft.AvaloniaEx;
using Macabresoft.Core;

public abstract class ValueEditorControl<T> : ValueControl<T>, IValueEditor<T> {
    public static readonly StyledProperty<bool> UpdateOnLostFocusProperty =
        AvaloniaProperty.Register<ValueEditorControl<T>, bool>(nameof(UpdateOnLostFocus), true);

    public event EventHandler<ValueChangedEventArgs<object>> ValueChanged;

    protected ValueEditorControl() {
    }

    protected ValueEditorControl(ValueControlDependencies dependencies) : base(dependencies) {
        if (dependencies != null) {
            this.ValuePropertyName = dependencies.ValuePropertyName;

            if (this.Owner is INotifyPropertyChanged notifyPropertyChanged) {
                notifyPropertyChanged.PropertyChanged += this.Owner_PropertyChanged;
            }
        }
    }

    public bool UpdateOnLostFocus {
        get => this.GetValue(UpdateOnLostFocusProperty);
        set => this.SetValue(UpdateOnLostFocusProperty, value);
    }

    public string ValuePropertyName { get; private set; }

    protected bool IgnoreUpdates { get; set; }

    public void SetValue(object newValue) {
        if (newValue is T typedNewValue) {
            this.Value = typedNewValue;
        }
    }

    public void SetValue(object newValue, bool ignoreUpdates) {
        var originalIgnoreUpdates = this.IgnoreUpdates;
        try {
            this.IgnoreUpdates = ignoreUpdates;
            this.SetValue(newValue);
        }
        finally {
            this.IgnoreUpdates = originalIgnoreUpdates;
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
            var eventArgs = new ValueChangedEventArgs<object>(originalValue, this.Value);
            if (eventArgs.HasChanged) {
                this.ValueChanged.SafeInvoke(this, eventArgs);
            }
        }
    }

    protected void RaiseValueChanged(object sender, ValueChangedEventArgs<object> e) {
        this.ValueChanged.SafeInvoke(sender, e);
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