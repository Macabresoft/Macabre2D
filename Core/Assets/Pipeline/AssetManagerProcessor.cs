namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="AssetManager" />.
    /// </summary>
    [ContentProcessor(DisplayName = "Asset Manager Processor - Macabresoft.MonoGame.Core")]
    public sealed class AssetManagerProcessor : JsonProcessor<AssetManager> {
    }
}