namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Linq;
using System.Threading.Tasks;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Unity.Resolution;

/// <summary>
/// An interface for a dialog service featuring dialogs exclusive to this project.
/// </summary>
public interface ILocalDialogService : ICommonDialogService {
    /// <summary>
    /// Opens a dialog that allows the user to pick a sprite.
    /// </summary>
    /// <returns>A sprite sheet and the sprite index on the sprite sheet.</returns>
    Task<(SpriteSheet SpriteSheet, byte SpriteIndex)> OpenSpriteSelectionDialog();

    /// <summary>
    /// Opens a dialog that allows the user to pick an <see cref="SpriteSheetAsset" />.
    /// </summary>
    /// <returns>A sprite sheet and the packaged asset identifier of the selected <see cref="SpriteSheetAsset" />.</returns>
    Task<(SpriteSheet SpriteSheet, Guid PackagedAssetId)> OpenSpriteSheetAssetSelectionDialog<TAsset>() where TAsset : SpriteSheetAsset;
}

/// <summary>
/// A dialog service.
/// </summary>
public sealed class LocalDialogService : CommonDialogService, ILocalDialogService {
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalDialogService" /> class.
    /// </summary>
    /// <param name="mainWindow">The main window.</param>
    public LocalDialogService(MainWindow mainWindow) : base(mainWindow) {
    }

    /// <inheritdoc />
    public async Task<(SpriteSheet SpriteSheet, byte SpriteIndex)> OpenSpriteSelectionDialog() {
        var window = Resolver.Resolve<SpriteSelectionDialog>();
        if (await window.ShowDialog<bool>(this.MainWindow) && window.ViewModel.SelectedSprite is SpriteDisplayModel sprite) {
            return (sprite.SpriteSheet, sprite.Index);
        }

        return (null, 0);
    }

    /// <inheritdoc />
    public async Task<(SpriteSheet SpriteSheet, Guid PackagedAssetId)> OpenSpriteSheetAssetSelectionDialog<TAsset>() where TAsset : SpriteSheetAsset {
        var window = Resolver.Resolve<SpriteSheetAssetSelectionDialog>(
            new ParameterOverride("viewModel",
                Resolver.Resolve<SpriteSheetAssetSelectionViewModel<TAsset>>()));

        if (await window.ShowDialog<bool>(this.MainWindow) &&
            window.DataContext is SpriteSheetAssetSelectionViewModel<TAsset> { SelectedAsset: TAsset tileSet } viewModel &&
            viewModel.SpriteSheets.Select(x => x.SpriteSheet).FirstOrDefault(x => x.GetAssets<TAsset>().Any(y => y.Id == tileSet.Id)) is SpriteSheet spriteSheet) {
            return (spriteSheet, tileSet.Id);
        }

        return (null, Guid.Empty);
    }
}