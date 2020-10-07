namespace Macabresoft.MonoGame.Core2D {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="Scene" />.
    /// </summary>
    [ContentImporter(".m2dscene", DefaultProcessor = nameof(SceneProcessor), DisplayName = "Scene Importer - Macabresoft.MonoGame.Core2D")]
    public sealed class SceneImporter : JsonImporter {
    }
}