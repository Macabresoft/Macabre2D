namespace Macabresoft.Macabre2D.UI.ProjectEditor.Controls.ValueEditors {
    using Avalonia;
    using Avalonia.Data;
    using Avalonia.LogicalTree;
    using Avalonia.Markup.Xaml;
    using Avalonia.Threading;
    using Microsoft.Xna.Framework;
    using Point = Microsoft.Xna.Framework.Point;

    public class PointEditor : ValueEditorControl<Point> {
        public static readonly StyledProperty<int> XMaximumProperty =
            AvaloniaProperty.Register<PointEditor, int>(nameof(XMaximum), int.MaxValue);

        public static readonly StyledProperty<int> XMinimumProperty =
            AvaloniaProperty.Register<PointEditor, int>(nameof(XMinimum), int.MinValue);

        public static readonly DirectProperty<PointEditor, int> XValueProperty =
            AvaloniaProperty.RegisterDirect<PointEditor, int>(
                nameof(XValue),
                editor => editor.XValue,
                (editor, value) => editor.XValue = value);

        public static readonly StyledProperty<int> YMaximumProperty =
            AvaloniaProperty.Register<PointEditor, int>(nameof(YMaximum), int.MaxValue);

        public static readonly StyledProperty<int> YMinimumProperty =
            AvaloniaProperty.Register<PointEditor, int>(nameof(YMinimum), int.MinValue);

        public static readonly DirectProperty<PointEditor, int> YValueProperty =
            AvaloniaProperty.RegisterDirect<PointEditor, int>(
                nameof(YValue),
                editor => editor.YValue,
                (editor, value) => editor.YValue = value);

        private int _xValue;
        private int _yValue;

        public PointEditor() {
            this.InitializeComponent();
        }

        public int XMaximum {
            get => this.GetValue(XMaximumProperty);
            set => this.SetValue(XMaximumProperty, value);
        }

        public int XMinimum {
            get => this.GetValue(XMinimumProperty);
            set => this.SetValue(XMinimumProperty, value);
        }

        public int XValue {
            get => this._xValue;
            set => this.SetAndRaise(XValueProperty, ref this._xValue, value);
        }

        public int YMaximum {
            get => this.GetValue(YMaximumProperty);
            set => this.SetValue(YMaximumProperty, value);
        }

        public int YMinimum {
            get => this.GetValue(YMinimumProperty);
            set => this.SetValue(YMinimumProperty, value);
        }

        public int YValue {
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
                Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Point(this.XValue, this.Value.Y)));
            }
            else if (change.Property.Name == nameof(this.YValue)) {
                Dispatcher.UIThread.Post(() => this.SetValue(ValueProperty, new Point(this.Value.X, this.YValue)));
            }
        }

        private void InitializeComponent() {
            AvaloniaXamlLoader.Load(this);
        }

        private void UpdateDisplayValues() {
            this._xValue = this.Value.X;
            this._yValue = this.Value.Y;
            Dispatcher.UIThread.Post(() => {
                this.RaisePropertyChanged(XValueProperty, Optional<int>.Empty, this.XValue);
                this.RaisePropertyChanged(YValueProperty, Optional<int>.Empty, this.YValue);
            });
        }
    }
}