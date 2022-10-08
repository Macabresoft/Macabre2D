namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A sprite sheet tied to a single <see cref="Texture2D" /> which also defines sprites, animations, and tile sets.
/// </summary>
[DataContract(Name = "Sprite Sheet")]
[Category("Sprite Sheet")]
public class SpriteSheetAsset : AssetPackage<Texture2D> {
    /// <summary>
    /// The valid file extensions for a <see cref="Texture2D" />.
    /// </summary>
    public static readonly string[] ValidFileExtensions = { ".jpg", ".png" };

    private readonly Dictionary<byte, Point> _spriteIndexToLocation = new();

    [DataMember]
    [Category("Auto Tile Sets")]
    private AutoTileSetCollection _autoTileSets = new();

    private byte _columns = 1;
    private byte _rows = 1;

    [DataMember]
    [Category("Animations")]
    private SpriteAnimationCollection _spriteAnimations = new();

    private Point _spriteSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheetAsset" /> class.
    /// </summary>
    public SpriteSheetAsset() {
        this._autoTileSets.CollectionChanged += this.SpriteSheetAsset_CollectionChanged;
        this._autoTileSets.PropertyChanged += this.RaisePropertyChanged;
        this._spriteAnimations.CollectionChanged += this.SpriteSheetAsset_CollectionChanged;
        this._spriteAnimations.PropertyChanged += this.RaisePropertyChanged;
    }

    /// <summary>
    /// Gets the auto tile sets.
    /// </summary>
    public INameableCollection AutoTileSets => this._autoTileSets;

    /// <summary>
    /// Gets or sets the number of columns in this sprite sheet.
    /// </summary>
    [DataMember]
    public byte Columns {
        get => this._columns;
        set {
            if (value == 0) {
                value = 1;
            }

            if (this.Set(ref this._columns, value)) {
                this._spriteIndexToLocation.Clear();
                this.ResetColumnWidth();
            }
        }
    }

    /// <inheritdoc />
    public override bool IncludeFileExtensionInContentPath => false;

    /// <summary>
    /// Gets the max index.
    /// </summary>
    public byte MaxIndex => (byte)(this.Rows * this.Columns - 1);

    /// <summary>
    /// Gets or sets the number of rows in this sprite sheet.
    /// </summary>
    [DataMember]
    public byte Rows {
        get => this._rows;
        set {
            if (value == 0) {
                value = 1;
            }

            if (this.Set(ref this._rows, value)) {
                this._spriteIndexToLocation.Clear();
                this.ResetRowHeight();
            }
        }
    }

    /// <summary>
    /// Gets the sprite animations.
    /// </summary>
    public INameableCollection SpriteAnimations => this._spriteAnimations;

    /// <summary>
    /// Gets the size of a single sprite on this sprite sheet.
    /// </summary>
    public Point SpriteSize {
        get => this._spriteSize;
        private set => this.Set(ref this._spriteSize, value);
    }

    /// <summary>
    /// Draws the specified sprite.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="pixelsPerUnit">The pixels per unit.</param>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="position">The position.</param>
    /// <param name="scale">The scale.</param>
    /// <param name="rotation">The rotation.</param>
    /// <param name="color">The color.</param>
    /// <param name="orientation">The orientation.</param>
    public void Draw(
        SpriteBatch spriteBatch,
        ushort pixelsPerUnit,
        byte spriteIndex,
        Vector2 position,
        Vector2 scale,
        float rotation,
        Color color,
        SpriteEffects orientation) {
        if (this.Content != null && this.SpriteSize != Point.Zero) {
            spriteBatch.Draw(
                this.Content,
                position * pixelsPerUnit,
                new Rectangle(this.GetSpriteLocation(spriteIndex), this.SpriteSize),
                color,
                rotation,
                Vector2.Zero,
                scale,
                orientation,
                0f);
        }
    }

    /// <summary>
    /// Draws the specified sprite.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="pixelsPerUnit">The pixels per unit.</param>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="transform">The transform.</param>
    /// <param name="rotation">The rotation.</param>
    /// <param name="color">The color.</param>
    /// <param name="orientation">The orientation.</param>
    public void Draw(
        SpriteBatch spriteBatch,
        ushort pixelsPerUnit,
        byte spriteIndex,
        Transform transform,
        float rotation,
        Color color,
        SpriteEffects orientation) {
        this.Draw(spriteBatch, pixelsPerUnit, spriteIndex, transform.Position, transform.Scale, rotation, color, orientation);
    }

    /// <summary>
    /// Draws the specified sprite.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="pixelsPerUnit">The pixels per unit.</param>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="transform">The transform.</param>
    /// <param name="color">The color.</param>
    /// <param name="orientation">The orientation.</param>
    public void Draw(
        SpriteBatch spriteBatch,
        ushort pixelsPerUnit,
        byte spriteIndex,
        Transform transform,
        Color color,
        SpriteEffects orientation) {
        this.Draw(spriteBatch, pixelsPerUnit, spriteIndex, transform, transform.Rotation, color, orientation);
    }

    /// <summary>
    /// Gets the <see cref="SpriteSheetMember" /> collection of the specified type.
    /// </summary>
    /// <typeparam name="TAsset">The type of asset.</typeparam>
    /// <returns>A collection of <see cref="SpriteSheetMember" />.</returns>
    public IReadOnlyCollection<TAsset> GetAssets<TAsset>() where TAsset : SpriteSheetMember {
        IReadOnlyCollection<TAsset> result;
        if (typeof(TAsset) == typeof(SpriteAnimation)) {
            result = (IReadOnlyCollection<TAsset>)this._spriteAnimations;
        }
        else if (typeof(TAsset) == typeof(AutoTileSet)) {
            result = (IReadOnlyCollection<TAsset>)this._autoTileSets;
        }
        else {
            result = new List<TAsset>();
        }

        return result;
    }

    /// <inheritdoc />
    public override string GetContentBuildCommands(string contentPath, string fileExtension) {
        // TODO: allow customization of compilation parameters.
        var contentStringBuilder = new StringBuilder();
        contentStringBuilder.AppendLine($"#begin {contentPath}");
        contentStringBuilder.AppendLine($@"/importer:{nameof(TextureImporter)}");
        contentStringBuilder.AppendLine($@"/processor:{nameof(TextureProcessor)}");
        contentStringBuilder.AppendLine(@"/processorParam:ColorKeyColor=255,0,255,255");
        contentStringBuilder.AppendLine(@"/processorParam:ColorKeyEnabled=True");
        contentStringBuilder.AppendLine(@"/processorParam:GenerateMipmaps=False");
        contentStringBuilder.AppendLine(@"/processorParam:PremultiplyAlpha=True");
        contentStringBuilder.AppendLine(@"/processorParam:ResizeToPowerOfTwo=False");
        contentStringBuilder.AppendLine(@"/processorParam:MakeSquare=False");
        contentStringBuilder.AppendLine(@"/processorParam:TextureFormat=Color");
        contentStringBuilder.AppendLine($@"/build:{contentPath}{fileExtension}");
        contentStringBuilder.AppendLine($"#end {contentPath}");
        return contentStringBuilder.ToString();
    }

    /// <summary>
    /// Gets the sprite index given a column and row. Normalized to exist on the sprite sheet.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <param name="row">The row.</param>
    /// <returns>The normalized sprite index.</returns>
    public byte GetSpriteIndex(byte column, byte row) {
        while (column > this.Columns) {
            column -= this.Columns;
        }

        while (row > this.Rows) {
            row -= this.Rows;
        }


        return (byte)(row * this.Columns + column);
    }

    /// <summary>
    /// Gets the sprite location based on its index
    /// </summary>
    /// <param name="spriteIndex">The index of the sprite. Counts from left to right, top to bottom.</param>
    /// <returns>The sprite location.</returns>
    public Point GetSpriteLocation(byte spriteIndex) {
        if (!this._spriteIndexToLocation.TryGetValue(spriteIndex, out var location) && this._columns > 0) {
            var row = Math.DivRem(spriteIndex, this._columns, out var column);
            location = new Point(column * this.SpriteSize.X, row * this.SpriteSize.Y);
            this._spriteIndexToLocation[spriteIndex] = location;
        }

        return location;
    }

    /// <summary>
    /// Gets the sprite size.
    /// </summary>
    /// <param name="imageSize">The image size in pixels.</param>
    /// <param name="columns">The number of columns in the sprite sheet.</param>
    /// <param name="rows">The number of rows in the sprite sheet.</param>
    /// <returns>The sprite size.</returns>
    public static Point GetSpriteSize(Point imageSize, byte columns, byte rows) {
        return new Point(
            GetColumnWidth(imageSize.X, columns),
            GetRowHeight(imageSize.Y, rows));
    }

    /// <summary>
    /// Gets the sprite size and location.
    /// </summary>
    /// <param name="imageSize">The image size in pixels.</param>
    /// <param name="columns">The number of columns in the sprite sheet.</param>
    /// <param name="rows">The number of rows in the sprite sheet.</param>
    /// <param name="index">The sprite index.</param>
    /// <returns>The sprite size and location as a <see cref="Rectangle" />.</returns>
    public static Rectangle GetSpriteSizeAndLocation(Point imageSize, byte columns, byte rows, byte index) {
        var size = GetSpriteSize(imageSize, columns, rows);
        var row = Math.DivRem(index, columns, out var column);
        var location = new Point(column * size.X, row * size.Y);
        return new Rectangle(location, size);
    }

    /// <inheritdoc />
    public override void LoadContent(Texture2D content) {
        base.LoadContent(content);
        this._spriteIndexToLocation.Clear();
        this.ResetColumnWidth();
        this.ResetRowHeight();
    }

    /// <summary>
    /// Unloads the content.
    /// </summary>
    public void UnloadContent() {
        this.Content = null;
    }

    /// <inheritdoc />
    protected override IEnumerable<IIdentifiable> GetPackages() {
        var packages = new List<IIdentifiable>();
        packages.AddRange(this._autoTileSets);
        packages.AddRange(this._spriteAnimations);
        return packages;
    }

    private static int GetColumnWidth(int imageWidth, byte columns) {
        return columns != 0 ? imageWidth / columns : 0;
    }

    private static int GetRowHeight(int imageHeight, byte rows) {
        return rows != 0 ? imageHeight / rows : 0;
    }

    private void ResetColumnWidth() {
        if (this.Content != null && this._columns != 0) {
            this.SpriteSize = new Point(GetColumnWidth(this.Content.Width, this._columns), this.SpriteSize.Y);
        }
    }

    private void ResetRowHeight() {
        if (this.Content != null && this._rows > 0) {
            this.SpriteSize = new Point(this.SpriteSize.X, GetRowHeight(this.Content.Height, this._rows));
        }
    }

    private void SpriteSheetAsset_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.NewItems != null) {
            foreach (var newItem in e.NewItems.OfType<SpriteSheetMember>()) {
                newItem.SpriteSheet = this;
            }
        }
    }
}