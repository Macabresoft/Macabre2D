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
}