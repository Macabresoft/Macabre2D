namespace Macabresoft.Macabre2D.Framework.Layout; 

/// <summary>
/// Interface for an entity which can be arranged by a <see cref="ILayoutArranger"/>.
/// </summary>
public interface ILayoutArrangeable : IBoundable {
    
    /// <summary>
    /// Gets the row for this arrangeable.
    /// </summary>
    /// <remarks>
    /// When arranging a vertical stack panel, row will be used to determine order.
    /// </remarks>
    public byte Row { get; }
    
    /// <summary>
    /// Gets the column for this arrangeable.
    /// </summary>
    /// <remarks>
    /// When arranging a horizontal stack panel, column will be used to determine order.
    /// </remarks>
    public byte Column { get; }
}