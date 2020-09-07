namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="Scene" />.
    /// </summary>
    /// <seealso cref="Macabresoft.MonoGame.Core.Content.JsonProcessor{Macabresoft.MonoGame.Core.Scene}" />
    [ContentProcessor(DisplayName = "Scene Processor - Macabresoft.MonoGame.Core")]
    public sealed class SceneProcessor : JsonProcessor<GameScene> {
    }
}