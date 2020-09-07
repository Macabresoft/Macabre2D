namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="AssetManager" />.
    /// </summary>
    /// <seealso cref="Macabresoft.MonoGame.Core.Content.JsonImporter" />
    [ContentImporter(".m2dam", DefaultProcessor = nameof(AssetManagerProcessor), DisplayName = "Asset Manager Importer - Macabresoft.MonoGame.Core")]
    public sealed class AssetManagerImporter : JsonImporter {
    }
}