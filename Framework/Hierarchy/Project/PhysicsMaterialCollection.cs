namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using Macabresoft.Core;

/// <summary>
/// A collection of <see cref="PhysicsMaterial" /> stored at the project level.
/// </summary>
public class PhysicsMaterialCollection : ObservableCollectionExtended<PhysicsMaterial>, IIndexedCollection, INameableCollection {

    /// <inheritdoc />
    public string Name => "Physics Materials";

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() => this.Items.GetEnumerator();
    
    /// <inheritdoc />
    public int IndexOfUntyped(object item) {
        if (item is PhysicsMaterial material) {
            return this.IndexOf(material);
        }

        return -1;
    }
}