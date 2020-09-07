namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="GameSettings" />.
    /// </summary>
    [ContentProcessor(DisplayName = "Game Settings Processor - Macabresoft.MonoGame.Core")]
    public sealed class GameSettingsProcessor : JsonProcessor<GameSettings> {
    }
}