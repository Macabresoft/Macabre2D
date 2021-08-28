namespace Macabresoft.Macabre2D.UI.ProjectEditor.Services {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Macabresoft.Macabre2D.UI.ProjectEditor.Views;
    using Macabresoft.Macabre2D.UI.ProjectEditor.Views.Dialogs;
    using Unity.Resolution;

    /// <summary>
    /// A dialog service.
    /// </summary>
    public class DialogService : IDialogService {
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService" /> class.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        public DialogService(MainWindow mainWindow) {
            this._mainWindow = mainWindow;
        }

        /// <inheritdoc />
        public async Task<IContentNode> OpenAssetSelectionDialog(Type baseAssetType, bool allowDirectorySelection) {
            IContentNode selectedNode = null;
            var window = Resolver.Resolve<ContentSelectionDialog>(new ParameterOverride(typeof(Type), baseAssetType), new ParameterOverride(typeof(bool), allowDirectorySelection));
            var result = await window.ShowDialog<bool>(this._mainWindow);

            if (result && window.ViewModel != null) {
                selectedNode = window.ViewModel.SelectedContentNode?.Node;
            }

            return selectedNode;
        }

        /// <inheritdoc />
        public async Task<bool> OpenAutoTileSetEditor(AutoTileSet tileSet, SpriteSheet spriteSheet, ContentFile file) {
            var window = Resolver.Resolve<AutoTileSetEditorDialog>(
                new ParameterOverride(typeof(AutoTileSet), tileSet),
                new ParameterOverride(typeof(SpriteSheet), spriteSheet),
                new ParameterOverride(typeof(ContentFile), file));
            return await window.ShowDialog<bool>(this._mainWindow);
        }

        /// <inheritdoc />
        public async Task OpenLicenseDialog() {
            var window = Resolver.Resolve<LicenseDialog>();
            await window.ShowDialog(this._mainWindow);
        }

        /// <inheritdoc />
        public async Task<(SpriteSheet SpriteSheet, byte SpriteIndex)> OpenSpriteSelectionDialog() {
            var window = Resolver.Resolve<SpriteSelectionDialog>();
            if (await window.ShowDialog<bool>(this._mainWindow) && window.ViewModel.SelectedSprite is SpriteDisplayModel sprite) {
                return (sprite.SpriteSheet, sprite.Index);
            }

            return (null, 0);
        }

        /// <inheritdoc />
        public async Task<Type> OpenTypeSelectionDialog(IEnumerable<Type> types) {
            Type selectedType = null;
            var window = Resolver.Resolve<TypeSelectionDialog>(new ParameterOverride(typeof(IEnumerable<Type>), types));
            var result = await window.ShowDialog<bool>(this._mainWindow);

            if (result && window.ViewModel != null) {
                selectedType = window.ViewModel.SelectedType;
            }

            return selectedType;
        }

        /// <inheritdoc />
        public async Task ShowWarningDialog(string title, string message) {
            var window = Resolver.Resolve<WarningDialog>();
            window.Title = title;
            window.WarningMessage = message;
            await window.ShowDialog(this._mainWindow);
        }

        /// <inheritdoc />
        public async Task<YesNoCancelResult> ShowYesNoDialog(string title, string message, bool allowCancel) {
            var window = Resolver.Resolve<YesNoCancelDialog>();
            window.Title = title;
            window.Question = message;

            var result = await window.ShowDialog<YesNoCancelResult>(this._mainWindow);
            return result;
        }
    }
}