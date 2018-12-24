namespace Macabre2D.Framework.Content {

    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

    /// <summary>
    /// Content type writer for <see cref="GameSettings"/>.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.Content.JsonWriter{Macabre2D.Framework.GameSettings}"/>
    [ContentTypeWriter]
    public sealed class GameSettingsWriter : JsonWriter<GameSettings> {

        /// <inheritdoc/>
        public override string GetRuntimeReader(TargetPlatform targetPlatform) {
            return $"{typeof(GameSettingsReader).FullName}, {nameof(Macabre2D)}.{nameof(Framework)}";
        }

        /// <inheritdoc/>
        public override string GetRuntimeType(TargetPlatform targetPlatform) {
            return $"{typeof(GameSettings).FullName}, {nameof(Macabre2D)}.{nameof(Framework)}";
        }
    }
}