namespace Macabre2D.Framework.Serialization {

    using Newtonsoft.Json;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Serializes to Json.
    /// </summary>
    /// <seealso cref="ISerializationService"/>
    public sealed class Serializer : ISerializer {
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
            this._jsonSerializer.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
            this._jsonSerializer.Converters.Add(new JsonColorConverter());
        }

        /// <inheritdoc/>
        public T Deserialize<T>(string path) {
            using (var streamReader = new StreamReader(path))
            using (var jsonReader = new JsonTextReader(streamReader)) {
                return this._jsonSerializer.Deserialize<T>(jsonReader);
            }
        }

        /// <inheritdoc/>
        public T DeserializeFromString<T>(string json) {
            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader)) {
                return this._jsonSerializer.Deserialize<T>(jsonReader);
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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