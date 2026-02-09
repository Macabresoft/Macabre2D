namespace Macabre2D.UI.Common;

using Avalonia.Markup.Xaml;
using Unity;

public partial class BoolEditor : ValueEditorControl<bool> {
    public BoolEditor() : this(null) {
    }

    [InjectionConstructor]
    public BoolEditor(ValueControlDependencies dependencies) : base(dependencies) {
        this.InitializeComponent();
    }
}