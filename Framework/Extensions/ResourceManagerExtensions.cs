namespace Macabre2D.Framework;

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using Macabre2D.Project.Common;

/// <summary>
/// Extension methods for <see cref="ResourceManager" />.
/// </summary>
public static class ResourceManagerExtensions {
    /// <param name="manager">The manager.</param>
    extension(ResourceManager manager) {
        /// <summary>
        /// Tries to get a resource string from a <see cref="ResourceManager" />.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="resource">The resource.</param>
        /// <returns>A value indicating whether the resource was found.</returns>
        public bool TryGetString(string resourceName, [NotNullWhen(true)] out string? resource) {
            resource = manager.GetString(resourceName, Resources.Culture);
            return resource != null;
        }

        /// <summary>
        /// Tries to get a resource string from a <see cref="ResourceManager" />.
        /// </summary>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="resource">The resource.</param>
        /// <returns>A value indicating whether the resource was found.</returns>
        public bool TryGetString(string resourceName, ResourceCulture culture, [NotNullWhen(true)] out string? resource) {
            var cultureInfo = CultureInfo.GetCultureInfo(culture.ToCultureName());
            resource = manager.GetString(resourceName, cultureInfo);
            return resource != null;
        }
    }
}