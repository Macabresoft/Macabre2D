namespace Macabresoft.Macabre2D.Framework.Layout; 

/// <summary>
/// Interface for an object that arranges <see cref="ILayoutArrangeable"/>.
/// </summary>
public interface ILayoutArranger {
    /// <summary>
    /// Requests a rearrangement. This is called when size, row, or column changes on a <see cref="ILayoutArrangeable"/>.
    /// </summary>
    void RequestRearrange(ILayoutArrangeable requester);

    /// <summary>
    /// Gets the bounding area for a specific row and column.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <returns>The bounding area.</returns>
    BoundingArea GetBoundingArea(int row, int column);

    /// <summary>
    /// Gets the bounding area for a specific row, column, row span, and column span.
    /// </summary>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    /// <param name="rowSpan">The row span.</param>
    /// <param name="columnSpan">The column span.</param>
    /// <returns>The bounding area.</returns>
    BoundingArea GetBoundingArea(int row, int column, int rowSpan, int columnSpan);

    /// <summary>
    /// Gets the bounding area for a <see cref="ILayoutArrangeable"/>.
    /// </summary>
    /// <param name="arrangeable">The arrangeable entity.</param>
    /// <returns></returns>
    BoundingArea GetBoundingArea(ILayoutArrangeable arrangeable);
}