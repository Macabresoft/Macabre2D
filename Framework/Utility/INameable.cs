namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// Interface for something which can be named.
/// </summary>
public interface INameable {
    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; set; }

    /// <inheritdoc cref="object" />
    string? ToString() {
        return this.Name;
    }
}