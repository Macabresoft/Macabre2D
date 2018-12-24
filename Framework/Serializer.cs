namespace Macabre2D.Framework {

    using Newtonsoft.Json;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Serializes to Json.
    /// </summary>
    /// <seealso cref="ISerializationService"/>
    public sealed class Serializer {
        private readonly JsonSerializer _jsonSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Serializer"/> class.
        /// </summary>
        public Serializer() : this(new JsonSerializer()) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Serializer"/> class.
        /// </summary>
        /// <param name="jsonSerializer">The json serializer.</param>
        public Serializer(JsonSerializer jsonSerializer) {
            this._jsonSerializer = jsonSerializer;
            this._jsonSerializer.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            this._jsonSerializer.MissingMemberHandling = MissingMemberHandling.Ignore;
            this._jsonSerializer.Formatting = Formatting.Indented;
            this._jsonSerializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
            this._jsonSerializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            this._jsonSerializer.TypeNameHandling = TypeNameHandling.All;
        }

        /// <summary>
        /// Deserializes a file at the specified path.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="path">The path.</param>
        /// <returns>A deserialized object from the specified file.</returns>
        public T Deserialize<T>(string path) {
            using (var streamReader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(streamReader)) {
                return this._jsonSerializer.Deserialize<T>(jsonReader);
            }
        }

        /// <summary>
        /// Deserializes from string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <returns>A deserialized object from the provided JSON.</returns>
        public T DeserializeFromString<T>(string json) {
            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader)) {
                return this._jsonSerializer.Deserialize<T>(jsonReader);
            }
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="value">The thing to serialize.</param>
        /// <param name="path">The path.</param>
        public void Serialize(object value, string path) {
            var directoryName = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directoryName)) {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }

            using (var streamWriter = new StreamWriter(path))
            using (var jsonWriter = new JsonTextWriter(streamWriter)) {
                this._jsonSerializer.Serialize(jsonWriter, value);
            }
        }

        /// <summary>
        /// Serializes the specified value to a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The JSON string of an object.</returns>
        public string SerializeToString(object value) {
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder))
            using (var jsonWriter = new JsonTextWriter(stringWriter)) {
                this._jsonSerializer.Serialize(jsonWriter, value);
            }

            return stringBuilder.ToString();
        }
    }
}