namespace Macabre2D.Framework.Content {

    using Microsoft.Xna.Framework.Content.Pipeline;

    /// <summary>
    /// A content processor for scenes.
    /// </summary>
    /// <typeparam name="T">The type being serialized.</typeparam>
    /// <seealso cref="Microsoft.Xna.Framework.Content.Pipeline.ContentProcessor{string, T}"/>
    public class JsonProcessor<T> : ContentProcessor<string, T> {
        private readonly Serializer _serializer = new Serializer();

        /// <inheritdoc/>
        public override T Process(string input, ContentProcessorContext context) {
            return this._serializer.DeserializeFromString<T>(input);
        }
    }
}