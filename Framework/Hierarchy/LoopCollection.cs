namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// An explicit observable collection of <see cref="ILoop" />.
/// </summary>
public class LoopCollection : ObservableCollection<ILoop>, INameableCollection {
    /// <inheritdoc />
    public string Name => "Loops";

    /// <summary>
    /// Reorders the specified loop.
    /// </summary>
    /// <param name="loop">The loop.</param>
    /// <param name="newIndex">The new index.</param>
    public void Reorder(ILoop loop, int newIndex) {
        var originalIndex = this.IndexOf(loop);
        this.MoveItem(originalIndex, newIndex);
    }

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() {
        return this.Items.GetEnumerator();
    }
}