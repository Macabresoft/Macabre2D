namespace Macabre2D.UI.Models {

    using MahApps.Metro.IconPacks;
    using System.Collections.Generic;

    public class NullAsset : Asset, IParent<Asset> {

        public NullAsset() : base("(None)") {
        }

        public NullAsset(string name) : base(name) {
        }

        public IReadOnlyCollection<Asset> Children { get; } = new List<Asset>();

        public override PackIconMaterialKind Icon {
            get {
                return PackIconMaterialKind.None;
            }
        }

        public bool AddChild(Asset child) {
            return false;
        }

        public bool RemoveChild(Asset child) {
            return false;
        }
    }
}