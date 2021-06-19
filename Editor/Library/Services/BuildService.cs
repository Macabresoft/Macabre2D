namespace Macabresoft.Macabre2D.UI.Library.Services {
    using Macabresoft.Core;
    using System.Diagnostics;
    using System.IO;
    using Macabresoft.Macabre2D.UI.Library.Models.Content;
    using System.Linq;

    /// <summary>
    /// Interface that abstracts out building content and projects.
    /// </summary>
    public interface IBuildService {
        /// <summary>
        /// Builds the content.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The exit code of the MGCB process.</returns>
        int BuildContent(BuildContentArguments args);

        /// <summary>
        /// Builds the content with an output directory in mind.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="outputDirectoryPath">The path to the output directory.</param>
        /// <returns>The exit code of the MGCB process.</returns>
        int BuildContent(BuildContentArguments args, string outputDirectoryPath);
    }

    /// <summary>
    /// Service which abstracts out building content and projects.
    /// </summary>
    public class BuildService : IBuildService {
        private readonly IFileSystemService _fileSystem;
        private readonly IProcessService _processService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildService" /> class.
        /// </summary>
        /// <param name="fileSystem">The file system service.</param>
        /// <param name="processService">The process service.</param>
        public BuildService(IFileSystemService fileSystem, IProcessService processService) {
            this._fileSystem = fileSystem;
            this._processService = processService;
        }

        /// <inheritdoc />
        public int BuildContent(BuildContentArguments args) {
            return this.BuildContent(args, null);
        }

        /// <inheritdoc />
        public int BuildContent(BuildContentArguments args, string outputDirectoryPath) {
            var exitCode = -1;
            if (!string.IsNullOrWhiteSpace(args.ContentFilePath) && this._fileSystem.DoesFileExist(args.ContentFilePath)) {
                var startInfo = new ProcessStartInfo {
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    FileName = "mgcb",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = Path.GetDirectoryName(args.ContentFilePath) ?? string.Empty
                };
                
                var arguments = !string.IsNullOrEmpty(outputDirectoryPath) ? 
                    args.ToConsoleArguments(outputDirectoryPath) : 
                    args.ToConsoleArguments();
                
                startInfo.ArgumentList.AddRange(arguments);
                exitCode = this._processService.StartProcess(startInfo);
            }

            return exitCode;
        }
    }
}