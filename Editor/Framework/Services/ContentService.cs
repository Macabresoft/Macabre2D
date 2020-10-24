using Macabresoft.Macabre2D.Editor.Framework.Models.Content;
using System.Diagnostics;
using System.IO;

namespace Macabresoft.Macabre2D.Editor.Framework.Services {

    /// <summary>
    /// Interface for a service that handles MonoGame content for the editor.
    /// </summary>
    public interface IContentService {

        /// <summary>
        /// Builds the content.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The exit code of the MGCB process.</returns>
        int Build(BuildContentArguments args);
    }

    /// <summary>
    /// A service that handles MonoGame content for the editor.
    /// </summary>
    public sealed class ContentService : IContentService {

        /// <inheritdoc />
        public int Build(BuildContentArguments args) {
            var exitCode = -1;

            var startInfo = new ProcessStartInfo() {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = "mgcb",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = args.ToConsoleArguments(),
                WorkingDirectory = Path.GetDirectoryName(args.ContentFilePath)
            };

            using (var process = Process.Start(startInfo)) {
                process.WaitForExit();
                exitCode = process.ExitCode;
            }

            return exitCode;
        }
    }
}