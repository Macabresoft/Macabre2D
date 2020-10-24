using System.IO;
using System.Linq;

namespace Macabresoft.Macabre2D.Editor.Framework.Models.Content {

    /// <summary>
    /// Arguments for building content using MGCB.
    /// </summary>
    public class BuildContentArguments {

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildContentArguments" /> struct.
        /// </summary>
        /// <param name="platform">The platform.</param>
        /// <param name="performCompression">if set to <c>true</c> MGCB will perform compression.</param>
        public BuildContentArguments(string contentFilePath, string platform, bool performCompression) {
            this.ContentFilePath = contentFilePath;
            this.Platform = platform;
            this.PerformCompression = performCompression;
        }

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
        /// <returns></returns>
        public string ToConsoleArguments() {
            var contentPath = Path.GetDirectoryName(this.ContentFilePath);

            var arguments = new[] {
                $"/outputDir:\"{Path.Combine(contentPath, "bin", this.Platform)}\"",
                $"/intermediateDir:\"{Path.Combine(contentPath, "obj", this.Platform)}\"",
                $"/platform:{this.Platform}",
                "/config:",
                "/profile:Reach",
                $"/compress:{this.PerformCompression}",
                "/rebuild",
                $"/@:\"{this.ContentFilePath}\""
            };

            return arguments.Aggregate((first, second) => $"{first} {second}");
        }
    }
}