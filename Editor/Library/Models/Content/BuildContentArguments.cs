namespace Macabresoft.Macabre2D.Editor.Library.Models.Content {
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Arguments for building content using MGCB.
    /// </summary>
    public class BuildContentArguments {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildContentArguments" /> struct.
        /// </summary>
        /// <param name="contentFilePath">The content file path.</param>
        /// <param name="contentDirectoryPath">The content directory path.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="performCompression">if set to <c>true</c> MGCB will perform compression.</param>
        public BuildContentArguments(
            string contentFilePath,
            string contentDirectoryPath,
            string platform,
            bool performCompression) {
            this.ContentFilePath = contentFilePath;
            this.ContentDirectoryPath = contentDirectoryPath;
            this.Platform = platform;
            this.PerformCompression = performCompression;
        }

        /// <summary>
        /// Gets the content directory path.
        /// </summary>
        /// <value>The content directory path.</value>
        public string ContentDirectoryPath { get; }

        /// <summary>
        /// Gets the content file path.
        /// </summary>
        /// <value>The content file path.</value>
        public string ContentFilePath { get; }

        /// <summary>
        /// Gets a value indicating whether or not MGCB should perform compression.
        /// </summary>
        /// <value><c>true</c> if MGCB should perform compression; otherwise, <c>false</c>.</value>
        public bool PerformCompression { get; }

        /// <summary>
        /// Gets the platform.
        /// </summary>
        /// <value>The platform.</value>
        public string Platform { get; }

        /// <summary>
        /// Converts to console arguments used by MGCB.
        /// </summary>
        /// <returns>The console arguments.</returns>
        public string ToConsoleArguments() {
            var arguments = this.ToGlobalProperties(this.ContentDirectoryPath);
            arguments.Add("/rebuild");
            arguments.Add($"/@:\"{this.ContentFilePath}\"");
            return arguments.Aggregate((first, second) => $"{first} {second}");
        }

        /// <summary>
        /// Gets the lines for the global properties section of an MGCB file.
        /// </summary>
        /// <returns>The lines for the global properties section of an MGCB file.</returns>
        public IEnumerable<string> ToGlobalProperties() {
            return this.ToGlobalProperties(this.ContentDirectoryPath);
        }

        private IList<string> ToGlobalProperties(string projectDirectoryPath) {
            return new List<string> {
                $"/outputDir:\"{Path.Combine(projectDirectoryPath, "bin", this.Platform)}\"",
                $"/intermediateDir:\"{Path.Combine(projectDirectoryPath, "obj", this.Platform)}\"",
                $"/platform:{this.Platform}",
                "/config:",
                "/profile:Reach",
                $"/compress:{this.PerformCompression}"
            };
        }
    }
}