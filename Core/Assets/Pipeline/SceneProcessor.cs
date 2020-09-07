namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="Scene" />.
    /// </summary>
    [ContentProcessor(DisplayName = "Scene Processor - Macabresoft.MonoGame.Core")]
    public sealed class SceneProcessor : JsonProcessor<GameScene> {
    }
}