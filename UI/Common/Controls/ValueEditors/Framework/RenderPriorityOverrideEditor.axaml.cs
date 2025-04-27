namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.Project.Common;
using Unity;

public partial class RenderPriorityOverrideEditor : ValueOverrideEditor<RenderPriorityOverride, RenderPriority> {
    public RenderPriorityOverrideEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public RenderPriorityOverrideEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies, undoService) {
        this.InitializeComponent();
    }
}