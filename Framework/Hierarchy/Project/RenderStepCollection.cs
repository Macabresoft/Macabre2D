namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Linq;
using Macabresoft.Core;

/// <summary>
/// A collection of shaders stored at the project level.
/// </summary>
public class RenderStepCollection : ObservableCollectionExtended<RenderStep>, INameableCollection {

    /// <inheritdoc />
    public string Name => "Render Steps";

    /// <inheritdoc />
    IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() => this.Items.GetEnumerator();
}