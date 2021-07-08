namespace Macabresoft.Macabre2D.UI.Common.Models.Content {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Macabresoft.Macabre2D.UI.Common.Services;

    /// <summary>
    /// Arguments for building content using MGCB.
    /// </summary>
    public class BuildContentArguments {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildContentArguments" /> struct.
        /// </summary>
        /// <param name="contentFilePath">The content file path.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="performCompression">if set to <c>true</c> MGCB will perform compression.</param>
        public BuildContentArguments(
            string contentFilePath,
            string platform,
            bool performCompression) {
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
        /// <returns>The console arguments.</returns>
        public IEnumerable<string> ToConsoleArguments() {
            return this.ToConsoleArguments(PathService.BinDirectoryName);
        }

        /// <summary>
        /// Converts to console arguments used by MGCB.
        /// </summary>
        /// <param name="outputDirectoryPath">The path to the output directory.</param>
        /// <returns>The console arguments.</returns>
        public IEnumerable<string> ToConsoleArguments(string outputDirectoryPath) {
            var arguments = this.ToGlobalPropertiesInternal(outputDirectoryPath);
            arguments.Add("/rebuild");
            arguments.Add($"/@:{Path.GetFileName(this.ContentFilePath)}");
            return arguments;
        }

        /// <summary>
        /// Gets the lines for the global properties section of an MGCB file.
        /// </summary>
        /// <returns>The lines for the global properties section of an MGCB file.</returns>
        public IEnumerable<string> ToGlobalProperties() {
            return this.ToGlobalPropertiesInternal(PathService.BinDirectoryName);
        }
        
        /// <summary>
        /// Gets the lines for the global properties section of an MGCB file.
        /// </summary>
        /// <returns>The lines for the global properties section of an MGCB file.</returns>
        public IEnumerable<string> ToGlobalProperties(string outputDirectory) {
            return this.ToGlobalPropertiesInternal(outputDirectory);
        }

        private IList<string> ToGlobalPropertiesInternal(string outputDirectory) {
            return new List<string> {
                $"/outputDir:{outputDirectory}",
                $"/intermediateDir:{PathService.ObjDirectoryName}",
                $"/platform:{this.Platform}",
                "/config:",
                "/profile:Reach",
                $"/compress:{this.PerformCompression}"
            };
        }
    }
}