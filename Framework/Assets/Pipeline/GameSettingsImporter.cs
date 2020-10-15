namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="GameSettings" />.
    /// </summary>
    [ContentImporter(".m2dgs", DefaultProcessor = nameof(GameSettingsProcessor), DisplayName = "Game Settings Importer - Macabresoft.Macabre2D.Framework")]
    public sealed class GameSettingsImporter : JsonImporter {
    }
}