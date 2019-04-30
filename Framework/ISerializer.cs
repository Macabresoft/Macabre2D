using System;

namespace Macabre2D.Framework {

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
}