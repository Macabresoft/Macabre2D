namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

    /// <summary>
    /// Content type writer for <see cref="Scene"/>.
    /// </summary>
    /// <seealso cref="Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler.ContentTypeWriter{Scene}"/>
    [ContentTypeWriter]
    public sealed class SceneWriter : JsonWriter<Scene> {

        /// <inheritdoc/>
        public override string GetRuntimeReader(TargetPlatform targetPlatform) {
            return $"{typeof(SceneReader).FullName}, {nameof(Macabre2D)}.{nameof(Framework)}";
        }

        /// <inheritdoc/>
        public override string GetRuntimeType(TargetPlatform targetPlatform) {
            return $"{typeof(Scene).FullName}, {nameof(Macabre2D)}.{nameof(Framework)}";
        }
    }
}