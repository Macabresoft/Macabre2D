namespace Macabre2D.Framework;

/// <summary>
/// Interface for an entity that can be enabled.
/// </summary>
public interface IEnableable {
    /// <summary>
    /// Gets or sets a value indicating whether this instance is enabled.
    /// </summary>
    bool IsEnabled { get; set; }
}