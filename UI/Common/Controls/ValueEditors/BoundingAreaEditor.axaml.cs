namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Macabresoft.Macabre2D.Framework;
using Unity;

public class BoundingAreaEditor : ValueEditorControl<BoundingArea> {
    public static readonly DirectProperty<BoundingAreaEditor, float> BottomValueProperty =
        AvaloniaProperty.RegisterDirect<BoundingAreaEditor, float>(
            nameof(BottomValue),
            editor => editor.BottomValue,
            (editor, value) => editor.BottomValue = value);

    public static readonly DirectProperty<BoundingAreaEditor, float> LeftValueProperty =
        AvaloniaProperty.RegisterDirect<BoundingAreaEditor, float>(
            nameof(LeftValue),
            editor => editor.LeftValue,
            (editor, value) => editor.LeftValue = value);

    public static readonly DirectProperty<BoundingAreaEditor, float> RightValueProperty =
        AvaloniaProperty.RegisterDirect<BoundingAreaEditor, float>(
            nameof(RightValue),
            editor => editor.RightValue,
            (editor, value) => editor.RightValue = value);

    public static readonly DirectProperty<BoundingAreaEditor, float> TopValueProperty =
        AvaloniaProperty.RegisterDirect<BoundingAreaEditor, float>(
            nameof(TopValue),
            editor => editor.TopValue,
            (editor, value) => editor.TopValue = value);

    public static readonly StyledProperty<float> XMaximumProperty =
        AvaloniaProperty.Register<BoundingAreaEditor, float>(nameof(XMaximum), float.MaxValue);

    public static readonly StyledProperty<float> XMinimumProperty =
        AvaloniaProperty.Register<BoundingAreaEditor, float>(nameof(XMinimum), float.MinValue);

    public static readonly StyledProperty<float> YMaximumProperty =
        AvaloniaProperty.Register<BoundingAreaEditor, float>(nameof(YMaximum), float.MaxValue);

    public static readonly StyledProperty<float> YMinimumProperty =
        AvaloniaProperty.Register<BoundingAreaEditor, float>(nameof(YMinimum), float.MinValue);

    private float _bottomValue;
    private float _leftValue;
    private float _rightValue;
    private float _topValue;

    public BoundingAreaEditor() : this(null) {
    }

    [InjectionConstructor]
    public BoundingAreaEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    public float BottomValue {
        get => this._bottomValue;
        set => this.SetAndRaise(BottomValueProperty, ref this._bottomValue, value);
    }

    public float LeftValue {
        get => this._leftValue;
        set => this.SetAndRaise(LeftValueProperty, ref this._leftValue, value);
    }

    public float RightValue {
        get => this._rightValue;
        set => this.SetAndRaise(RightValueProperty, ref this._rightValue, value);
    }

    public float TopValue {
        get => this._topValue;
        set => this.SetAndRaise(TopValueProperty, ref this._topValue, value);
    }

    public float XMaximum {
        get => this.GetValue(XMaximumProperty);
        set => this.SetValue(XMaximumProperty, value);
    }

    public float XMinimum {
        get => this.GetValue(XMinimumProperty);
        set => this.SetValue(XMinimumProperty, value);
    }

    public float YMaximum {
        get => this.GetValue(YMaximumProperty);
        set => this.SetValue(YMaximumProperty, value);
    }

    public float YMinimum {
        get => this.GetValue(YMinimumProperty);
        set => this.SetValue(YMinimumProperty, value);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
        base.OnAttachedToLogicalTree(e);
        this.UpdateDisplayValues();
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
        base.OnPropertyChanged(change);

        switch (change.Property.Name) {
            case nameof(this.Value):
                this.UpdateDisplayValues();
                break;
            case nameof(this.LeftValue):
                Dispatcher.UIThread.Post(() =>
                    this.SetValue(ValueProperty, new BoundingArea(this.LeftValue, this.Value.Maximum.X, this.Value.Minimum.Y, this.Value.Maximum.Y)));
                break;
            case nameof(this.RightValue):
                Dispatcher.UIThread.Post(() =>
                    this.SetValue(ValueProperty, new BoundingArea(this.Value.Minimum.X, this.RightValue, this.Value.Minimum.Y, this.Value.Maximum.Y)));
                break;
            case nameof(this.BottomValue):
                Dispatcher.UIThread.Post(() =>
                    this.SetValue(ValueProperty, new BoundingArea(this.Value.Minimum.X, this.Value.Maximum.X, this.BottomValue, this.Value.Maximum.Y)));
                break;
            case nameof(this.TopValue):
                Dispatcher.UIThread.Post(() =>
                    this.SetValue(ValueProperty, new BoundingArea(this.Value.Minimum.X, this.Value.Maximum.X, this.Value.Minimum.Y, this.TopValue)));
                break;
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void UpdateDisplayValues() {
        this._leftValue = this.Value.Minimum.X;
        this._rightValue = this.Value.Maximum.X;
        this._topValue = this.Value.Maximum.Y;
        this._bottomValue = this.Value.Minimum.Y;

        Dispatcher.UIThread.Post(() =>
        {
            this.RaisePropertyChanged(LeftValueProperty, Optional<float>.Empty, this.LeftValue);
            this.RaisePropertyChanged(RightValueProperty, Optional<float>.Empty, this.RightValue);
            this.RaisePropertyChanged(BottomValueProperty, Optional<float>.Empty, this.BottomValue);
            this.RaisePropertyChanged(TopValueProperty, Optional<float>.Empty, this.TopValue);
        });
    }
}