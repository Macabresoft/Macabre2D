namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Macabresoft.Core;

    /// <summary>
    /// An observable collection of <see cref="AutoTileSet"/>.
    /// </summary>
    [DataContract]
    [Category("Auto Tile Sets")]
    public class AutoTileSetCollection : ObservableCollectionExtended<AutoTileSet> {
    }
}