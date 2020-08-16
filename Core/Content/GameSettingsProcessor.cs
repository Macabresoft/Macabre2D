namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="GameSettings" />.
    /// </summary>
    /// <seealso cref="Macabresoft.MonoGame.Core.Content.JsonProcessor{Macabresoft.MonoGame.Core.GameSettings}" />
    [ContentProcessor(DisplayName = "Game Settings Processor - Macabresoft.MonoGame.Core")]
    public sealed class GameSettingsProcessor : JsonProcessor<GameSettings> {
    }
}