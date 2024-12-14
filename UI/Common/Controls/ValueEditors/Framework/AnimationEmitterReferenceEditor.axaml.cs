namespace Macabresoft.Macabre2D.UI.Common;

using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Unity;

public partial class AnimationEmitterReferenceEditor : BaseSpriteSheetReferenceEditor<AnimationEmitterReference> {
    public AnimationEmitterReferenceEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IFileSystemService>(),
        Resolver.Resolve<IPathService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public AnimationEmitterReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ICommonDialogService dialogService,
        IFileSystemService fileSystem,
        IPathService pathService,
        IUndoService undoService) : base(dependencies, assetManager, dialogService, fileSystem, pathService, undoService) {
        this.InitializeComponent();
    }
}