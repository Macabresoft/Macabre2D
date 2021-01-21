namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Represents a sprite that can be loaded as content.
    /// </summary>
    public sealed class Sprite : NotifyPropertyChanged, IPackagedAsset<SpriteSheet>, IDisposable {
        private string _name = string.Empty;
        private SpriteSheet? _spriteSheet;
        private byte _row;
        private byte _column;
        private byte _width = 1;
        private byte _height = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite" /> class.
        /// </summary>
        /// <param name="assetId">The asset identifier.</param>
        public Sprite(Guid assetId) {
            this.AssetId = assetId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite" /> class.
        /// </summary>
        /// <param name="assetId">The identifier.</param>
        /// <param name="location">
        /// The location of this specific sprite on the <see cref="Texture2D" />.
        /// </param>
        /// <param name="size">The size of this specific sprite on the <see cref="Texture2D" />.</param>
        public Sprite(Guid assetId, Point location, Point size) {
            this.AssetId = assetId;
            this.Location = location;
            this.Size = size;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sprite" /> class.
        /// </summary>
        private Sprite() {
        }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        /// <value>The texture.</value>
        public Texture2D? Texture => this._spriteSheet?.Content;

        /// <inheritdoc />
        [DataMember]
        public Guid AssetId { get; set; } = Guid.Empty;

        /// <summary>
        /// Gets or sets the row on the <see cref="SpriteSheet"/> that this sprite starts on from the top of the sheet.
        /// </summary>
        [DataMember]
        public byte Row {
            get => this._row;
            set {
                if (this.Set(ref this._row, value)) {
                    this.InvalidateSprite();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the column on the <see cref="SpriteSheet"/> that this sprite starts on from the left side of the sheet.
        /// </summary>
        [DataMember]
        public byte Column {
            get => this._column;
            set {
                if (this.Set(ref this._column, value)) {
                    this.InvalidateSprite();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the number of tiles wide this sprite is on the <see cref="SpriteSheet"/>.
        /// </summary>
        [DataMember]
        public byte Width {
            get => this._width;
            set {
                if (this.Set(ref this._width, value)) {
                    this.InvalidateSprite();
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the number of tiles tall this sprite is on the <see cref="SpriteSheet"/>.
        /// </summary>
        [DataMember]
        public byte Height {
            get => this._height;
            set {
                if (this.Set(ref this._height, value)) {
                    this.InvalidateSprite();
                }
            }
        }

        /// <summary>
        /// Gets the location of this sprite in pixels on the <see cref="Texture2D"/>.
        /// </summary>
        /// <value>The location.</value>
        public Point Location { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name {
            get => this._name;
            set => this.Set(ref this._name, value);
        }

        /// <summary>
        /// Gets the size of this sprite in pixels on the <see cref="Texture2D"/>.
        /// </summary>
        /// <value>The size.</value>
        public Point Size { get; private set; }

        /// <inheritdoc />
        public void Initialize(SpriteSheet owningPackage) {
            if (this._spriteSheet != null) {
                this._spriteSheet.PropertyChanged -= this.SpriteSheet_PropertyChanged;
            }
            
            this._spriteSheet = owningPackage;
            this.InvalidateSprite();
            this._spriteSheet.PropertyChanged += this.SpriteSheet_PropertyChanged;
        }

        private void SpriteSheet_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(SpriteSheet.Content) || e.PropertyName == nameof(SpriteSheet.Columns) || e.PropertyName == nameof(SpriteSheet.Rows)) {
                this.InvalidateSprite();
            }
        }

        /// <inheritdoc />
        public void Dispose() {
            if (this._spriteSheet != null) {
                this._spriteSheet.PropertyChanged -= this.SpriteSheet_PropertyChanged;
            }
        }

        private void InvalidateSprite() {
            if (this._spriteSheet != null) {
                var (location, size) = this._spriteSheet.GetLocationAndSize(this.Column, this.Row, this.Width, this.Height);
                this.Location = location;
                this.Size = size;
                this.RaisePropertyChanged(nameof(this.Location));
            }
        }
    }
}