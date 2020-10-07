namespace Macabresoft.MonoGame.Core2D {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="Scene" />.
    /// </summary>
    [ContentProcessor(DisplayName = "Scene Processor - Macabresoft.MonoGame.Core2D")]
    public sealed class SceneProcessor : JsonProcessor<GameScene> {
    }
}