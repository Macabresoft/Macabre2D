namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="AssetManager" />.
    /// </summary>
    /// <seealso cref="Macabresoft.MonoGame.Core.Content.JsonProcessor{Macabresoft.MonoGame.Core.AssetManager}" />
    [ContentProcessor(DisplayName = "Asset Manager Processor - Macabresoft.MonoGame.Core")]
    public sealed class AssetManagerProcessor : JsonProcessor<AssetManager> {
    }
}