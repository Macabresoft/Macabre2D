using System.Diagnostics;

namespace Macabresoft.Macabre2D.Editor.Framework.Services {

    /// <summary>
    /// Interface for a service that handles MonoGame content for the editor.
    /// </summary>
    public interface IContentService {

        /// <summary>
        /// Builds the content.
        /// </summary>
        /// <param name="isDebug">if set to <c>true</c>, use debug.</param>
        /// <returns>The exit code of the MGCB process.</returns>
        int Build(bool isDebug);
    }

    /// <summary>
    /// A service that handles MonoGame content for the editor.
    /// </summary>
    public sealed class ContentService : IContentService {

        /// <inheritdoc />
        public int Build(bool isDebug) {
            var exitCode = -1;

            // TODO: replace this with a real call to mgcb and not just a help call
            var startInfo = new ProcessStartInfo() {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = "mgcb",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = "--help"
            };

            using (var process = Process.Start(startInfo)) {
                process.WaitForExit();
                exitCode = process.ExitCode;
            }

            return exitCode;
        }
    }
}