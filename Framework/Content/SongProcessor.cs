namespace Macabre2D.Framework {

    using CosmicSynth.Framework;
    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for <see cref="Song"/>.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.JsonProcessor{CosmicSynth.Framework.Song}"/>
    [ContentProcessor(DisplayName = "Song Processor - Cosmic Synth")]
    public sealed class SongProcessor : JsonProcessor<Song> {
    }
}