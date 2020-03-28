namespace Macabre2D.Engine.Windows.Models {

    using System;
    using System.Collections.Generic;

    public interface IIdentifiableAsset {

        IEnumerable<Guid> GetOwnedAssetIds();
    }
}