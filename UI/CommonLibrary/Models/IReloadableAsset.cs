namespace Macabre2D.UI.CommonLibrary.Models {

    using System;
    using System.Collections.Generic;

    public interface IReloadableAsset {

        IEnumerable<Guid> GetOwnedAssetIds();

        void Reload();
    }
}