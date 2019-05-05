namespace Macabre2D.Framework.Content {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="GameSettings"/>.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.Content.JsonImporter"/>
    [ContentImporter(".m2dgs", DefaultProcessor = nameof(GameSettingsProcessor), DisplayName = "Game Settings Importer - Macabre2D")]
    public sealed class GameSettingsImporter : JsonImporter {
    }
}