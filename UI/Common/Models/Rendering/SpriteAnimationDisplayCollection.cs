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
    /// A collection of <see cref="SpriteAnimation" /> for display.
    /// </summary>
    public class SpriteAnimationDisplayCollection : NotifyPropertyChanged, IReadOnlyCollection<SpriteAnimation> {
        private readonly ContentFile _file;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAnimationDisplayCollection" /> class.
        /// </summary>
        /// <param name="spriteSheet">The sprite sheet.</param>
        /// <param name="file">The file.</param>
        public SpriteAnimationDisplayCollection(SpriteSheet spriteSheet, ContentFile file) {
            this.SpriteSheet = spriteSheet;
            this._file = file;

            var fileInfo = new FileInfo(file.GetFullPath());
            if (fileInfo.Exists) {
                this.Image = new Bitmap(fileInfo.FullName);
            }
        }

        /// <inheritdoc />
        public int Count => this.SpriteSheet.SpriteAnimations.Count;

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
        /// Gets the sprite animation.
        /// </summary>
        public IReadOnlyCollection<SpriteAnimation> SpriteAnimations => this.SpriteSheet.SpriteAnimations;

        /// <summary>
        /// Gets the sprite sheet.
        /// </summary>
        public SpriteSheet SpriteSheet { get; }

        /// <inheritdoc />
        public IEnumerator<SpriteAnimation> GetEnumerator() {
            return this.SpriteSheet.SpriteAnimations.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}