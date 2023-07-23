namespace Macabresoft.Macabre2D.UI.Editor;

using System;
using System.Linq;
using System.Threading.Tasks;
using Macabresoft.Macabre2D.Framework;
using Macabresoft.Macabre2D.UI.Common;
using Unity;
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
    /// Opens a dialog that allows the user to pick an <see cref="SpriteSheetMember" />.
    /// </summary>
    /// <returns>A sprite sheet and the packaged asset identifier of the selected <see cref="SpriteSheetMember" />.</returns>
    Task<(SpriteSheet SpriteSheet, Guid PackagedAssetId)> OpenSpriteSheetAssetSelectionDialog<TAsset>() where TAsset : SpriteSheetMember;
}

/// <summary>
/// A dialog service.
/// </summary>
public sealed class LocalDialogService : CommonDialogService, ILocalDialogService {
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
    public async Task<(SpriteSheet SpriteSheet, byte SpriteIndex)> OpenSpriteSelectionDialog() {
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
    public async Task<(SpriteSheet SpriteSheet, Guid PackagedAssetId)> OpenSpriteSheetAssetSelectionDialog<TAsset>() where TAsset : SpriteSheetMember {
        var window = Resolver.Resolve<SpriteSheetAssetSelectionDialog>(
            new ParameterOverride("viewModel",
                Resolver.Resolve<SpriteSheetAssetSelectionViewModel<TAsset>>()));
        
        if (await window.ShowDialog<bool>(this.MainWindow) &&
            window.DataContext is SpriteSheetAssetSelectionViewModel<TAsset> { SelectedAsset: { } asset } viewModel &&
            viewModel.SpriteSheets.Select(x => x.SpriteSheet).FirstOrDefault(x => x.GetAssets<TAsset>().Any(y => y.Id == asset.Id)) is { } contentSpriteSheet &&
            this._assetManager.TryGetMetadata(contentSpriteSheet.ContentId, out var metadata) &&
            metadata?.Asset is SpriteSheet spriteSheet) {
            return (spriteSheet, asset.Id);
        }

        return (null, Guid.Empty);
    }
}