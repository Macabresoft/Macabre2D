namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="AssetManager" />.
    /// </summary>
    [ContentProcessor(DisplayName = "Asset Manager Processor - Macabresoft.Macabre2D.Framework")]
    public sealed class AssetManagerProcessor : JsonProcessor<AssetManager> {
    }
}