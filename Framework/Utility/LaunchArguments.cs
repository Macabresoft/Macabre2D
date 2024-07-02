namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;

/// <summary>
/// A set of standard launch arguments for any game in the Macabre2D engine.
/// </summary>
[Flags]
public enum LaunchArguments {
    None = 0,
    DebugMode = 1 << 0,
    EditorMode = 1 << 1
}

/// <summary>
/// Helper for <see cref="LaunchArguments"/>.
/// </summary>
internal static class LaunchArgumentHelpers {
    private static readonly HashSet<string> DebugKeys = [
        "-d",
        "-debug"
    ];

    private static readonly HashSet<string> EditorKeys = [
        "-e",
        "-editor",
        "-edit"
    ];
    
    /// <summary>
    /// Converts a list of <see cref="string"/> arguments into <see cref="LaunchArguments"/>.
    /// </summary>
    /// <param name="arguments">The arguments as a <see cref="string"/> array.</param>
    /// <returns>The arguments as <see cref="LaunchArguments"/>.</returns>
    public static LaunchArguments ToLaunchArguments(this string[] arguments) {
        var result = LaunchArguments.None;
        foreach (var argument in arguments) {
            if (DebugKeys.Contains(argument.ToLower())) {
                result |= LaunchArguments.DebugMode;
            }
            else if (EditorKeys.Contains(argument.ToLower())) {
                result |= LaunchArguments.EditorMode;
            }
        }

        return result;
    }
}