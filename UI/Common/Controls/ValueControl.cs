namespace Macabresoft.Macabre2D.UI.Common;

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

public abstract class ValueControl<T> : UserControl, IValueInfo<T>, IObserver<AvaloniaPropertyChangedEventArgs<T>> {
    public static readonly StyledProperty<string> CategoryProperty =
        AvaloniaProperty.Register<ValueControl<T>, string>(nameof(Category));

    public static readonly DirectProperty<ValueControl<T>, ValueControlCollection> CollectionProperty =
        AvaloniaProperty.RegisterDirect<ValueControl<T>, ValueControlCollection>(
            nameof(Collection),
            editor => editor.Collection,
            (editor, value) => editor.Collection = value);

    public static readonly StyledProperty<object> OwnerProperty =
        AvaloniaProperty.Register<ValueControl<T>, object>(nameof(Owner));

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<ValueControl<T>, string>(nameof(Title));

    public static readonly StyledProperty<T> ValueProperty =
        AvaloniaProperty.Register<ValueControl<T>, T>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);


    private ValueControlCollection _collection;

    protected ValueControl() {
        ValueProperty.Changed.Subscribe(this);
    }

    protected ValueControl(ValueControlDependencies dependencies) {
        if (dependencies != null) {
            if (dependencies.Value is T typedValue) {
                this.Value = typedValue;
            }

            this.Owner = dependencies.Owner;
            this.Title = dependencies.Title;
        }
    }

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

    public void OnCompleted() {
    }

    public void OnError(Exception error) {
    }

    public void OnNext(AvaloniaPropertyChangedEventArgs<T> value) {
        this.OnValueChanged(value);
    }

    public void Teardown() {
        this.Owner = null;
        this.Value = default;
    }

    protected virtual void OnValueChanged(AvaloniaPropertyChangedEventArgs<T> args) {
    }
}