namespace Macabre2D.UI.CommonLibrary.Models {

    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.Framework;
    using MahApps.Metro.IconPacks;
    using System;
    using System.Collections.Generic;

    public sealed class PrefabAsset : AddableAsset<Prefab>, IIdentifiableAsset {

        public PrefabAsset() : this(string.Empty) {
        }

        public PrefabAsset(string name) : base(name) {
        }

        public override string FileExtension {
            get {
                return FileHelper.PrefabExtension;
            }
        }

        public override PackIconMaterialKind Icon {
            get {
                return PackIconMaterialKind.FilePowerpoint;
            }
        }

        public override void Delete() {
            this.RemoveIdentifiableContentFromScenes(this.SavableValue.Id);
            base.Delete();
        }

        public IEnumerable<Guid> GetOwnedAssetIds() {
            return new[] { this.SavableValue.Id };
        }
    }
}