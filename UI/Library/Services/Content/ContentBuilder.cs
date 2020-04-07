namespace Macabre2D.UI.Library.Services.Content {

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public static class ContentBuilder {

        public static int BuildContent(out Exception exception, params string[] args) {
            var currentDirectory = Directory.GetCurrentDirectory();
            exception = null;
            var result = 0;

            try {
                var mgcbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"MSBuild\MonoGame\v3.0\Tools", "MGCB.exe");
                var startInfo = new ProcessStartInfo {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    FileName = mgcbPath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = string.Join(" ", args.Select(x => $"\"{x}\""))
                };

                using (var process = Process.Start(startInfo)) {
                    process.WaitForExit();
                    result = process.ExitCode;
                }
            }
            catch (Exception e) {
                exception = e;
                result = result == 0 ? -1 : result;
            }
            finally {
                Directory.SetCurrentDirectory(currentDirectory);
            }

            return result;
        }
    }
}