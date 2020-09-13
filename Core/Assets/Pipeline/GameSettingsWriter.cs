namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

    /// <summary>
    /// Content type writer for <see cref="GameSettings" />.
    /// </summary>
    [ContentTypeWriter]
    public sealed class GameSettingsWriter : JsonWriter<GameSettings> {

        /// <inheritdoc />
        public override string GetRuntimeReader(TargetPlatform targetPlatform) {
            return $"{typeof(GameSettingsReader).FullName}, {nameof(Macabresoft)}.{nameof(MonoGame)}.{nameof(Core)}";
        }

        /// <inheritdoc />
        public override string GetRuntimeType(TargetPlatform targetPlatform) {
            return $"{typeof(GameSettings).FullName}, {nameof(Macabresoft)}.{nameof(MonoGame)}.{nameof(Core)}";
        }
    }
}