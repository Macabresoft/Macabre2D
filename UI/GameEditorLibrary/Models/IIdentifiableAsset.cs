namespace Macabre2D.UI.GameEditorLibrary.Models {

    using System;
    using System.Collections.Generic;

    public interface IIdentifiableAsset {

        IEnumerable<Guid> GetOwnedAssetIds();
    }
}