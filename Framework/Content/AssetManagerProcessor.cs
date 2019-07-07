namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="AssetManager"/>.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.Content.JsonProcessor{Macabre2D.Framework.AssetManager}"/>
    [ContentProcessor(DisplayName = "Asset Manager Processor - Macabre2D")]
    public sealed class AssetManagerProcessor : JsonProcessor<AssetManager> {
    }
}