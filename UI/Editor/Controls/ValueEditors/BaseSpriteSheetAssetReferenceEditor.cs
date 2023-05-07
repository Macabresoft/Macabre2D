namespace Macabresoft.Macabre2D.UI.Editor;

using System.Threading.Tasks;
using Macabresoft.AvaloniaEx;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;

public class BaseSpriteSheetAssetReferenceEditor<TAssetReference, TPackagedAsset> : BaseSpriteSheetReferenceEditor<TAssetReference>
    where TAssetReference : Framework.SpriteSheetAssetReference<TPackagedAsset>, IAssetReference<SpriteSheet>
    where TPackagedAsset : SpriteSheetMember {
    public BaseSpriteSheetAssetReferenceEditor(
        ValueControlDependencies dependencies,
        IAssetManager assetManager,
        ILocalDialogService dialogService,
        IFileSystemService fileSystem,
        IPathService pathService,
        IUndoService undoService) : base(dependencies, assetManager, dialogService, fileSystem, pathService, undoService) {
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
        var (spriteSheet, packagedAssetId) = await this.DialogService.OpenSpriteSheetAssetSelectionDialog<TPackagedAsset>();
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