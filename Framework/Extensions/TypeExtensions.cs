namespace Macabre2D.Framework;

using System;
using System.Reflection;
using System.Runtime.Serialization;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions {
    /// <summary>
    /// Converts a <see cref="Type"/> into a presentable name.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>A presentable name.</returns>
    public static string ToPresentableName(this Type type) {
        if (type.GetCustomAttribute(typeof(DataContractAttribute)) is DataContractAttribute attribute) {
            return string.IsNullOrEmpty(attribute.Name) ? type.Name : attribute.Name;
        }

        return type.Name;
    }
}