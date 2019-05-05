namespace Macabre2D.Framework.Content {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="AssetManager"/>.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.Content.JsonImporter"/>
    [ContentImporter(".m2dam", DefaultProcessor = nameof(AssetManagerProcessor), DisplayName = "Asset Manager Importer - Macabre2D")]
    public sealed class AssetManagerImporter : JsonImporter {
    }
}