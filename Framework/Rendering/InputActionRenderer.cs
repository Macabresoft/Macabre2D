namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// A renderer that combines <see cref="InputAction" /> with <see cref="GamePadIconSet" /> and <see cref="KeyboardIconSet" />
/// to render <see cref="Buttons" />,  <see cref="Keys" />, and <see cref="MouseButton" /> associated with actions.
/// </summary>
public class InputActionRenderer : BaseSpriteEntity {
    private InputAction _action;
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
    /// Gets the input action to display.
    /// </summary>
    [DataMember]
    public InputAction Action {
        get => this._action;
        set {
            if (value != this._action) {
                this._action = value;
                this.ResetBindings();
            }
        }
    }

    /// <inheritdoc />
    protected override SpriteSheet? SpriteSheet => this._spriteSheet;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        this.Game.InputDisplayChanged -= this.Game_InputDisplayChanged;
        this.Game.SettingsSaved -= this.Game_SettingsSaved;

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
        this.Game.SettingsSaved += this.Game_SettingsSaved;
        this.ResetSprite();
    }

    /// <summary>
    /// Tries to get the bindings for the current <see cref="Action" />.
    /// </summary>
    /// <param name="buttons">The buttons.</param>
    /// <param name="key">The key.</param>
    /// <param name="mouseButton">The mouse button</param>
    /// <returns>A value indicating whether or not the bindings were found.</returns>
    protected virtual bool TryGetBindings(out Buttons buttons, out Keys key, out MouseButton mouseButton) {
        return this.Game.InputBindings.TryGetBindings(this._action, out buttons, out key, out mouseButton);
    }

    private void Game_InputDisplayChanged(object? sender, InputDisplay e) {
        this.ResetSprite();
    }

    private void Game_SettingsSaved(object? sender, EventArgs e) {
        this.ResetBindings();
    }

    private void GamePadReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(GamePadIconSetReference.ContentId)) {
            this.ResetSprite();
        }
    }

    private static Buttons GetFirst(Buttons buttons) {
        return buttons == Buttons.None ? buttons : Enum.GetValues<Buttons>().FirstOrDefault(x => buttons.HasFlag(x));
    }

    private void ResetBindings() {
        if (this.TryGetBindings(out var buttons, out var key, out var mouseButton)) {
            this._button = GetFirst(buttons);
        }

        if (this.IsInitialized) {
            this.ResetSprite();
        }
    }

    private void ResetSprite() {
        var iconSet = this.Game.InputDisplayStyle switch {
            InputDisplay.GamePadX => this.GamePadXReference.PackagedAsset,
            InputDisplay.GamePadN => this.GamePadNReference.PackagedAsset,
            InputDisplay.GamePadS => this.GamePadSReference.PackagedAsset,
            InputDisplay.Auto => null,
            InputDisplay.Keyboard => null,
            _ => null
        };

        if (iconSet != null && iconSet.TryGetSpriteIndex(this._button, out var index)) {
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