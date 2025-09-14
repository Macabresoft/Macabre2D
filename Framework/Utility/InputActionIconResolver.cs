namespace Macabresoft.Macabre2D.Framework;

using System.Diagnostics.CodeAnalysis;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// Interface which resolves icons for <see cref="InputAction" />.
/// </summary>
public interface IInputActionIconResolver {
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="game">The game.</param>
    void Initialize(IGame game);

    /// <summary>
    /// Tries to get the sprite sheet and sprite index of an icon for the given <see cref="InputAction" />.
    /// </summary>
    /// <remarks>
    /// This will default to the icon sets defined as fallbacks in <see cref="IGameProject" />.
    /// </remarks>
    /// <param name="action">The action.</param>
    /// <param name="inputDevice">The input device.</param>
    /// <param name="displayMode">The display mode.</param>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="kerning">The kerning on the icon.</param>
    /// <returns>A value indicating whether an icon was found.</returns>
    bool TryGetIcon(
        InputAction action,
        InputDevice inputDevice,
        InputActionDisplayMode displayMode,
        [NotNullWhen(true)] out SpriteSheet? spriteSheet,
        [NotNullWhen(true)] out byte? spriteIndex,
        out int kerning);

    /// <summary>
    /// Tries to get the sprite sheet and sprite index of an icon for the given <see cref="InputAction" />.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="inputDevice">The input device.</param>
    /// <param name="displayMode">The display mode.</param>
    /// <param name="gamePadIconSet">The game pad icon set.</param>
    /// <param name="keyboardIconSet">The keyboard icon set.</param>
    /// <param name="mouseIconSet">The mouse icon set.</param>
    /// <param name="spriteSheet">The sprite sheet.</param>
    /// <param name="spriteIndex">The sprite index.</param>
    /// <param name="kerning">The kerning on the icon.</param>
    /// <returns>A value indicating whether an icon was found.</returns>
    bool TryGetIcon(
        InputAction action,
        InputDevice inputDevice,
        InputActionDisplayMode displayMode,
        GamePadIconSet? gamePadIconSet,
        KeyboardIconSet? keyboardIconSet,
        MouseButtonIconSet? mouseIconSet,
        [NotNullWhen(true)] out SpriteSheet? spriteSheet,
        [NotNullWhen(true)] out byte? spriteIndex,
        out int kerning);
}

/// <summary>
/// Resolves icons for <see cref="InputAction" />.
/// </summary>
public class InputActionIconResolver : IInputActionIconResolver {
    private IGame _game = BaseGame.Empty;

    /// <summary>
    /// Gets an empty <see cref="IInputActionIconResolver" /> instance.
    /// </summary>
    public static IInputActionIconResolver Empty { get; } = new EmptyInputActionIconResolver();

    /// <inheritdoc />
    public void Initialize(IGame game) {
        this._game = game;
    }

    /// <inheritdoc />
    public bool TryGetIcon(
        InputAction action,
        InputDevice inputDevice,
        InputActionDisplayMode displayMode,
        [NotNullWhen(true)] out SpriteSheet? spriteSheet,
        [NotNullWhen(true)] out byte? spriteIndex,
        out int kerning) =>
        this.TryGetIcon(action, inputDevice, displayMode, null, null, null, out spriteSheet, out spriteIndex, out kerning);

    /// <inheritdoc />
    public bool TryGetIcon(
        InputAction action,
        InputDevice inputDevice,
        InputActionDisplayMode displayMode,
        GamePadIconSet? gamePadIconSet,
        KeyboardIconSet? keyboardIconSet,
        MouseButtonIconSet? mouseIconSet,
        [NotNullWhen(true)] out SpriteSheet? spriteSheet,
        [NotNullWhen(true)] out byte? spriteIndex,
        out int kerning) {
        spriteSheet = null;
        spriteIndex = null;
        kerning = 0;

        if (inputDevice == InputDevice.Auto) {
            inputDevice = this._game.DesiredInputDevice;
        }

        if (inputDevice == InputDevice.KeyboardMouse) {
            this._game.InputSettings.TryGetBindings(action, out MouseButton mouseButton);
            this._game.InputSettings.TryGetBindings(action, out Keys key);
            if (displayMode == InputActionDisplayMode.SecondaryOnly || displayMode == InputActionDisplayMode.SecondaryThenPrimary && mouseButton != MouseButton.None) {
                var iconSet = mouseIconSet ?? this._game.Project.Fallbacks.MouseReference.PackagedAsset;
                if (iconSet != null && iconSet.TryGetSpriteIndex(mouseButton, out var index)) {
                    spriteIndex = index;
                    spriteSheet = iconSet.SpriteSheet;
                    kerning = iconSet.GetKerning(mouseButton);
                }
            }
            else if (key != Keys.None) {
                var iconSet = keyboardIconSet ?? this._game.Project.Fallbacks.KeyboardReference.PackagedAsset;
                if (iconSet != null && iconSet.TryGetSpriteIndex(key, out var index)) {
                    spriteIndex = index;
                    spriteSheet = iconSet.SpriteSheet;
                    kerning = iconSet.GetKerning(key);
                }
            }
        }
        else {
            this._game.InputSettings.TryGetBindings(action, out var primaryButton, out var secondaryButton);
            if (displayMode == InputActionDisplayMode.SecondaryOnly || displayMode == InputActionDisplayMode.SecondaryThenPrimary && secondaryButton != Buttons.None) {
                var iconSet = gamePadIconSet ?? this.GetGamePadIconSet();

                if (iconSet != null && iconSet.TryGetSpriteIndex(secondaryButton, out var index)) {
                    spriteIndex = index;
                    spriteSheet = iconSet.SpriteSheet;
                    kerning = iconSet.GetKerning(secondaryButton);
                }
            }
            else if (primaryButton != Buttons.None) {
                var iconSet = this.GetGamePadIconSet();

                if (iconSet != null && iconSet.TryGetSpriteIndex(primaryButton, out var index)) {
                    spriteIndex = index;
                    spriteSheet = iconSet.SpriteSheet;
                    kerning = iconSet.GetKerning(primaryButton);
                }
            }
        }

        return spriteSheet != null && spriteIndex != null;
    }

    private GamePadIconSet? GetGamePadIconSet() {
        return this._game.InputSettings.DesiredGamePad switch {
            GamePadDisplay.X => this._game.Project.Fallbacks.GamePadXReference.PackagedAsset,
            GamePadDisplay.N => this._game.Project.Fallbacks.GamePadNReference.PackagedAsset,
            GamePadDisplay.S => this._game.Project.Fallbacks.GamePadSReference.PackagedAsset,
            _ => null
        };
    }

    private class EmptyInputActionIconResolver : IInputActionIconResolver {

        public void Initialize(IGame game) {
        }

        public bool TryGetIcon(
            InputAction action,
            InputDevice inputDevice,
            InputActionDisplayMode displayMode,
            [NotNullWhen(true)] out SpriteSheet? spriteSheet,
            [NotNullWhen(true)] out byte? spriteIndex,
            out int kerning) {
            spriteSheet = null;
            spriteIndex = null;
            kerning = 0;
            return false;
        }

        public bool TryGetIcon(
            InputAction action,
            InputDevice inputDevice,
            InputActionDisplayMode displayMode,
            GamePadIconSet? gamePadIconSet,
            KeyboardIconSet? keyboardIconSet,
            MouseButtonIconSet? mouseIconSet,
            [NotNullWhen(true)] out SpriteSheet? spriteSheet,
            [NotNullWhen(true)] out byte? spriteIndex,
            out int kerning) => this.TryGetIcon(action, inputDevice, displayMode, out spriteSheet, out spriteIndex, out kerning);
    }
}