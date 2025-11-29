namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using Macabresoft.Core;

/// <summary>
/// A collection of <see cref="IRenderStep" /> stored at the project level.
/// </summary>
public class RenderStepCollection : ObservableCollectionExtended<IRenderStep>, IIndexedCollection, INameableCollection {

    /// <inheritdoc />
    public string Name => "Render Steps";

    /// <inheritdoc />
    public int IndexOfUntyped(object item) {
        if (item is IRenderStep step) {
            return this.IndexOf(step);
        }

        return -1;
    }

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() => this.Items.GetEnumerator();
}