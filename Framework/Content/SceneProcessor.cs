namespace Macabre2D.Framework.Content {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="Scene"/>.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.Content.JsonProcessor{Macabre2D.Framework.Scene}"/>
    [ContentProcessor(DisplayName = "Scene Processor - Macabre2D")]
    public sealed class SceneProcessor : JsonProcessor<Scene> {
    }
}