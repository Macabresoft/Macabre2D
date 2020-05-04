namespace Macabre2D.UI.GameEditorLibrary.Models {

    using System;
    using System.Collections.Generic;

    public interface IReloadableAsset {

        IEnumerable<Guid> GetOwnedAssetIds();

        void Reload();
    }
}