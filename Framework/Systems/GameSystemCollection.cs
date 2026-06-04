namespace Macabre2D.Framework;

using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// An explicit observable collection of <see cref="IGameSystem" />.
/// </summary>
public class GameSystemCollection : ObservableCollection<IGameSystem>, IIndexedCollection, INameableCollection {
    /// <inheritdoc />
    public string Name => "Systems";

    /// <inheritdoc />
    public int IndexOfUntyped(object item) {
        if (item is IGameSystem system) {
            return this.IndexOf(system);
        }

        return -1;
    }

    /// <summary>
    /// Reorders the specified system.
    /// </summary>
    /// <param name="system">The system.</param>
    /// <param name="newIndex">The new index.</param>
    public void Reorder(IGameSystem system, int newIndex) {
        var originalIndex = this.IndexOf(system);
        this.MoveItem(originalIndex, newIndex);
    }

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() => this.Items.GetEnumerator();
}