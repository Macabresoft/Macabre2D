namespace Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs {
    using Avalonia.Media.Imaging;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using Unity;

    /// <summary>
    /// A view model for editing auto tile sets.
    /// </summary>
    public class AutoTileSetEditorViewModel : BaseDialogViewModel {
        private readonly IUndoService _parentUndoService;
        private readonly SpriteSheet _spriteSheet;
        private readonly ContentFile _file;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTileSetEditorViewModel" /> class.
        /// </summary>
        public AutoTileSetEditorViewModel() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTileSetEditorViewModel" /> class.
        /// </summary>
        /// <param name="childUndoService">The child undo service.</param>
        /// <param name="parentUndoService">The parent undo service.</param>
        /// <param name="tileSet">The tile set being edited.</param>
        /// <param name="spriteSheet">The sprite sheet.</param>
        /// <param name="file">The content file.</param>
        [InjectionConstructor]
        public AutoTileSetEditorViewModel(
            IChildUndoService childUndoService, 
            IUndoService parentUndoService,
            AutoTileSet tileSet,
            SpriteSheet spriteSheet,
            ContentFile file) {
            this.UndoService = childUndoService;
            this._parentUndoService = parentUndoService;
            this.TileSet = tileSet;
            this._spriteSheet = spriteSheet;
            this._file = file;
            this.Bitmap = new Bitmap(this._file.GetFullPath());
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
        
        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        public Bitmap Bitmap { get; }

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