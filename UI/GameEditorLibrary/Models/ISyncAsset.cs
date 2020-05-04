namespace Macabre2D.UI.GameEditorLibrary.Models {

    using System.Collections.Generic;

    public interface ISyncAsset<T> {

        IEnumerable<T> GetAssetsToSync();
    }
}