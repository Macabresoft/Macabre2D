namespace Macabre2D.UI.Library.Models {

    using System.Collections.Generic;

    public interface ISyncAsset<T> {

        IEnumerable<T> GetAssetsToSync();
    }
}