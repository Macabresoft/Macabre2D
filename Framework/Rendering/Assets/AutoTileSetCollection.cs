namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Macabresoft.Core;

    /// <summary>
    /// An observable collection of <see cref="AutoTileSet" />.
    /// </summary>
    [DataContract]
    [Category("Auto Tile Sets")]
    public class AutoTileSetCollection : ObservableCollectionExtended<AutoTileSet>, INameableCollection {
        /// <inheritdoc />
        public string Name => "Auto Tile Sets";
        
        /// <inheritdoc />
        IEnumerator<INameable> IEnumerable<INameable>.GetEnumerator() {
            return this.Items.GetEnumerator();
        }
    }
}