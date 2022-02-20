namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// An explicit observable collection of <see cref="ILoopSystem" />.
/// </summary>
public class SystemCollection : ObservableCollection<ILoopSystem>, INameableCollection {
    /// <inheritdoc />
    public string Name => "Systems";

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() {
        return this.Items.GetEnumerator();
    }

    /// <summary>
    /// Reorders the specified system.
    /// </summary>
    /// <param name="system">The system.</param>
    /// <param name="newIndex">The new index.</param>
    public void Reorder(ILoopSystem system, int newIndex) {
        var originalIndex = this.IndexOf(system);
        this.MoveItem(originalIndex, newIndex);
    }
}