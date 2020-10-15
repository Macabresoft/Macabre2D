namespace Macabresoft.Macabre2D.Framework {

    using Microsoft.Xna.Framework.Content;

    /// <summary>
    /// Content reader for JSON.
    /// </summary>
    /// <typeparam name="T">The type being serialized.</typeparam>
    public class JsonReader<T> : ContentTypeReader<T> {
        private readonly Serializer _serializer = new Serializer();

        /// <inheritdoc />
        protected override T Read(ContentReader input, T existingInstance) {
            var json = input.ReadString();
            return this._serializer.DeserializeFromString<T>(json);
        }
    }
}