namespace Macabre2D.Common;

using System.Text;
using Newtonsoft.Json;

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
    /// Deserializes a file from the specified stream.
    /// </summary>
    /// <typeparam name="T">The type to deserialize to.</typeparam>
    /// <param name="stream">The stream.</param>
    /// <returns>A deserialized object from the specified stream.</returns>
    T Deserialize<T>(Stream stream);

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
/// Serializes to JSON.
/// </summary>
public sealed class Serializer : ISerializer {
#nullable disable
    private readonly JsonSerializer _jsonSerializer;

    /// <summary>
    /// Initializes a new instance of the <see cref="Serializer" /> class.
    /// </summary>
    public Serializer() {
        this._jsonSerializer = new JsonSerializer {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            MaxDepth = 512,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            Formatting = Formatting.Indented,
            ObjectCreationHandling = ObjectCreationHandling.Auto,
            PreserveReferencesHandling = PreserveReferencesHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new CustomContractResolver()
        };

        this._jsonSerializer.Converters.Add(new JsonColorConverter());
    }

    /// <summary>
    /// Gets the singleton instance of <see cref="ISerializer" />.
    /// </summary>
    public static ISerializer Instance { get; } = new Serializer();

    /// <inheritdoc />
    public T Deserialize<T>(string path) {
        using var streamReader = new StreamReader(path);
        using var jsonReader = new JsonTextReader(streamReader);
        return this._jsonSerializer.Deserialize<T>(jsonReader);
    }

    /// <inheritdoc />
    public T Deserialize<T>(Stream stream) {
        using var streamReader = new StreamReader(stream);
        using var jsonReader = new JsonTextReader(streamReader);
        return this._jsonSerializer.Deserialize<T>(jsonReader);
    }

    /// <inheritdoc />
    public T DeserializeFromString<T>(string json) {
        using var stringReader = new StringReader(json);
        using var jsonReader = new JsonTextReader(stringReader);
        return this._jsonSerializer.Deserialize<T>(jsonReader);
    }

    /// <inheritdoc />
    public object DeserializeFromString(string json, Type type) {
        using var stringReader = new StringReader(json);
        using var jsonReader = new JsonTextReader(stringReader);
        return this._jsonSerializer.Deserialize(jsonReader, type) ?? new object();
    }

    /// <inheritdoc />
    public void Serialize(object value, string path) {
        var directoryName = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directoryName)) {
            Directory.CreateDirectory(directoryName);
        }

        using var streamWriter = new StreamWriter(path);
        using var jsonWriter = new JsonTextWriter(streamWriter);
        this._jsonSerializer.Serialize(jsonWriter, value);
    }

    /// <inheritdoc />
    public string SerializeToString(object value) {
        var stringBuilder = new StringBuilder();
        using (var stringWriter = new StringWriter(stringBuilder))
        using (var jsonWriter = new JsonTextWriter(stringWriter)) {
            this._jsonSerializer.Serialize(jsonWriter, value);
        }

        return stringBuilder.ToString();
    }
}