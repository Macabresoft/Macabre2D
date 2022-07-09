namespace Macabresoft.Macabre2D.UI.Common;

using Avalonia.Markup.Xaml;
using Macabresoft.Macabre2D.Framework;
using Unity;

public class LineStripEditor : ValueEditorControl<LineStrip> {
    public LineStripEditor() : this(null) {
    }

    [InjectionConstructor]
    public LineStripEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}