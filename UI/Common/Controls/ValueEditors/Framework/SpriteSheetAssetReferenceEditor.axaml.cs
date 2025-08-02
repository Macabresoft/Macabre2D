namespace Macabresoft.Macabre2D.UI.Common;

using System.Threading.Tasks;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Unity;

public partial class SpriteSheetAssetReferenceEditor : BaseSpriteSheetReferenceEditor<ISpriteSheetAssetReference> {
    public SpriteSheetAssetReferenceEditor() : this(
        null,
        Resolver.Resolve<IAssetManager>(),
        Resolver.Resolve<ICommonDialogService>(),
        Resolver.Resolve<IFileSystemService>(),
        Resolver.Resolve<IPathService>(),
        Resolver.Resolve<IUndoService>()) {
    }

    [InjectionConstructor]
    public SpriteSheetAssetReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ICommonDialogService dialogService,
        IFileSystemService fileSystem,
        IPathService pathService,
        IUndoService undoService) : base(dependencies, assetManager, dialogService, fileSystem, pathService, undoService) {
        this.InitializeComponent();
    }

    protected override void Clear() {
        var asset = this.Value.Asset;
        var packagedAssetId = this.Value.PackagedAssetId;

        if (asset != null) {
            this.UndoService.Do(
                () => this.Value.Clear(),
                () =>
                {
                    this.Value.LoadAsset(asset);
                    this.Value.PackagedAssetId = packagedAssetId;
                });
        }
    }

    protected override async Task Select() {
        var (spriteSheet, packagedAssetId) = await this.DialogService.OpenSpriteSheetAssetSelectionDialog(this.Value.PackagedAssetType, this.Title);
        if (spriteSheet != null) {
            var originalAsset = this.Value.Asset;
            var originalPackagedAssetId = this.Value.PackagedAssetId;
            this.UndoService.Do(
                () =>
                {
                    this.Value.LoadAsset(spriteSheet);
                    this.Value.PackagedAssetId = packagedAssetId;
                    this.ResetPath();
                },
                () =>
                {
                    if (originalAsset != null) {
                        this.Value.PackagedAssetId = originalPackagedAssetId;
                        this.Value.LoadAsset(originalAsset);
                        this.ResetPath();
                    }
                    else {
                        this.Value.Clear();
                    }
                });
        }
    }
}