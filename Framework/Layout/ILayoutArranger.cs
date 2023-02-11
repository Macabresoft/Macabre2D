namespace Macabresoft.Macabre2D.Framework.Layout; 

/// <summary>
/// Interface for an object that arranges <see cref="ILayoutArrangeable"/>.
/// </summary>
public interface ILayoutArranger {
    /// <summary>
    /// Requests a rearrangement. This is called when size, row, or column changes on a <see cref="ILayoutArrangeable"/>.
    /// </summary>
    void RequestRearrange();

    /// <summary>
    /// Gets the bounding area for a specific row and column.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <returns>The bounding area.</returns>
    BoundingArea GetBoundingArea(byte row, byte column);
}