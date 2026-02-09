namespace Macabre2D.UI.Common;

using Macabresoft.AvaloniaEx;
using Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Unity;

public partial class ColorOverrideEditor : ValueOverrideEditor<ColorOverride, Color> {
    public ColorOverrideEditor() : this(null, Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public ColorOverrideEditor(ValueControlDependencies dependencies, IUndoService undoService) : base(dependencies, undoService) {
        this.InitializeComponent();
    }
}