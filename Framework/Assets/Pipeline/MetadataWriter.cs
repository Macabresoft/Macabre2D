namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

    /// <summary>
    /// Content type writer for <see cref="ContentMetadata" />.
    /// </summary>
    [ContentTypeWriter]
    public sealed class MetadataWriter : JsonWriter<ContentMetadata> {

        /// <inheritdoc />
        public override string GetRuntimeReader(TargetPlatform targetPlatform) {
            return $"{typeof(MetadataReader).FullName}, {nameof(Macabresoft)}.{nameof(Macabre2D)}.{nameof(Framework)}";
        }

        /// <inheritdoc />
        public override string GetRuntimeType(TargetPlatform targetPlatform) {
            return $"{typeof(MetadataReader).FullName}, {nameof(Macabresoft)}.{nameof(Macabre2D)}.{nameof(Framework)}";
        }
    }
}