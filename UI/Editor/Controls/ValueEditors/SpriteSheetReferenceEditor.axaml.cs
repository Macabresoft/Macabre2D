namespace Macabresoft.Macabre2D.UI.Editor;

using Avalonia.Markup.Xaml;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Unity;

public class SpriteSheetReferenceEditor : BaseSpriteSheetReferenceEditor<SpriteSheetReference> {
    public SpriteSheetReferenceEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ILocalDialogService>(),
        Resolver.Resolve<IFileSystemService>(),
        Resolver.Resolve<IPathService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public SpriteSheetReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ILocalDialogService dialogService,
        IFileSystemService fileSystem,
        IPathService pathService,
        IUndoService undoService) : base(dependencies, assetManager, dialogService, fileSystem, pathService, undoService) {
        this.InitializeComponent();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }
}