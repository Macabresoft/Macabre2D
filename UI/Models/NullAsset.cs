namespace Macabre2D.UI.Models {

    using System.Collections.Generic;

    public class NullAsset : Asset, IParent<Asset> {

        public NullAsset() : base("(None)") {
        }

        public NullAsset(string name) : base(name) {
        }

        public IReadOnlyCollection<Asset> Children { get; } = new List<Asset>();

        public override AssetType Type {
            get {
                return AssetType.Folder;
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