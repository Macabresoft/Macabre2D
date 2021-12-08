namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Xna.Framework;
using Unity;

public class Vector2Editor : ValueEditorControl<Vector2> {
    public static readonly StyledProperty<float> XMaximumProperty =
        AvaloniaProperty.Register<Vector2Editor, float>(nameof(XMaximum), float.MaxValue);

    public static readonly StyledProperty<float> XMinimumProperty =
        AvaloniaProperty.Register<Vector2Editor, float>(nameof(XMinimum), float.MinValue);

    public static readonly DirectProperty<Vector2Editor, float> XValueProperty =
        AvaloniaProperty.RegisterDirect<Vector2Editor, float>(
            nameof(XValue),
            editor => editor.XValue,
            (editor, value) => editor.XValue = value);

    public static readonly StyledProperty<float> YMaximumProperty =
        AvaloniaProperty.Register<Vector2Editor, float>(nameof(YMaximum), float.MaxValue);

    public static readonly StyledProperty<float> YMinimumProperty =
        AvaloniaProperty.Register<Vector2Editor, float>(nameof(YMinimum), float.MinValue);

    public static readonly DirectProperty<Vector2Editor, float> YValueProperty =
        AvaloniaProperty.RegisterDirect<Vector2Editor, float>(
            nameof(YValue),
            editor => editor.YValue,
            (editor, value) => editor.YValue = value);

    private float _xValue;
    private float _yValue;

    public Vector2Editor() : this(null) {
    }

    [InjectionConstructor]
    public Vector2Editor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    public float XMaximum {
        get => this.GetValue(XMaximumProperty);
        set => this.SetValue(XMaximumProperty, value);
    }

    public float XMinimum {
        get => this.GetValue(XMinimumProperty);
        set => this.SetValue(XMinimumProperty, value);
    }

    public float XValue {
        get => this._xValue;
        set => this.SetAndRaise(XValueProperty, ref this._xValue, value);
    }

    public float YMaximum {
        get => this.GetValue(YMaximumProperty);
        set => this.SetValue(YMaximumProperty, value);
    }

    public float YMinimum {
        get => this.GetValue(YMinimumProperty);
        set => this.SetValue(YMinimumProperty, value);
    }

    public float YValue {
        get => this._yValue;
        set => this.SetAndRaise(YValueProperty, ref this._yValue, value);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e) {
        base.OnAttachedToLogicalTree(e);
        this.UpdateDisplayValues();
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change) {
        base.OnPropertyChanged(change);

        if (change.Property.Name == nameof(this.Value)) {
            this.UpdateDisplayValues();
        }
        else if (change.Property.Name == nameof(this.XValue)) {
            Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Vector2(this.XValue, this.Value.Y)));
        }
        else if (change.Property.Name == nameof(this.YValue)) {
            Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Vector2(this.Value.X, this.YValue)));
        }
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    private void UpdateDisplayValues() {
        this._xValue = this.Value.X;
        this._yValue = this.Value.Y;
        Dispatcher.UIThread.Post(() => {
            this.RaisePropertyChanged(XValueProperty, Optional<float>.Empty, this.XValue);
            this.RaisePropertyChanged(YValueProperty, Optional<float>.Empty, this.YValue);
        });
    }
}