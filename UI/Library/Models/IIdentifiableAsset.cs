namespace Macabre2D.UI.Library.Models {

    using System;
    using System.Collections.Generic;

    public interface IIdentifiableAsset {

        IEnumerable<Guid> GetOwnedAssetIds();
    }
}