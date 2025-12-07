namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using Macabresoft.Core;

/// <summary>
/// A collection of <see cref="PhysicsMaterial" /> stored at the project level.
/// </summary>
public class PhysicsMaterialCollection : ObservableCollectionExtended<PhysicsMaterial>, IIndexedCollection, INameableCollection {

    /// <inheritdoc />
    public string Name => "Physics Materials";

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() => this.Items.GetEnumerator();

    /// <summary>
    /// Gets the physics material or the default value.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The physics material.</returns>
    public PhysicsMaterial Get(Guid id) {
        return this.Items.FirstOrDefault(x => x.Id == id) ?? PhysicsMaterial.Default;
    }
    
    /// <inheritdoc />
    public int IndexOfUntyped(object item) {
        if (item is PhysicsMaterial material) {
            return this.IndexOf(material);
        }

        return -1;
    }
}