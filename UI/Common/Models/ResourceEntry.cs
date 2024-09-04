namespace Macabresoft.Macabre2D.UI.Common;

/// <summary>
/// Represents a string entry in a resource dictionary.
/// </summary>
/// <param name="Key">The key.</param>
/// <param name="Value">The value.</param>
public readonly record struct ResourceEntry(string Key, string Value) {
    /// <inheritdoc />
    public override string ToString() => $"{Key} ({Value})";
}