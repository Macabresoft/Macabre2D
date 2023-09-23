namespace Macabresoft.Macabre2D.Framework;

using System;
using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// Event args for when a <see cref="Layers" /> has its name changed.
/// </summary>
/// <seealso cref="EventArgs" />
public sealed class LayerNameChangedEventArgs : EventArgs {
    /// <summary>
    /// The layer.
    /// </summary>
    public readonly Layers Layer;

    /// <summary>
    /// The name of the layer.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayerNameChangedEventArgs" /> class.
    /// </summary>
    /// <param name="layer">The layer.</param>
    /// <param name="name">The name.</param>
    public LayerNameChangedEventArgs(Layers layer, string name) {
        this.Layer = layer;
        this.Name = name;
    }
}