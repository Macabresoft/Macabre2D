namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="GameSettings" />.
    /// </summary>
    [ContentImporter(".m2dgs", DefaultProcessor = nameof(GameSettingsProcessor), DisplayName = "Game Settings Importer - Macabresoft.MonoGame.Core")]
    public sealed class GameSettingsImporter : JsonImporter {
    }
}