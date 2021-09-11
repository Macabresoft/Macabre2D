namespace Macabresoft.Macabre2D.UI.Common.Services {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;

    /// <summary>
    /// An interface for a dialog service.
    /// </summary>
    public interface IDialogService {
        /// <summary>
        /// Opens a dialog that allows the user to pick a <see cref="IContentNode" /> whose asset inherits from the specified base
        /// type.
        /// </summary>
        /// <param name="baseAssetType">The base asset type.</param>
        /// <param name="allowDirectorySelection">
        /// A value indicating whether or not the user can select a directory. Usually used
        /// when creating a new asset as opposed to loading one.
        /// </param>
        /// <returns>The selected content node.</returns>
        Task<IContentNode> OpenAssetSelectionDialog(Type baseAssetType, bool allowDirectorySelection);

        /// <summary>
        /// Opens the auto tile set editor.
        /// </summary>
        /// <param name="tileSet">The tile set to edit.</param>
        /// <param name="spriteSheet">The sprite sheet which owns the tile set.</param>
        /// <param name="file">The content file representing the sprite sheet.</param>
        /// <returns>A value indicating whether or not the user pressed ok.</returns>
        Task<bool> OpenAutoTileSetEditor(AutoTileSet tileSet, SpriteSheet spriteSheet, ContentFile file);

        /// <summary>
        /// Opens a dialog to show the licenses.
        /// </summary>
        /// <returns>A task.</returns>
        Task OpenLicenseDialog();

        /// <summary>
        /// Opens a dialog that allows the user to pick a sprite.
        /// </summary>
        /// <returns>A sprite sheet and the sprite index on the sprite sheet.</returns>
        Task<(SpriteSheet SpriteSheet, byte SpriteIndex)> OpenSpriteSelectionDialog();
        
        /// <summary>
        /// Opens a dialog that allows the user to pick an <see cref="AutoTileSet"/>.
        /// </summary>
        /// <returns>A sprite sheet and the packaged asset identifier of the selected <see cref="AutoTileSet"/>.</returns>
        Task<(SpriteSheet SpriteSheet, Guid PackagedAssetId)> OpenAutoTileSetSelectionDialog();

        /// <summary>
        /// Opens a dialog that allows the user to pick a <see cref="Type" />.
        /// </summary>
        /// <param name="types">The types to select from.</param>
        /// <returns>The selected type.</returns>
        Task<Type> OpenTypeSelectionDialog(IEnumerable<Type> types);

        /// <summary>
        /// Shows a warning dialog.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <returns>A task.</returns>
        Task ShowWarningDialog(string title, string message);

        /// <summary>
        /// Shows a dialog with a yes and a no button.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message, usually a yes or no question.</param>
        /// <param name="allowCancel">A value indicating whether or not to allow cancellation.</param>
        /// <returns><c>true</c> if the user selected yes; otherwise <c>false</c>.</returns>
        Task<YesNoCancelResult> ShowYesNoDialog(string title, string message, bool allowCancel);
    }
}