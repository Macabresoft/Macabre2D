namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="Scene" />.
    /// </summary>
    [ContentImporter(".m2dscene", DefaultProcessor = nameof(SceneProcessor), DisplayName = "Scene Importer - Macabresoft.MonoGame.Core")]
    public sealed class SceneImporter : JsonImporter {
    }
}