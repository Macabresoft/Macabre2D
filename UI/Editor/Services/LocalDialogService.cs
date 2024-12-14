namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Linq;
using System.Threading.Tasks;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Unity;
using Unity.Resolution;

/// <summary>
/// A dialog service.
/// </summary>
public sealed class LocalDialogService : CommonDialogService {
    private readonly IAssetManager _assetManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalDialogService" /> class.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="assetManager">The asset manager.</param>
    /// <param name="mainWindow">The main window.</param>
    public LocalDialogService(IUnityContainer container, IAssetManager assetManager, MainWindow mainWindow) : base(container, mainWindow) {
        this._assetManager = assetManager;
    }

    /// <inheritdoc />
    public override async Task<(SpriteSheet SpriteSheet, byte SpriteIndex)> OpenSpriteSelectionDialog() {
        var window = Resolver.Resolve<SpriteSelectionDialog>();
        if (await window.ShowDialog<bool>(this.MainWindow) &&
            window.ViewModel.SelectedSprite is { SpriteSheet: { } contentSpriteSheet } sprite &&
            this._assetManager.TryGetMetadata(contentSpriteSheet.ContentId, out var metadata) &&
            metadata?.Asset is SpriteSheet spriteSheet) {
            return (spriteSheet, sprite.Index);
        }

        return (null, 0);
    }

    /// <inheritdoc />
    public override async Task<(SpriteSheet SpriteSheet, Guid PackagedAssetId)> OpenSpriteSheetAssetSelectionDialog<TAsset>() => await this.OpenSpriteSheetAssetSelectionDialog(typeof(TAsset));

    /// <inheritdoc />
    public override async Task<(SpriteSheet SpriteSheet, Guid PackagedAssetId)> OpenSpriteSheetAssetSelectionDialog(Type assetType) {
        var window = Resolver.Resolve<SpriteSheetAssetSelectionDialog>(new ParameterOverride("packagedAssetType", assetType));

        if (await window.ShowDialog<bool>(this.MainWindow) &&
            window.DataContext is SpriteSheetAssetSelectionViewModel { SelectedAsset: { } asset } viewModel &&
            viewModel.SpriteSheets.Select(x => x.SpriteSheet).FirstOrDefault(x => x.GetAssets(assetType).Any(y => y.Id == asset.Id)) is { } contentSpriteSheet &&
            this._assetManager.TryGetMetadata(contentSpriteSheet.ContentId, out var metadata) &&
            metadata?.Asset is SpriteSheet spriteSheet) {
            return (spriteSheet, asset.Id);
        }

        return (null, Guid.Empty);
    }
}