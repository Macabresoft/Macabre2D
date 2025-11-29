namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// Represents collections that can be moved by their index.
/// </summary>
public interface IIndexedCollection {

    /// <summary>
    /// Gets the index of the item without relying on type.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The item's index.</returns>
    int IndexOfUntyped(object item);

    /// <summary>
    /// Moves the specified item to the new index.
    /// </summary>
    /// <param name="originalIndex">The original index.</param>
    /// <param name="newIndex">The new index.</param>
    void Move(int originalIndex, int newIndex);
}