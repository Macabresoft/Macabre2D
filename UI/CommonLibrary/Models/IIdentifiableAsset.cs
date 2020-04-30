namespace Macabre2D.UI.CommonLibrary.Models {

    using System;
    using System.Collections.Generic;

    public interface IIdentifiableAsset {

        IEnumerable<Guid> GetOwnedAssetIds();
    }
}