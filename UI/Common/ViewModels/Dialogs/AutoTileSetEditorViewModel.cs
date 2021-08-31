namespace Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs {
    using System.Collections.Generic;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Models.Rendering;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for editing auto tile sets.
    /// </summary>
    public class AutoTileSetEditorViewModel : BaseDialogViewModel {
        private readonly ContentFile _file;
        private readonly IUndoService _parentUndoService;
        private readonly SpriteSheet _spriteSheet;
        private ThumbnailSize _selectedThumbnailSize;
        private byte _selectedTile;

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
            this.SpriteCollection = new SpriteDisplayCollection(spriteSheet, file);

            var tiles = new AutoTileIndexModel[this.TileSet.Size];
            for (byte i = 0; i < tiles.Length; i++) {
                tiles[i] = new AutoTileIndexModel(this.TileSet, i);
            }

            this.Tiles = tiles;
            this.IsOkEnabled = true;
            
            
        }

        /// <summary>
        /// Gets the sprite collection.
        /// </summary>
        public SpriteDisplayCollection SpriteCollection { get; }
        
        /// <summary>
        /// Gets the tiles.
        /// </summary>
        public IReadOnlyCollection<AutoTileIndexModel> Tiles { get; }

        /// <summary>
        /// Gets the tile set.
        /// </summary>
        public AutoTileSet TileSet { get; }

        /// <summary>
        /// Gets the undo service.
        /// </summary>
        public IChildUndoService UndoService { get; }

        /// <summary>
        /// Gets or sets the selected thumbnail size.
        /// </summary>
        public ThumbnailSize SelectedThumbnailSize {
            get => this._selectedThumbnailSize;
            set => this.RaiseAndSetIfChanged(ref this._selectedThumbnailSize, value);
        }

        /// <summary>
        /// Gets or sets the selected tile. This value comes from converting the connected <see cref="CardinalDirections" /> of the
        /// tile to a byte.
        /// </summary>
        public byte SelectedTile {
            get => this._selectedTile;
            set => this.RaiseAndSetIfChanged(ref this._selectedTile, value);
        }

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