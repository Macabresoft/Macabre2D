namespace Macabresoft.Macabre2D.Framework {
    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="GameScene" />.
    /// </summary>
    [ContentImporter(SceneAsset.FileExtension, DefaultProcessor = nameof(SceneProcessor), DisplayName = "Scene Importer - Macabresoft.Macabre2D.Framework")]
    public sealed class SceneImporter : JsonImporter {
    }
}