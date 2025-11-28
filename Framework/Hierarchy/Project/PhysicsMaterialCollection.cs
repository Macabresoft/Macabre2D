namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using Macabresoft.Core;

/// <summary>
/// A collection of <see cref="PhysicsMaterial" /> stored at the project level.
/// </summary>
public class PhysicsMaterialCollection  : ObservableCollectionExtended<PhysicsMaterial>, INameableCollection, IEnumerable<PhysicsMaterial> {

    /// <inheritdoc />
    public string Name => "Physics Materials";

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() => this.Items.GetEnumerator();
}