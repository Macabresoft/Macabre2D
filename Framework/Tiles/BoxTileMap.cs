namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
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

    /// <summary>
    /// Gets or sets the color.
    /// </summary>
    [DataMember]
    public Color Color { get; set; } = Color.White;

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
        this.TileSet.PropertyChanged -= this.TileSet_PropertyChanged;
        base.Deinitialize();
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.TileSet.PropertyChanged += this.TileSet_PropertyChanged;
        this.RenderOptions.PropertyChanged += this.RenderSettings_PropertyChanged;
        this.RenderOptions.Initialize(this.CreateSize);
        this.ResetSprites();
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, this.Color);
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

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
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
        if (this.TileSet.Asset is { } spriteSheet) {
            var spriteHeight = spriteSheet.SpriteSize.Y * this.Project.UnitsPerPixel;
            var spriteWidth = spriteSheet.SpriteSize.X * this.Project.UnitsPerPixel;
            var minimum = this.BoundingArea.Minimum;
            var (x, y) = minimum;
            if (this.Height == 1) {
                if (this.Width == 1) {
                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)CardinalDirections.None));
                }
                else {
                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)CardinalDirections.East));
                    x += spriteWidth;

                    for (var column = 1; column < this.Width - 1; column++) {
                        this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.West)));
                        x += spriteWidth;
                    }

                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)CardinalDirections.West));
                }
            }
            else if (this.Width == 1) {
                this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)CardinalDirections.North));
                y += spriteWidth;

                for (var row = 1; row < this.Height - 1; row++) {
                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.North | CardinalDirections.South)));
                    y += spriteWidth;
                }

                this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)CardinalDirections.South));
            }
            else {
                this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.North)));
                x += spriteWidth;

                for (var column = 1; column < this.Width - 1; column++) {
                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.West | CardinalDirections.North)));
                    x += spriteWidth;
                }

                this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.West | CardinalDirections.North)));
                x = minimum.X;
                y += spriteHeight;

                for (var row = 1; row < this.Height - 1; row++) {
                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.North | CardinalDirections.South)));
                    x += spriteWidth;

                    for (var column = 1; column < this.Width - 1; column++) {
                        this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.West | CardinalDirections.North | CardinalDirections.South)));
                        x += spriteWidth;
                    }

                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.West | CardinalDirections.North | CardinalDirections.South)));
                    y += spriteHeight;
                    x = minimum.X;
                }

                this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.South)));
                x += spriteWidth;

                for (var column = 1; column < this.Width - 1; column++) {
                    this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.East | CardinalDirections.West | CardinalDirections.South)));
                    x += spriteWidth;
                }

                this._spritePositionsAndTileIndex.Add((new Vector2(x, y), (byte)(CardinalDirections.West | CardinalDirections.South)));
            }
        }
    }

    private void TileSet_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(AutoTileSetReference.ContentId)) {
            this.Reset();
        }
    }
}