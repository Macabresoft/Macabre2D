namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="GameSettings" />.
    /// </summary>
    [ContentImporter(GameProject.ProjectFileExtension, DefaultProcessor = nameof(GameProjectProcessor), DisplayName = "Game Project Importer - Macabresoft.Macabre2D.Framework")]
    public sealed class GameProjectImporter : JsonImporter {
    }
}