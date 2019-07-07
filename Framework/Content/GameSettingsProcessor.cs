namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="GameSettings"/>.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.Content.JsonProcessor{Macabre2D.Framework.GameSettings}"/>
    [ContentProcessor(DisplayName = "Game Settings Processor - Macabre2D")]
    public sealed class GameSettingsProcessor : JsonProcessor<GameSettings> {
    }
}