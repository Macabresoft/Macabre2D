namespace Macabre2D.UI.Common;

using Macabresoft.AvaloniaEx;
using Macabre2D.Framework;
using Macabre2D.Project.Common;
using Unity;

public partial class LayersOverrideEditor : ValueOverrideEditor<LayersOverride, Layers> {
    public LayersOverrideEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public LayersOverrideEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies, undoService) {
        this.InitializeComponent();
    }
}