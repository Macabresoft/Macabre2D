namespace Macabresoft.AvaloniaEx;

using System.Diagnostics;

/// <summary>
/// Interface for a service which wraps <see cref="Process" /> operations.
/// </summary>
public interface IProcessService {
    /// <summary>
    /// Starts the process described in the <see cref="ProcessStartInfo" />.
    /// </summary>
    /// <param name="startInfo">The info required to start the requested process.</param>
    /// <returns>The exit code of the process.</returns>
    int StartProcess(ProcessStartInfo startInfo);
}

/// <summary>
/// A service which wraps <see cref="Process" /> operations.
/// </summary>
public class ProcessService : IProcessService {
    public int StartProcess(ProcessStartInfo startInfo) {
        var exitCode = -1;
        using var process = Process.Start(startInfo);
        if (process != null) {
            process.WaitForExit();
            exitCode = process.ExitCode;
        }

        return exitCode;
    }
}