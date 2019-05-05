namespace Macabre2D.Framework.Content {

    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

    /// <summary>
    /// Content type writer for <see cref="AssetManager"/>.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.Content.JsonWriter{Macabre2D.Framework.AssetManager}"/>
    [ContentTypeWriter]
    public sealed class AssetManagerWriter : JsonWriter<AssetManager> {

        /// <inheritdoc/>
        public override string GetRuntimeReader(TargetPlatform targetPlatform) {
            return $"{typeof(AssetManagerReader).FullName}, {nameof(Macabre2D)}.{nameof(Framework)}";
        }

        /// <inheritdoc/>
        public override string GetRuntimeType(TargetPlatform targetPlatform) {
            return $"{typeof(AssetManager).FullName}, {nameof(Macabre2D)}.{nameof(Framework)}";
        }
    }
}