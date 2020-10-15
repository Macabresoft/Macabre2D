namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="AssetManager" />.
    /// </summary>
    [ContentImporter(".m2dam", DefaultProcessor = nameof(AssetManagerProcessor), DisplayName = "Asset Manager Importer - Macabresoft.Macabre2D.Framework")]
    public sealed class AssetManagerImporter : JsonImporter {
    }
}