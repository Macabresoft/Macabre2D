namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// Defines how to display the input action based on defined bindings.
/// </summary>
public enum InputActionDisplayMode {
    Primary,
    SecondaryThenPrimary,
    SecondaryOnly
}

/// <summary>
/// A renderer that combines <see cref="InputAction" /> with <see cref="GamePadIconSet" /> and <see cref="KeyboardIconSet" />
/// to render <see cref="Buttons" />,  <see cref="Keys" />, and <see cref="MouseButton" /> associated with actions.
/// </summary>
public class InputActionRenderer : BaseSpriteEntity {
    private InputAction _action;
    private int _currentKerning;
    private InputActionDisplayMode _displayMode;
    private GamePadDisplay _gamePadDisplay = GamePadDisplay.X;
    private InputDevice _inputDeviceToRender = InputDevice.Auto;
    private int _kerning;
    private Keys _key = Keys.None;
    private MouseButton _mouseButton = MouseButton.None;
    private Buttons _primaryButton = Buttons.None;
    private Buttons _secondaryButton = Buttons.None;
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

    /// <summary>
    /// Gets the icon set reference for the mouse.
    /// </summary>
    [DataMember]
    public MouseButtonIconSetReference MouseReference { get; } = new();

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

    /// <summary>
    /// Gets or sets a value indicating whether to display the primary or secondary binding for this input.
    /// </summary>
    [DataMember]
    public InputActionDisplayMode DisplayMode {
        get => this._displayMode;
        set {
            if (this._displayMode != value) {
                this._displayMode = value;
                if (this.IsInitialized) {
                    this.ResetSprite();
                }
            }
        }
    }

    /// <summary>
    /// Gets the input device to render.
    /// </summary>
    [DataMember]
    public InputDevice InputDeviceToRender {
        get => this._inputDeviceToRender;
        set {
            if (value != this._inputDeviceToRender) {
                this._inputDeviceToRender = value;
                this.RequestRefresh();
            }
        }
    }

    /// <summary>
    /// Gets or sets the kerning. This is the space between letters in pixels. Positive numbers will increase the space, negative numbers will decrease it.
    /// </summary>
    [DataMember]
    public int Kerning {
        get => this._kerning;
        set {
            if (value != this._kerning) {
                this._kerning = value;
                this.RequestRefresh();
            }
        }
    }

    /// <inheritdoc />
    protected override SpriteSheet? SpriteSheet => this._spriteSheet;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();

        this.Game.InputDeviceChanged -= this.Game_InputDisplayChanged;
        this.Game.SettingsSaved -= this.Game_SettingsSaved;
        this.GamePadNReference.AssetChanged -= this.IconSetReference_AssetChanged;
        this.GamePadSReference.AssetChanged -= this.IconSetReference_AssetChanged;
        this.GamePadXReference.AssetChanged -= this.IconSetReference_AssetChanged;
        this.KeyboardReference.AssetChanged -= this.IconSetReference_AssetChanged;
        this.MouseReference.AssetChanged -= this.IconSetReference_AssetChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.GamePadNReference.AssetChanged += this.IconSetReference_AssetChanged;
        this.GamePadSReference.AssetChanged += this.IconSetReference_AssetChanged;
        this.GamePadXReference.AssetChanged += this.IconSetReference_AssetChanged;
        this.KeyboardReference.AssetChanged += this.IconSetReference_AssetChanged;
        this.MouseReference.AssetChanged += this.IconSetReference_AssetChanged;

        this.Game.InputDeviceChanged += this.Game_InputDisplayChanged;
        this.Game.SettingsSaved += this.Game_SettingsSaved;
        this.Game.InputBindings.BindingChanged += this.InputBindings_BindingChanged;
        this.ResetBindings();
    }

    public override void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (this.Game.DesiredInputDevice != InputDevice.KeyboardMouse && this._gamePadDisplay != this.Game.InputBindings.DesiredGamePad) {
            this.ResetSprite();
        }

        base.Render(frameTime, viewBoundingArea, colorOverride);
    }

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
        yield return this.KeyboardReference;
        yield return this.MouseReference;
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
            GamePadDisplay.X => this.GamePadXReference.PackagedAsset ?? this.Project.Fallbacks.GamePadXReference.PackagedAsset,
            GamePadDisplay.N => this.GamePadNReference.PackagedAsset ?? this.Project.Fallbacks.GamePadNReference.PackagedAsset,
            GamePadDisplay.S => this.GamePadSReference.PackagedAsset ?? this.Project.Fallbacks.GamePadSReference.PackagedAsset,
            _ => null
        };
    }

    private KeyboardIconSet? GetKeyboardIconSet() => this.KeyboardReference.PackagedAsset ?? this.Project.Fallbacks.KeyboardReference.PackagedAsset;

    private MouseButtonIconSet? GetMouseButtonIconSet() => this.MouseReference.PackagedAsset ?? this.Project.Fallbacks.MouseReference.PackagedAsset;

    private void IconSetReference_AssetChanged(object? sender, bool hasAsset) {
        this.ResetSprite();
    }

    private void InputBindings_BindingChanged(object? sender, InputAction e) {
        this.ResetBindings();
    }

    private void RequestRefresh() {
        if (this.IsInitialized) {
            this.ResetSprite();
        }
    }

    private void ResetBindings() {
        if (this.IsInitialized) {
            if (this.Game.InputBindings.TryGetBindings(this._action, out var primaryButton, out var secondaryButton, out var key, out var mouseButton)) {
                this._primaryButton = GetFirst(primaryButton);
                this._secondaryButton = GetFirst(secondaryButton);
                this._key = key;
                this._mouseButton = mouseButton;
            }

            this.ResetSprite();
        }
    }

    private void ResetSprite() {
        this._currentKerning = this.Kerning;
        this._gamePadDisplay = this.Game.InputBindings.DesiredGamePad;

        this._spriteIndex = null;
        this._spriteSheet = null;

        var inputDevice = this.InputDeviceToRender == InputDevice.Auto ? this.Game.DesiredInputDevice : this.InputDeviceToRender;
        if (inputDevice == InputDevice.KeyboardMouse) {
            if (this.DisplayMode == InputActionDisplayMode.SecondaryOnly || this.DisplayMode == InputActionDisplayMode.SecondaryThenPrimary && this._mouseButton != MouseButton.None) {
                var iconSet = this.GetMouseButtonIconSet();
                if (iconSet != null && iconSet.TryGetSpriteIndex(this._mouseButton, out var index)) {
                    this._spriteIndex = index;
                    this._spriteSheet = iconSet.SpriteSheet;
                    this._currentKerning += iconSet.GetKerning(this._mouseButton);
                }
            }
            else if (this._key != Keys.None) {
                var iconSet = this.GetKeyboardIconSet();
                if (iconSet != null && iconSet.TryGetSpriteIndex(this._key, out var index)) {
                    this._spriteIndex = index;
                    this._spriteSheet = iconSet.SpriteSheet;
                    this._currentKerning += iconSet.GetKerning(this._key);
                }
            }
        }
        else if (this.DisplayMode == InputActionDisplayMode.SecondaryOnly || this.DisplayMode == InputActionDisplayMode.SecondaryThenPrimary && this._secondaryButton != Buttons.None) {
            var iconSet = this.GetGamePadIconSet();

            if (iconSet != null && iconSet.TryGetSpriteIndex(this._secondaryButton, out var index)) {
                this._spriteIndex = index;
                this._spriteSheet = iconSet.SpriteSheet;
                this._currentKerning += iconSet.GetKerning(this._secondaryButton);
            }
        }
        else if (this._primaryButton != Buttons.None) {
            var iconSet = this.GetGamePadIconSet();

            if (iconSet != null && iconSet.TryGetSpriteIndex(this._primaryButton, out var index)) {
                this._spriteIndex = index;
                this._spriteSheet = iconSet.SpriteSheet;
                this._currentKerning += iconSet.GetKerning(this._primaryButton);
            }
        }

        this.Reset();
    }
}