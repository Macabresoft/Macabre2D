namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// Renders a game pad button according to <see cref="IGame.InputDisplayStyle" />.
/// </summary>
public class GamePadButtonRenderer : BaseSpriteEntity {
    private Buttons _button = Buttons.A;
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
    public override void Initialize(IScene scene, IEntity parent) {
        this.Game.InputDisplayChanged -= this.Game_InputDisplayChanged;

        this.GamePadNReference.PropertyChanged -= this.GamePadReference_PropertyChanged;
        this.GamePadSReference.PropertyChanged -= this.GamePadReference_PropertyChanged;
        this.GamePadXReference.PropertyChanged -= this.GamePadReference_PropertyChanged;

        base.Initialize(scene, parent);

        this.GamePadNReference.Initialize(this.Scene.Assets);
        this.GamePadSReference.Initialize(this.Scene.Assets);
        this.GamePadXReference.Initialize(this.Scene.Assets);

        this.GamePadNReference.PropertyChanged += this.GamePadReference_PropertyChanged;
        this.GamePadSReference.PropertyChanged += this.GamePadReference_PropertyChanged;
        this.GamePadXReference.PropertyChanged += this.GamePadReference_PropertyChanged;

        this.Game.InputDisplayChanged += this.Game_InputDisplayChanged;
        this.ResetSprite();
    }

    private void Game_InputDisplayChanged(object? sender, InputDisplay e) {
        this.ResetSprite();
    }

    private void GamePadReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(GamePadIconSetReference.ContentId)) {
            this.ResetSprite();
        }
    }

    private void ResetSprite() {
        var iconSet = this.Game.InputDisplayStyle switch {
            InputDisplay.GamePadX => this.GamePadXReference?.PackagedAsset,
            InputDisplay.GamePadN => this.GamePadNReference?.PackagedAsset,
            InputDisplay.GamePadS => this.GamePadSReference?.PackagedAsset,
            InputDisplay.Auto => null,
            InputDisplay.Keyboard => null,
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