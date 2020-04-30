namespace Macabre2D.UI.CommonLibrary.Models {

    using System.Collections.Generic;

    public interface ISyncAsset<T> {

        IEnumerable<T> GetAssetsToSync();
    }
}