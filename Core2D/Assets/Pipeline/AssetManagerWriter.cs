namespace Macabresoft.MonoGame.Core2D {

    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

    /// <summary>
    /// Content type writer for <see cref="AssetManager" />.
    /// </summary>
    [ContentTypeWriter]
    public sealed class AssetManagerWriter : JsonWriter<AssetManager> {

        /// <inheritdoc />
        public override string GetRuntimeReader(TargetPlatform targetPlatform) {
            return $"{typeof(AssetManagerReader).FullName}, {nameof(Macabresoft)}.{nameof(MonoGame)}.{nameof(Core)}";
        }

        /// <inheritdoc />
        public override string GetRuntimeType(TargetPlatform targetPlatform) {
            return $"{typeof(AssetManager).FullName}, {nameof(Macabresoft)}.{nameof(MonoGame)}.{nameof(Core)}";
        }
    }
}