namespace Macabresoft.MonoGame.Core2D {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="GameSettings" />.
    /// </summary>
    [ContentProcessor(DisplayName = "Game Settings Processor - Macabresoft.MonoGame.Core2D")]
    public sealed class GameSettingsProcessor : JsonProcessor<GameSettings> {
    }
}