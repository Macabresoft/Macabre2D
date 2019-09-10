namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using MahApps.Metro.IconPacks;

    public sealed class PrefabAsset : AddableAsset<Prefab> {

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
    }
}