namespace Macabresoft.Macabre2D.UI.Editor;

using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Unity;

public partial class SpriteSheetFontReferenceEditor : BaseSpriteSheetAssetReferenceEditor<SpriteSheetFontReference, SpriteSheetFont> {
    public SpriteSheetFontReferenceEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IFileSystemService>(),
        Resolver.Resolve<IPathService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public SpriteSheetFontReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ICommonDialogService dialogService,
        IFileSystemService fileSystem,
        IPathService pathService,
        IUndoService undoService) : base(dependencies, assetManager, dialogService, fileSystem, pathService, undoService) {
        this.InitializeComponent();
    }
}