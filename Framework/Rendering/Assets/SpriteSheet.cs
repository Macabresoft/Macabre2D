namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A sprite sheet tied to a single <see cref="Texture2D" /> which also defines sprites, animations, and tile sets.
/// </summary>
[DataContract(Name = "Sprite Sheet")]
[Category("Sprite Sheet")]
public class SpriteSheet : AssetPackage<Texture2D> {
    /// <summary>
    /// The valid file extensions for a <see cref="Texture2D" />.
    /// </summary>
    public static readonly string[] ValidFileExtensions = [".jpg", ".png"];

    private readonly Dictionary<byte, Point> _spriteIndexToLocation = [];

    [DataMember]
    [Category("Auto Tile Sets")]
    private AutoTileSetCollection _autoTileSets = [];

    private byte _columns = 1;

    [DataMember]
    [Category("Fonts")]
    private SpriteSheetFontCollection _fonts = [];

    [DataMember]
    [Category("Icon Sets")]
    private SpriteSheetIconSetCollection _iconSets = [];

    private byte _rows = 1;

    [DataMember]
    [Category("Animations")]
    private SpriteAnimationCollection _spriteAnimations = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteSheet" /> class.
    /// </summary>
    public SpriteSheet() {
        this._autoTileSets.CollectionChanged += this.SpriteSheetMember_CollectionChanged;
        this._autoTileSets.PropertyChanged += this.RaisePropertyChanged;
        this._fonts.CollectionChanged += this.SpriteSheetMember_CollectionChanged;
        this._fonts.PropertyChanged += this.RaisePropertyChanged;
        this._iconSets.CollectionChanged += this.SpriteSheetMember_CollectionChanged;
        this._iconSets.PropertyChanged += this.RaisePropertyChanged;
        this._spriteAnimations.CollectionChanged += this.SpriteSheetMember_CollectionChanged;
        this._spriteAnimations.PropertyChanged += this.RaisePropertyChanged;
    }

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

    /// <summary>
    /// Gets or sets the default render priority.
    /// </summary>
    [DataMember]
    public RenderPriority DefaultRenderPriority {
        get;
        set => this.Set(ref field, value);
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
    /// Gets the size of a single sprite on this sprite sheet.
    /// </summary>
    public Point SpriteSize {
        get;
        private set => this.Set(ref field, value);
    }

    /// <summary>
    /// Draws the specified sprite.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch.</param>
    /// <param name="pixelsPerUnit">The pixels per unit.</param>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="position">The position.</param>
    /// <param name="scale">The scale.</param>
    /// <param name="color">The color.</param>
    /// <param name="orientation">The orientation.</param>
    public void Draw(
        SpriteBatch spriteBatch,
        ushort pixelsPerUnit,
        byte spriteIndex,
        Vector2 position,
        Vector2 scale,
        Color color,
        SpriteEffects orientation) {
        if (this.Content != null && this.SpriteSize != Point.Zero) {
            spriteBatch.Draw(
                this.Content,
                position * pixelsPerUnit,
                new Rectangle(this.GetSpriteLocation(spriteIndex), this.SpriteSize),
                color,
                0f,
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
    /// <param name="position">The position.</param>
    /// <param name="color">The color.</param>
    /// <param name="orientation">The orientation.</param>
    public void Draw(
        SpriteBatch spriteBatch,
        ushort pixelsPerUnit,
        byte spriteIndex,
        Vector2 position,
        Color color,
        SpriteEffects orientation) {
        this.Draw(spriteBatch, pixelsPerUnit, spriteIndex, position, Vector2.One, color, orientation);
    }

    /// <summary>
    /// Gets the <see cref="SpriteSheetMember" /> collection of the specified type.
    /// </summary>
    /// <typeparam name="TAsset">The type of asset.</typeparam>
    /// <returns>A collection of <see cref="SpriteSheetMember" />.</returns>
    public IReadOnlyCollection<TAsset> GetAssets<TAsset>() where TAsset : SpriteSheetMember {
        IReadOnlyCollection<TAsset> result;
        var memberType = typeof(TAsset);
        if (memberType == typeof(SpriteAnimation)) {
            result = (IReadOnlyCollection<TAsset>)this._spriteAnimations;
        }
        else if (memberType == typeof(AutoTileSet)) {
            result = (IReadOnlyCollection<TAsset>)this._autoTileSets;
        }
        else if (memberType == typeof(SpriteSheetFont)) {
            result = (IReadOnlyCollection<TAsset>)this._fonts;
        }
        else if (memberType.IsAssignableTo(typeof(SpriteSheetIconSet))) {
            result = this._iconSets.OfType<TAsset>().ToList();
        }
        else {
            result = new List<TAsset>();
        }

        return result;
    }

    /// <summary>
    /// Gets the <see cref="SpriteSheetMember" /> collection of the specified type.
    /// </summary>
    /// <returns>A collection of <see cref="SpriteSheetMember" />.</returns>
    public IReadOnlyCollection<SpriteSheetMember> GetAssets(Type memberType) {
        IReadOnlyCollection<SpriteSheetMember> result;
        if (memberType == typeof(SpriteAnimation)) {
            result = this._spriteAnimations;
        }
        else if (memberType == typeof(AutoTileSet)) {
            result = this._autoTileSets;
        }
        else if (memberType == typeof(SpriteSheetFont)) {
            result = this._fonts;
        }
        else if (memberType.IsAssignableTo(typeof(SpriteSheetIconSet))) {
            result = this._iconSets.OfType<SpriteSheetMember>().Where(x => x.GetType().IsAssignableTo(memberType)).ToList();
        }
        else {
            result = new List<SpriteSheetMember>();
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
    /// Gets the member collection associated with the provided member type.
    /// </summary>
    /// <param name="memberType">The member type.</param>
    /// <returns>The collection.</returns>
    public ISpriteSheetMemberCollection? GetMemberCollection(Type memberType) {
        if (memberType == typeof(SpriteAnimation)) {
            return this._spriteAnimations;
        }

        if (memberType == typeof(AutoTileSet)) {
            return this._autoTileSets;
        }

        if (memberType == typeof(SpriteSheetFont)) {
            return this._fonts;
        }

        if (memberType.IsAssignableTo(typeof(SpriteSheetIconSet))) {
            return this._iconSets;
        }

        return null;
    }

    /// <summary>
    /// Gets the member collection associated with the provided member type.
    /// </summary>
    /// <typeparam name="TMember">The member type.</typeparam>
    /// <returns>The collection.</returns>
    public SpriteSheetMemberCollection<TMember>? GetMemberCollection<TMember>() where TMember : SpriteSheetMember, new() => this.GetMemberCollection(typeof(TMember)) as SpriteSheetMemberCollection<TMember>;

    /// <summary>
    /// Gets all the member collections contained in this <see cref="SpriteSheet" />.
    /// </summary>
    /// <returns>The member collections.</returns>
    public IReadOnlyCollection<INameableCollection> GetMemberCollections() =>
        new List<INameableCollection> {
            this._autoTileSets,
            this._spriteAnimations,
            this._fonts,
            this._iconSets
        };

    /// <summary>
    /// Gets the next sprite index. If the value ends up greater than the max index of this sprite sheet, it will wrap back around.
    /// </summary>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <returns>The next sprite index in this sheet.</returns>
    public byte GetNextSpriteIndex(byte spriteIndex) {
        var result = ++spriteIndex;
        var maxIndex = this.MaxIndex;

        if (maxIndex == 0) {
            result = 0;
        }
        else {
            while (result > maxIndex) {
                result -= maxIndex;
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the number of empty columns of pixels on the right side of this specific sprite.
    /// </summary>
    /// <param name="spriteIndex"></param>
    /// <remarks>
    /// This will return 0 if the sprite sheet's <see cref="Texture" /> has not been loaded. This is
    /// also inefficient and should not be called during runtime. The most likely reason to use this
    /// method is for determining the automatic kerning value.
    /// for a <see cref="SpriteSheetFont" />.
    /// </remarks>
    /// <returns>The number of empty pixels from right edge.</returns>
    public int GetNumberOfEmptyPixelsFromRightEdge(byte spriteIndex) {
        var result = 0;

        if (this.Content is { } texture) {
            var rectangle = new Rectangle(this.GetSpriteLocation(spriteIndex), this.SpriteSize);
            var data = new Color[this.SpriteSize.X * this.SpriteSize.Y];
            texture.GetData(0, rectangle, data, 0, data.Length);

            // Data starts at top left of sprite and goes horizontal before dropping down to the next row
            for (var x = this.SpriteSize.X - 1; x >= 0; x--) {
                var isTransparent = true;
                for (var y = 0; y < this.SpriteSize.Y; y++) {
                    isTransparent &= data[this.SpriteSize.X * y + x].A == 0;

                    if (!isTransparent) {
                        break;
                    }
                }

                if (isTransparent) {
                    result++;
                }
                else {
                    break;
                }
            }
        }

        return result;
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
    public static Point GetSpriteSize(Point imageSize, byte columns, byte rows) => new(GetColumnWidth(imageSize.X, columns), GetRowHeight(imageSize.Y, rows));

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
        packages.AddRange(this._fonts);
        packages.AddRange(this._iconSets);
        return packages;
    }

    /// <inheritdoc />
    protected override void SetNewPackagedAssetIds() {
        foreach (var package in this.GetPackages()) {
            package.Id = Guid.NewGuid();
        }
    }

    private static int GetColumnWidth(int imageWidth, byte columns) => columns != 0 ? imageWidth / columns : 0;

    private static int GetRowHeight(int imageHeight, byte rows) => rows != 0 ? imageHeight / rows : 0;

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

    private void SpriteSheetMember_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.NewItems != null) {
            foreach (var newItem in e.NewItems.OfType<SpriteSheetMember>()) {
                newItem.SpriteSheet = this;
            }
        }
    }
}