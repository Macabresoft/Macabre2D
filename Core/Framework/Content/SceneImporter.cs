namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="Scene" />.
    /// </summary>
    /// <seealso cref="Macabresoft.MonoGame.Core.Content.JsonImporter" />
    [ContentImporter(".m2dscene", DefaultProcessor = nameof(SceneProcessor), DisplayName = "Scene Importer - Macabresoft.MonoGame.Core")]
    public sealed class SceneImporter : JsonImporter {
    }
}