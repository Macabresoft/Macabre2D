namespace Macabre2D.UI.Common;

using Avalonia;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Macabre2D.Framework;
using Unity;

public partial class PaddingEditor : ValueEditorControl<Padding> {
    public static readonly DirectProperty<PaddingEditor, float> BottomValueProperty =
        AvaloniaProperty.RegisterDirect<PaddingEditor, float>(
            nameof(BottomValue),
            editor => editor.BottomValue,
            (editor, value) => editor.BottomValue = value);

    public static readonly DirectProperty<PaddingEditor, float> LeftValueProperty =
        AvaloniaProperty.RegisterDirect<PaddingEditor, float>(
            nameof(LeftValue),
            editor => editor.LeftValue,
            (editor, value) => editor.LeftValue = value);

    public static readonly DirectProperty<PaddingEditor, float> RightValueProperty =
        AvaloniaProperty.RegisterDirect<PaddingEditor, float>(
            nameof(RightValue),
            editor => editor.RightValue,
            (editor, value) => editor.RightValue = value);


    public static readonly DirectProperty<PaddingEditor, float> TopValueProperty =
        AvaloniaProperty.RegisterDirect<PaddingEditor, float>(
            nameof(TopValue),
            editor => editor.TopValue,
            (editor, value) => editor.TopValue = value);

    private float _bottomValue;
    private float _leftValue;
    private float _rightValue;
    private float _topValue;

    public PaddingEditor() : this(null) {
    }

    [InjectionConstructor]
    public PaddingEditor(ValueControlDependencies dependencies) : base(dependencies) {
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

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
        base.OnAttachedToLogicalTree(e);
        this.UpdateDisplayValues();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property.Name == nameof(this.Value)) {
            this.UpdateDisplayValues();
        }
        else if (change.Property.Name == nameof(this.LeftValue)) {
            Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Padding(this.LeftValue, this.Value.Top, this.Value.Right, this.Value.Bottom)));
        }
        else if (change.Property.Name == nameof(this.TopValue)) {
            Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Padding(this.Value.Left, this.TopValue, this.Value.Right, this.Value.Bottom)));
        }
        else if (change.Property.Name == nameof(this.RightValue)) {
            Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Padding(this.Value.Left, this.Value.Top, this.RightValue, this.Value.Bottom)));
        }
        else if (change.Property.Name == nameof(this.BottomValue)) {
            Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Padding(this.Value.Left, this.Value.Top, this.Value.Right, this.BottomValue)));
        }
    }

    private void UpdateDisplayValues() {
        var oldLeft = this._leftValue;
        var oldTop = this._topValue;
        var oldRight = this._rightValue;
        var oldBottom = this._bottomValue;

        this._leftValue = this.Value.Left;
        this._topValue = this.Value.Top;
        this._rightValue = this.Value.Right;
        this._bottomValue = this.Value.Bottom;

        Dispatcher.UIThread.Post(() =>
        {
            this.RaisePropertyChanged(LeftValueProperty, oldLeft, this.LeftValue);
            this.RaisePropertyChanged(TopValueProperty, oldTop, this.TopValue);
            this.RaisePropertyChanged(LeftValueProperty, oldRight, this.RightValue);
            this.RaisePropertyChanged(TopValueProperty, oldBottom, this.BottomValue);
        });
    }
}