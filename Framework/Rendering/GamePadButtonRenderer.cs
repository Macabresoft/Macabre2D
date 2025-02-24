﻿namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// Renders a game pad button.
/// </summary>
public class GamePadButtonRenderer : BaseSpriteEntity {
    private Buttons _button = Buttons.A;
    private int _currentKerning;
    private GamePadDisplay _gamePadDisplay = GamePadDisplay.X;
    private byte? _spriteIndex;
    private SpriteSheet? _spriteSheet;

    /// <summary>
    /// Gets the <see cref="GamePadIconSetReference" /> for "Game Pad N".
    /// </summary>
    [DataMember]
    public GamePadIconSetReference GamePadNReference { get; } = new();

    /// <summary>
    /// Gets the <see cref="GamePadIconSetReference" /> for "Game Pad S".
    /// </summary>
    [DataMember]
    public GamePadIconSetReference GamePadSReference { get; } = new();

    /// <summary>
    /// Gets the <see cref="GamePadIconSetReference" /> for "Game Pad X".
    /// </summary>
    [DataMember]
    public GamePadIconSetReference GamePadXReference { get; } = new();

    /// <inheritdoc />
    public override byte? SpriteIndex => this._spriteIndex;

    /// <summary>
    /// Gets and sets the button. Please don't use multiple values, it won't work.
    /// </summary>
    [DataMember]
    public Buttons Button {
        get => this._button;
        set {
            if (value != this._button) {
                this._button = value;
                this.ResetSprite();
            }
        }
    }

    /// <inheritdoc />
    protected override SpriteSheet? SpriteSheet => this._spriteSheet;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.GamePadNReference.AssetChanged -= this.GamePadReference_AssetChanged;
        this.GamePadSReference.AssetChanged -= this.GamePadReference_AssetChanged;
        this.GamePadXReference.AssetChanged -= this.GamePadReference_AssetChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.GamePadNReference.AssetChanged += this.GamePadReference_AssetChanged;
        this.GamePadSReference.AssetChanged += this.GamePadReference_AssetChanged;
        this.GamePadXReference.AssetChanged += this.GamePadReference_AssetChanged;

        this.ResetSprite();
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this._gamePadDisplay != this.Game.InputBindings.DesiredGamePad) {
            this.ResetSprite();
        }

        base.Render(frameTime, viewBoundingArea, colorOverride);
    }

    /// <inheritdoc />
    protected override Vector2 CreateSize() {
        if (this.SpriteSheet is { } spriteSheet) {
            return new Vector2(spriteSheet.SpriteSize.X + this._currentKerning, spriteSheet.SpriteSize.Y);
        }

        return base.CreateSize();
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.GamePadNReference;
        yield return this.GamePadSReference;
        yield return this.GamePadXReference;
    }

    private void GamePadReference_AssetChanged(object? sender, bool hasAsset) {
        this.ResetSprite();
    }

    private void ResetSprite() {
        this._gamePadDisplay = this.Game.InputBindings.DesiredGamePad;

        var iconSet = this.Game.InputBindings.DesiredGamePad switch {
            GamePadDisplay.X => this.GamePadXReference.PackagedAsset ?? this.Project.Fallbacks.GamePadXReference.PackagedAsset,
            GamePadDisplay.N => this.GamePadNReference.PackagedAsset ?? this.Project.Fallbacks.GamePadNReference.PackagedAsset,
            GamePadDisplay.S => this.GamePadSReference.PackagedAsset ?? this.Project.Fallbacks.GamePadSReference.PackagedAsset,
            _ => null
        };

        if (iconSet != null && iconSet.TryGetSpriteIndex(this.Button, out var index)) {
            this._spriteIndex = index;
            this._spriteSheet = iconSet.SpriteSheet;
            this._currentKerning += iconSet.GetKerning(this.Button);
        }
        else {
            this._spriteIndex = null;
            this._spriteSheet = null;
        }

        this.Reset();
    }
}