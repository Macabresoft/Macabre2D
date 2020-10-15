namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="Scene" />.
    /// </summary>
    [ContentProcessor(DisplayName = "Scene Processor - Macabresoft.Macabre2D.Framework")]
    public sealed class SceneProcessor : JsonProcessor<GameScene> {
    }
}