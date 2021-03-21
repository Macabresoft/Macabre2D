namespace Macabresoft.Macabre2D.Framework {
    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="ContentMetadata" />.
    /// </summary>
    [ContentImporter(ContentMetadata.FileExtension, DefaultProcessor = nameof(MetadataProcessor), DisplayName = "Metadata Importer - Macabresoft.Macabre2D.Framework")]
    public sealed class MetadataImporter : JsonImporter {
    }
}