namespace Macabresoft.Macabre2D.UI.Editor;

using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Unity;

public partial class PrefabReferenceEditor : BaseAssetReferenceEditor<PrefabReference, PrefabAsset> {
    public PrefabReferenceEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ILocalDialogService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public PrefabReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ILocalDialogService dialogService,
        IUndoService undoService) : base(dependencies, assetManager, dialogService, undoService) {
        this.InitializeComponent();
    }
}