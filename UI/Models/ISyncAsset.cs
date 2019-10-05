namespace Macabre2D.UI.Models {

    using System.Collections.Generic;

    public interface ISyncAsset<T> {

        IEnumerable<T> GetAssetsToSync();
    }
}