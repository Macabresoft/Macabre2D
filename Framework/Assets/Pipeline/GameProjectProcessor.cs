namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="GameProject" />.
    /// </summary>
    [ContentProcessor(DisplayName = "Game Project Processor - Macabresoft.Macabre2D.Framework")]
    public sealed class GameProjectProcessor : JsonProcessor<GameProject> {
    }
}