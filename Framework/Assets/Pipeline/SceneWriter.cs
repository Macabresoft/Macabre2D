namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

    /// <summary>
    /// Content type writer for <see cref="Scene" />.
    /// </summary>
    [ContentTypeWriter]
    public sealed class SceneWriter : JsonWriter<GameScene> {

        /// <inheritdoc />
        public override string GetRuntimeReader(TargetPlatform targetPlatform) {
            return $"{typeof(SceneReader).FullName}, {nameof(Macabresoft)}.{nameof(MonoGame)}.{nameof(Core)}";
        }

        /// <inheritdoc />
        public override string GetRuntimeType(TargetPlatform targetPlatform) {
            return $"{typeof(GameScene).FullName}, {nameof(Macabresoft)}.{nameof(MonoGame)}.{nameof(Core)}";
        }
    }
}