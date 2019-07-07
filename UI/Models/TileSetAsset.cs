namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
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

            set {
                this.SavableValue.SetSprites(value);
                this.RaisePropertyChanged(nameof(this.Sprites));
            }
        }

        public override AssetType Type {
            get {
                return AssetType.TileSet;
            }
        }
    }
}