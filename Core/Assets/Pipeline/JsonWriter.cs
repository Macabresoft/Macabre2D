namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Content.Pipeline;
    using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

    /// <summary>
    /// A content writer for JSON.
    /// </summary>
    /// <typeparam name="T">The type being serialized.</typeparam>
    public abstract class JsonWriter<T> : ContentTypeWriter<T> {
        private readonly Serializer _serializer = new Serializer();

        /// <inheritdoc />
        public override string GetRuntimeType(TargetPlatform targetPlatform) {
            return typeof(T).AssemblyQualifiedName;
        }

        /// <inheritdoc />
        protected override void Write(ContentWriter output, T value) {
            output.Write(this._serializer.SerializeToString(value));
        }
    }
}