namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

    /// <summary>
    /// Content type writer for <see cref="GameProject" />.
    /// </summary>
    [ContentTypeWriter]
    public sealed class GameProjectWriter : JsonWriter<GameProject> {

        /// <inheritdoc />
        public override string GetRuntimeReader(TargetPlatform targetPlatform) {
            return $"{typeof(GameProjectReader).FullName}, {nameof(Macabresoft)}.{nameof(Macabre2D)}.{nameof(Framework)}";
        }

        /// <inheritdoc />
        public override string GetRuntimeType(TargetPlatform targetPlatform) {
            return $"{typeof(GameSettings).FullName}, {nameof(Macabresoft)}.{nameof(Macabre2D)}.{nameof(Framework)}";
        }
    }
}