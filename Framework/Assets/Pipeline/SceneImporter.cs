namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="Scene" />.
    /// </summary>
    [ContentImporter(".m2dscene", DefaultProcessor = nameof(SceneProcessor), DisplayName = "Scene Importer - Macabresoft.Macabre2D.Framework")]
    public sealed class SceneImporter : JsonImporter {
    }
}