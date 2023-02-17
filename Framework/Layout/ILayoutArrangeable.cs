namespace Macabresoft.Macabre2D.Framework.Layout;

/// <summary>
/// Interface for an entity which can be arranged by a <see cref="ILayoutArranger" />.
/// </summary>
public interface ILayoutArrangeable : IBoundable {
    /// <summary>
    /// Gets the column for this arrangeable.
    /// </summary>
    /// <remarks>
    /// When arranging a horizontal stack panel, column will be used to determine order.
    /// </remarks>
    public byte Column { get; }

    /// <summary>
    /// Gets the row for this arrangeable.
    /// </summary>
    /// <remarks>
    /// When arranging a vertical stack panel, row will be used to determine order.
    /// </remarks>
    public byte Row { get; }
    
    /// <summary>
    /// Gets the number of rows this element spans.
    /// </summary>
    public byte RowSpan { get; }
    
    /// <summary>
    /// Gets the number of columns this element spans.
    /// </summary>
    public byte ColumnSpan { get; }

    /// <summary>
    /// Confines this instance to the specified <see cref="BoundingArea"/>.
    /// </summary>
    /// <param name="boundingArea">The bounding area.</param>
    public void ConfineToBounds(BoundingArea boundingArea);
}