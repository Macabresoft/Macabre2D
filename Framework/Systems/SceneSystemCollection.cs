namespace Macabre2D.Framework;

using System.Collections.Generic;
using System.Collections.ObjectModel;

/// <summary>
/// An explicit observable collection of <see cref="ISceneSystem" />.
/// </summary>
public class SceneSystemCollection : ObservableCollection<ISceneSystem>, INameableCollection {
    /// <inheritdoc />
    public string Name => "Systems";

    /// <summary>
    /// Reorders the specified system.
    /// </summary>
    /// <param name="system">The system.</param>
    /// <param name="newIndex">The new index.</param>
    public void Reorder(ISceneSystem system, int newIndex) {
        var originalIndex = this.IndexOf(system);
        this.MoveItem(originalIndex, newIndex);
    }

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() => this.Items.GetEnumerator();
}