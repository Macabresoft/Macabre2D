namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A tile map that uses a <see cref="AutoTileMap" /> to create a box with a height and width.
/// </summary>
public class BoxTileMap : RenderableEntity {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly List<(Vector2 Position, byte TileIndex)> _spritePositionsAndTileIndex = new();
    private int _height = 3;
    private int _width = 3;

    /// <inheritdoc />
    public override event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoxTileMap" /> class.
    /// </summary>
    public BoxTileMap() : base() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets the tile set.
    /// </summary>
    [DataMember]
    public AutoTileSetReference TileSet { get; } = new();
    
    
    /// <inheritdoc />
    public override RenderPriority RenderPriority {
        get {
            if (this.RenderPriorityOverride.IsEnabled) {
                return this.RenderPriorityOverride.Value;
            }

            return this.TileSet.Asset?.DefaultRenderPriority ?? default;
        }
        
        set {
            this.RenderPriorityOverride.IsEnabled = true;
            this.RenderPriorityOverride.Value = value;
        }
    }

    /// <summary>
    /// Gets a render priority override.
    /// </summary>
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public RenderPriorityOverride RenderPriorityOverride { get; } = new();

    /// <summary>
    /// Gets or sets the height in tiles.
    /// </summary>
    [DataMember]
    public int Height {
        get => this._height;
        set {
            if (value != this._height) {
                this._height = Math.Max(value, 1);
                this.Reset();
            }
        }
    }

    /// <summary>
    /// Gets or sets the render options.
    /// </summary>
    [DataMember]
    public RenderOptions RenderOptions { get; private set; } = new();

    /// <summary>
    /// Gets or sets the width in tiles.
    /// </summary>
    [DataMember]
    public int Width {
        get => this._width;
        set {
            if (value != this._width) {
                this._width = Math.Max(value, 1);
                this.Reset();
            }
        }
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        this.RenderOptions.PropertyChanged -= this.RenderSettings_PropertyChanged;
        this.TileSet.AssetChanged -= this.TileSet_AssetChanged;
        base.Deinitialize();
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.TileSet.AssetChanged += this.TileSet_AssetChanged;
        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.RenderOptions.Initialize(this.CreateSize);
        this.ResetSprites();
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.RenderOptions.Color);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this.SpriteBatch is { } spriteBatch && this.TileSet is { Asset: { } spriteSheet, PackagedAsset: { } tileSet }) {
            foreach (var sprite in this._spritePositionsAndTileIndex) {
                if (tileSet.TryGetSpriteIndex(sprite.TileIndex, out var spriteIndex)) {
                    this.RenderSprite(spriteBatch, spriteSheet, sprite.Position, spriteIndex, colorOverride);
                }
            }
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.TileSet;
    }


    /// <summary>
    /// Gets the sprite size in world units.
    /// </summary>
    /// <returns>The sprite size.</returns>
    protected Vector2 GetSpriteUnitSize() {
        var result = Vector2.Zero;

        if (this.TileSet.Asset is { } spriteSheet) {
            result = new Vector2(
                spriteSheet.SpriteSize.X * this.Project.UnitsPerPixel,
                spriteSheet.SpriteSize.Y * this.Project.UnitsPerPixel);
        }

        return result;
    }

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.Reset();
    }

    /// <summary>
    /// Sets the size of this <see cref="BoxTileMap" />.
    /// </summary>
    /// <remarks>
    /// This is more efficient than setting <see cref="Height" /> and <see cref="Width" /> separately,
    /// as it only needs to update the tiles a single time.
    /// </remarks>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    protected void SetSize(int width, int height) {
        this._width = Math.Max(width, 1);
        this._height = Math.Max(height, 1);
        this.Reset();
    }

    private BoundingArea CreateBoundingArea() => this.RenderOptions.CreateBoundingArea(this);

    private Vector2 CreateSize() {
        var result = Vector2.Zero;
        if (this.TileSet.Asset is { } spriteSheet) {
            return new Vector2(spriteSheet.SpriteSize.X * this.Width, spriteSheet.SpriteSize.Y * this.Height);
        }

        return result;
    }

    private void RenderSettings_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.RenderOptions.Offset)) {
            this.ResetBoundingArea();
        }
    }

    private void RenderSprite(SpriteBatch spriteBatch, SpriteSheet spriteSheet, Vector2 position, byte spriteIndex, Color colorOverride) {
        spriteBatch.Draw(
            this.Project.PixelsPerUnit,
            spriteSheet,
            spriteIndex,
            position,
            colorOverride,
            this.RenderOptions.Orientation);
    }

    private void Reset() {
        this.RenderOptions.InvalidateSize();
        this.ResetBoundingArea();
    }


    private void ResetBoundingArea() {
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
        this.ResetSprites();
    }

    private void ResetSprites() {
        this._spritePositionsAndTileIndex.Clear();
        var spriteSize = this.GetSpriteUnitSize();

        if (spriteSize != Vector2.Zero) {
            var minimum = this.BoundingArea.Minimum;
            var (x, y) = minimum;
            if (this.Height == 1) {
                if (this.Width == 1) {
                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)CardinalDirections.None));
                }
                else {
                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)CardinalDirections.East));
                    x += spriteSize.X;

                    for (var column = 1; column < this.Width - 1; column++) {
                        this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.West)));
                        x += spriteSize.X;
                    }

                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)CardinalDirections.West));
                }
            }
            else if (this.Width == 1) {
                this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)CardinalDirections.North));
                y += spriteSize.Y;

                for (var row = 1; row < this.Height - 1; row++) {
                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.North | CardinalDirections.South)));
                    y += spriteSize.Y;
                }

                this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)CardinalDirections.South));
            }
            else {
                this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.North)));
                x += spriteSize.X;

                for (var column = 1; column < this.Width - 1; column++) {
                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.West | CardinalDirections.North)));
                    x += spriteSize.X;
                }

                this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.West | CardinalDirections.North)));
                x = minimum.X;
                y += spriteSize.Y;

                for (var row = 1; row < this.Height - 1; row++) {
                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.North | CardinalDirections.South)));
                    x += spriteSize.X;

                    for (var column = 1; column < this.Width - 1; column++) {
                        this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.West | CardinalDirections.North | CardinalDirections.South)));
                        x += spriteSize.X;
                    }

                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.West | CardinalDirections.North | CardinalDirections.South)));
                    y += spriteSize.Y;
                    x = minimum.X;
                }

                this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.South)));
                x += spriteSize.X;

                for (var column = 1; column < this.Width - 1; column++) {
                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.West | CardinalDirections.South)));
                    x += spriteSize.X;
                }

                this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.West | CardinalDirections.South)));
            }
        }
    }

    private void TileSet_AssetChanged(object? sender, bool hasAsset) {
        this.Reset();
    }
}