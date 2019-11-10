namespace Macabre2D.Framework {

    using CosmicSynth.Framework;
    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

    /// <summary>
    /// Content type writer for <see cref="Song"/>.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.JsonWriter{CosmicSynth.Framework.Song}"/>
    [ContentTypeWriter]
    public sealed class SongWriter : JsonWriter<Song> {

        /// <inheritdoc/>
        public override string GetRuntimeReader(TargetPlatform targetPlatform) {
            return $"{typeof(SongReader).FullName}, {nameof(Macabre2D)}.{nameof(Framework)}";
        }

        /// <inheritdoc/>
        public override string GetRuntimeType(TargetPlatform targetPlatform) {
            return $"{typeof(Song).FullName}, {nameof(CosmicSynth)}.{nameof(Framework)}";
        }
    }
}