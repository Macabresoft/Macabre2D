namespace Macabre2D.UI.Models {

    using Macabre2D.Framework.Rendering;
    using Macabre2D.UI.Common;

    public sealed class TileSetAsset : AddableAsset<TileSet> {

        public override string FileExtension {
            get {
                return FileHelper.TileSetExtension;
            }
        }

        public Sprite[,] Sprites {
            get {
                return this.SavableValue.Sprites;
            }
        }

        public override AssetType Type {
            get {
                return AssetType.TileSet;
            }
        }
    }
}