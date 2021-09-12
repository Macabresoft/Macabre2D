namespace Macabresoft.Macabre2D.UI.Common.Models.Rendering {
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using Avalonia;
    using Avalonia.Media;
    using Avalonia.Media.Imaging;
    using Macabresoft.Macabre2D.Framework;
    using Macabresoft.Macabre2D.UI.Common.Models.Content;

    /// <summary>
    /// A collection of <see cref="AutoTileSet" /> for display.
    /// </summary>
    public class AutoTileSetDisplayCollection : NotifyPropertyChanged, IReadOnlyCollection<AutoTileSet> {
        private readonly ContentFile _file;
        private readonly SpriteSheet _spriteSheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTileSetDisplayCollection" /> class.
        /// </summary>
        /// <param name="spriteSheet">The sprite sheet.</param>
        /// <param name="file">The file.</param>
        public AutoTileSetDisplayCollection(SpriteSheet spriteSheet, ContentFile file) {
            this._spriteSheet = spriteSheet;
            this._file = file;

            var fileInfo = new FileInfo(file.GetFullPath());
            if (fileInfo.Exists) {
                this.Image = new Bitmap(fileInfo.FullName);
            }
        }

        /// <inheritdoc />
        public int Count => this._spriteSheet.AutoTileSets.Count;

        /// <summary>
        /// Gets the image.
        /// </summary>
        public IImage Image { get; }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string Name => this._file.Name;

        /// <summary>
        /// Gets the size of the sprite sheet.
        /// </summary>
        public Size Size => this.Image.Size;

        /// <summary>
        /// Gets the tile sets.
        /// </summary>
        public IReadOnlyCollection<AutoTileSet> TileSets => this._spriteSheet.AutoTileSets;

        /// <inheritdoc />
        public IEnumerator<AutoTileSet> GetEnumerator() {
            return this._spriteSheet.AutoTileSets.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}