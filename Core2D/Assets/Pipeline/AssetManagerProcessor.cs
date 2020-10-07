namespace Macabresoft.MonoGame.Core2D {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="AssetManager" />.
    /// </summary>
    [ContentProcessor(DisplayName = "Asset Manager Processor - Macabresoft.MonoGame.Core2D")]
    public sealed class AssetManagerProcessor : JsonProcessor<AssetManager> {
    }
}