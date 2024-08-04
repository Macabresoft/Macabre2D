namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// An explicit observable collection of <see cref="IGameSystem" />.
/// </summary>
public class SystemCollection : ObservableCollection<IGameSystem>, INameableCollection {
    /// <inheritdoc />
    public string Name => "Systems";

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
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() {
        return this.Items.GetEnumerator();
    }
}