namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="ContentMetadata" />.
    /// </summary>
    [ContentProcessor(DisplayName = "Metadata Processor - Macabresoft.Macabre2D.Framework")]
    public sealed class MetadataProcessor : JsonProcessor<ContentMetadata> {
    }
}