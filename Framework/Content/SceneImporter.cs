namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="Scene"/>.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.Content.JsonImporter"/>
    [ContentImporter(".m2dscene", DefaultProcessor = nameof(SceneProcessor), DisplayName = "Scene Importer - Macabre2D")]
    public sealed class SceneImporter : JsonImporter {
    }
}