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
    private Buttons _button = Buttons.None;
    private Keys _key = Keys.None;
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

    /// <summary>
    /// Gets the icon set reference for keyboards.
    /// </summary>
    [DataMember]
    public KeyboardIconSetReference KeyboardReference { get; } = new();

    /// <inheritdoc />
    public override byte? SpriteIndex {
        get => this._spriteIndex;
    }

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
    protected override SpriteSheet? SpriteSheet {
        get => this._spriteSheet;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        this.Game.InputDeviceChanged -= this.Game_InputDisplayChanged;
        this.Game.SettingsSaved -= this.Game_SettingsSaved;

        this.GamePadNReference.PropertyChanged -= this.IconSetReference_PropertyChanged;
        this.GamePadSReference.PropertyChanged -= this.IconSetReference_PropertyChanged;
        this.GamePadXReference.PropertyChanged -= this.IconSetReference_PropertyChanged;

        base.Initialize(scene, parent);

        this.GamePadNReference.Initialize(this.Scene.Assets);
        this.GamePadSReference.Initialize(this.Scene.Assets);
        this.GamePadXReference.Initialize(this.Scene.Assets);
        this.KeyboardReference.Initialize(this.Scene.Assets);

        this.GamePadNReference.PropertyChanged += this.IconSetReference_PropertyChanged;
        this.GamePadSReference.PropertyChanged += this.IconSetReference_PropertyChanged;
        this.GamePadXReference.PropertyChanged += this.IconSetReference_PropertyChanged;
        this.KeyboardReference.PropertyChanged += this.IconSetReference_PropertyChanged;

        this.Game.InputDeviceChanged += this.Game_InputDisplayChanged;
        this.Game.SettingsSaved += this.Game_SettingsSaved;
        this.Scene.Invoke(this.ResetBindings);
    }

    private void Game_InputDisplayChanged(object? sender, InputDevice e) {
        this.ResetSprite();
    }

    private void Game_SettingsSaved(object? sender, EventArgs e) {
        this.ResetBindings();
    }

    private static Buttons GetFirst(Buttons buttons) {
        if (buttons == Buttons.None) {
            return buttons;
        }

        var allButtons = Enum.GetValues<Buttons>().ToList();
        allButtons.Remove(Buttons.None);
        return allButtons.FirstOrDefault(x => buttons.HasFlag(x));
    }

    private GamePadIconSet? GetGamePadIconSet() {
        return this.Game.InputBindings.DesiredGamePad switch {
            GamePadDisplay.X => this.GamePadXReference.PackagedAsset ?? this.Project.GamePadXReference.PackagedAsset,
            GamePadDisplay.N => this.GamePadNReference.PackagedAsset ?? this.Project.GamePadNReference.PackagedAsset,
            GamePadDisplay.S => this.GamePadSReference.PackagedAsset ?? this.Project.GamePadSReference.PackagedAsset,
            _ => null
        };
    }

    private KeyboardIconSet? GetKeyboardIconSet() {
        return this.KeyboardReference.PackagedAsset ?? this.Project.KeyboardReference.PackagedAsset;
    }

    private void IconSetReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.GamePadXReference.ContentId)) {
            this.ResetSprite();
        }
    }

    private void ResetBindings() {
        if (this.IsInitialized) {
            if (this.Game.InputBindings.TryGetBindings(this._action, out var button, out var key, out _)) {
                this._button = GetFirst(button);
                this._key = key;
            }

            this.ResetSprite();
        }
    }

    private void ResetSprite() {
        this._spriteIndex = null;
        this._spriteSheet = null;

        if (this.Game.DesiredInputDevice == InputDevice.KeyboardMouse) {
            var iconSet = this.GetKeyboardIconSet();
            if (iconSet != null && iconSet.TryGetSpriteIndex(this._key, out var index)) {
                this._spriteIndex = index;
                this._spriteSheet = iconSet.SpriteSheet;
            }
        }
        else {
            var iconSet = this.GetGamePadIconSet();

            if (iconSet != null && iconSet.TryGetSpriteIndex(this._button, out var index)) {
                this._spriteIndex = index;
                this._spriteSheet = iconSet.SpriteSheet;
            }
        }

        this.Reset();
    }
}