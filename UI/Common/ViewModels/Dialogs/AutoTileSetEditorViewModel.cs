﻿namespace Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs {
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Services;

    /// <summary>
    /// A view model for editing auto tile sets.
    /// </summary>
    public class AutoTileSetEditorViewModel : BaseDialogViewModel {
        private readonly IUndoService _parentUndoService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTileSetEditorViewModel" /> class.
        /// </summary>
        /// <param name="childUndoService">The child undo service.</param>
        /// <param name="parentUndoService">The parent undo service.</param>
        /// <param name="tileSet">The tile set being edited.</param>
        public AutoTileSetEditorViewModel(IChildUndoService childUndoService, IUndoService parentUndoService, AutoTileSet tileSet) {
            this.UndoService = childUndoService;
            this._parentUndoService = parentUndoService;
            this.TileSet = tileSet;
            this.IsOkEnabled = true;
        }

        /// <summary>
        /// Gets the tile set.
        /// </summary>
        public AutoTileSet TileSet { get; }

        /// <summary>
        /// Gets the undo service.
        /// </summary>
        public IChildUndoService UndoService { get; }

        /// <inheritdoc />
        protected override void OnCancel() {
            var command = this.UndoService.GetChanges();
            command?.Undo();
            base.OnCancel();
        }

        /// <inheritdoc />
        protected override void OnOk() {
            var command = this.UndoService.GetChanges();
            this._parentUndoService.CommitExternalChanges(command);
            base.OnOk();
        }
    }
}