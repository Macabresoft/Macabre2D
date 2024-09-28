namespace Macabresoft.Macabre2D.Framework;

using System;

/// <summary>
/// Helper methods for <see cref="Action" />.
/// </summary>
public static class ActionHelpers {
    /// <summary>
    /// Gets an empty <see cref="Action" />.
    /// </summary>
    public static Action EmptyAction { get; } = () => { };
}