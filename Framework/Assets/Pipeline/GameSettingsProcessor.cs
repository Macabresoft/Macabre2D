namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="GameSettings" />.
    /// </summary>
    [ContentProcessor(DisplayName = "Game Settings Processor - Macabresoft.Macabre2D.Framework")]
    public sealed class GameSettingsProcessor : JsonProcessor<GameSettings> {
    }
}