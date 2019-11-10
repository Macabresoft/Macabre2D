namespace Macabre2D.Framework {

    using CosmicSynth.Framework;
    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// Content importer for <see cref="Song"/>.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.JsonImporter"/>
    [ContentImporter(".cosmicsong", DefaultProcessor = nameof(SongProcessor), DisplayName = "Song Importer - Cosmic Synth")]
    public sealed class SongImporter : JsonImporter {
    }
}