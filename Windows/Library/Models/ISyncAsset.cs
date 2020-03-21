namespace Macabre2D.Engine.Windows.Models {

    using System.Collections.Generic;

    public interface ISyncAsset<T> {

        IEnumerable<T> GetAssetsToSync();
    }
}