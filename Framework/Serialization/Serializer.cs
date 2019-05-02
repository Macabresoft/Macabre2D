namespace Macabre2D.Framework.Serialization {

    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Interface for a serializer to be used by the framework.
    /// </summary>
    public interface ISerializer {

        /// <summary>
        /// Deserializes a file at the specified path.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="path">The path.</param>
        /// <returns>A deserialized object from the specified file.</returns>
        T Deserialize<T>(string path);

        /// <summary>
        /// Deserializes from string.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="json">The json.</param>
        /// <returns>A deserialized object from the provided JSON.</returns>
        T DeserializeFromString<T>(string json);

        /// <summary>
        /// Deserializes from string.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="type">The type to deserialize to.</param>
        /// <returns>A deserialized object from the provided JSON.</returns>
        object DeserializeFromString(string json, Type type);

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="value">The thing to serialize.</param>
        /// <param name="path">The path.</param>
        void Serialize(object value, string path);

        /// <summary>
        /// Serializes the specified value to a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The JSON string of an object.</returns>
        string SerializeToString(object value);
    }

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
            this._jsonSerializer.TypeNameHandling = TypeNameHandling.Auto;
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
        public object DeserializeFromString(string json, Type type) {
            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader)) {
                return this._jsonSerializer.Deserialize(jsonReader, type);
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