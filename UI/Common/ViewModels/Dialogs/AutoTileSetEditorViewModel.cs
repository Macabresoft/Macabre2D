namespace Macabresoft.Macabre2D.UI.Common.ViewModels.Dialogs {
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Avalonia;
    using Avalonia.Threading;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;
    using Macabresoft.Macabre2D.UI.Common.Models.Rendering;
    using Macabresoft.Macabre2D.UI.Common.Services;
    using ReactiveUI;
    using Unity;

    /// <summary>
    /// A view model for editing auto tile sets.
    /// </summary>
    public class AutoTileSetEditorViewModel : BaseDialogViewModel {
        private const double MaxTileSize = 128;
        private readonly IChildUndoService _childUndoService;
        private readonly IUndoService _parentUndoService;
        private SpriteDisplayModel _selectedSprite;
        private ThumbnailSize _selectedThumbnailSize;
        private AutoTileIndexModel _selectedTile;

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
            ContentFile file) : base(childUndoService) {
            this._childUndoService = childUndoService;
            this._parentUndoService = parentUndoService;
            this.ClearSpriteCommand = ReactiveCommand.Create(
                this.ClearSprite,
                this.WhenAny(x => x.SelectedTile, x => x.Value != null));
            this.SelectTileCommand = ReactiveCommand.Create<AutoTileIndexModel>(this.SelectTile);
            this.SpriteCollection = new SpriteDisplayCollection(spriteSheet, file);

            var tiles = new AutoTileIndexModel[tileSet.Size];
            for (byte i = 0; i < tiles.Length; i++) {
                tiles[i] = new AutoTileIndexModel(tileSet, i);
            }

            this.Tiles = tiles;
            this.SelectedTile = this.Tiles.First();
            this.TileSize = this.GetTileSize();
            this.IsOkEnabled = true;
        }

        /// <summary>
        /// Clears the selected sprite from the selected tile.
        /// </summary>
        public ICommand ClearSpriteCommand { get; }

        /// <summary>
        /// Gets the select tile command.
        /// </summary>
        public ICommand SelectTileCommand { get; }

        /// <summary>
        /// Gets the sprite collection.
        /// </summary>
        public SpriteDisplayCollection SpriteCollection { get; }

        /// <summary>
        /// Gets the tiles.
        /// </summary>
        public IReadOnlyCollection<AutoTileIndexModel> Tiles { get; }

        /// <summary>
        /// Gets the size of a tile in pixels.
        /// </summary>
        public Size TileSize { get; }

        /// <summary>
        /// Gets or sets the selected sprite.
        /// </summary>
        public SpriteDisplayModel SelectedSprite {
            get => this._selectedSprite;
            set {
                if (this._selectedTile is AutoTileIndexModel selectedTile) {
                    var previousSprite = this._selectedSprite;
                    this.UndoService.Do(() => {
                        this.RaiseAndSetIfChanged(ref this._selectedSprite, value);
                        selectedTile.SpriteIndex = this._selectedSprite?.Index;
                    }, () => {
                        this.RaiseAndSetIfChanged(ref this._selectedSprite, previousSprite);
                        selectedTile.SpriteIndex = this._selectedSprite?.Index;
                    });
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected thumbnail size.
        /// </summary>
        public ThumbnailSize SelectedThumbnailSize {
            get => this._selectedThumbnailSize;
            set => this.RaiseAndSetIfChanged(ref this._selectedThumbnailSize, value);
        }

        /// <summary>
        /// Gets or sets the selected tile.
        /// </summary>
        public AutoTileIndexModel SelectedTile {
            get => this._selectedTile;
            private set {
                if (this._selectedTile != value) {
                    this.RaiseAndSetIfChanged(ref this._selectedTile, value);
                    this._selectedSprite = this._selectedTile != null ? this.SpriteCollection.FirstOrDefault(x => x.Index == this._selectedTile.SpriteIndex) : null;
                    this.RaisePropertyChanged(nameof(this.SelectedSprite));
                }
            }
        }

        /// <inheritdoc />
        protected override void OnCancel() {
            var command = this._childUndoService.GetChanges();
            command?.Undo();
            base.OnCancel();
        }

        /// <inheritdoc />
        protected override void OnOk() {
            var command = this._childUndoService.GetChanges();
            this._parentUndoService.CommitExternalChanges(command);
            base.OnOk();
        }

        private void ClearSprite() {
            if (this.SelectedTile != null && this.SelectedTile.SpriteIndex != null) {
                this.SelectedSprite = null;
            }
        }

        private Size GetTileSize() {
            var size = Size.Empty;
            if (this.SpriteCollection.Sprites.FirstOrDefault() is SpriteDisplayModel { Size: { Width: > 0, Height: > 0 } spriteSize }) {
                if (spriteSize.Width == spriteSize.Height) {
                    size = new Size(MaxTileSize, MaxTileSize);
                }
                else if (spriteSize.Width > spriteSize.Height) {
                    var ratio = spriteSize.Height / (double)spriteSize.Width;
                    size = new Size(MaxTileSize, MaxTileSize * ratio);
                }
                else {
                    var ratio = spriteSize.Width / (double)spriteSize.Height;
                    size = new Size(MaxTileSize * ratio, MaxTileSize);
                }
            }

            return size;
        }

        private void SelectTile(AutoTileIndexModel tile) {
            if (tile != null) {
                this.SelectedTile = tile;
                Dispatcher.UIThread.Post(() => this.RaisePropertyChanged(nameof(this.SelectedTile)), DispatcherPriority.ApplicationIdle);
            }
        }
    }
}