namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// Renders a game pad button.
/// </summary>
public class GamePadButtonRenderer : BaseSpriteEntity {
    private Buttons _button = Buttons.A;
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
        this.GamePadNReference.PropertyChanged -= this.GamePadReference_PropertyChanged;
        this.GamePadSReference.PropertyChanged -= this.GamePadReference_PropertyChanged;
        this.GamePadXReference.PropertyChanged -= this.GamePadReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.GamePadNReference.PropertyChanged += this.GamePadReference_PropertyChanged;
        this.GamePadSReference.PropertyChanged += this.GamePadReference_PropertyChanged;
        this.GamePadXReference.PropertyChanged += this.GamePadReference_PropertyChanged;

        this.ResetSprite();
    }

    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this._gamePadDisplay != this.Game.InputBindings.DesiredGamePad) {
            this.ResetSprite();
        }

        base.Render(frameTime, viewBoundingArea, colorOverride);
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.GamePadNReference;
        yield return this.GamePadSReference;
        yield return this.GamePadXReference;
    }

    private void GamePadReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(GamePadIconSetReference.ContentId)) {
            this.ResetSprite();
        }
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
        }
        else {
            this._spriteIndex = null;
            this._spriteSheet = null;
        }

        this.Reset();
    }
}