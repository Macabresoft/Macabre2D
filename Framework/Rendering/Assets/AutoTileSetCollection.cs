namespace Macabresoft.Macabre2D.Framework {
    using System.Runtime.Serialization;
    using Macabresoft.Core;

    /// <summary>
    /// An observable collection of <see cref="AutoTileSet"/>.
    /// </summary>
    [DataContract]
    public class AutoTileSetCollection : ObservableCollectionExtended<AutoTileSet> {
    }
}