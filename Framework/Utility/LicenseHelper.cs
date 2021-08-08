namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;

    /// <summary>
    /// A helper for licenses used by Macabre2D, including its editor.
    /// </summary>
    public static class LicenseHelper {
        /// <summary>
        /// Gets all license definitions.
        /// </summary>
        public static IReadOnlyCollection<LicenseDefinition> Definitions { get; } = new LicenseDefinition[] {
            new("Macabre2D", Licenses.Macabre2D),
            new("MonoGame", Licenses.MonoGame),
            new("Mono.Xna", Licenses.Mono_Xna)
        };
    }
}