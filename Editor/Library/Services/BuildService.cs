namespace Macabresoft.Macabre2D.Editor.Library.Services {
    using System.Diagnostics;
    using System.IO;
    using Macabresoft.Macabre2D.Editor.Library.Models.Content;

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
            var exitCode = -1;
            if (!string.IsNullOrWhiteSpace(args.ContentFilePath) && this._fileSystem.DoesFileExist(args.ContentFilePath)) {
                var startInfo = new ProcessStartInfo {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = "mgcb",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = args.ToConsoleArguments(),
                    WorkingDirectory = Path.GetDirectoryName(args.ContentFilePath) ?? string.Empty
                };

                exitCode = this._processService.StartProcess(startInfo);
            }

            return exitCode;
        }
    }
}