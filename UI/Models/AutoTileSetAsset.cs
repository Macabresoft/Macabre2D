namespace Macabre2D.UI.Models {

    using Macabre2D.Framework;
    using Macabre2D.UI.Common;
    using Macabre2D.UI.Models.FrameworkWrappers;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class AutoTileSetAsset : AddableAsset<AutoTileSet> {
        private readonly ObservableRangeCollection<SpriteWrapper> _sprites = new ObservableRangeCollection<SpriteWrapper>();

        public override string FileExtension {
            get {
                return FileHelper.AutoTileSetExtension;
            }
        }

        public IReadOnlyCollection<SpriteWrapper> Sprites {
            get {
                return this._sprites;
            }
        }

        public override AssetType Type {
            get {
                return AssetType.AutoTileSet;
            }
        }

        public override void Refresh(AssetManager assetManager) {
            base.Refresh(assetManager);

            var spritesToAdd = new SpriteWrapper[this.SavableValue.Size];
            var root = this.GetRootFolder();
            var spriteWrappers = root.GetAssetsOfType<SpriteWrapper>();

            for (byte i = 0; i < spritesToAdd.Length; i++) {
                var sprite = this.SavableValue.GetSprite(i);
                if (sprite != null) {
                    spritesToAdd[i] = spriteWrappers.FirstOrDefault(x => x.Sprite.Id == sprite.Id);
                }
            }

            this._sprites.Reset(spritesToAdd);
        }

        public void RemoveSprite(byte index) {
            this.SetSprite(null, index);
        }

        public void SetSprite(SpriteWrapper sprite, byte index) {
            if (index < this.SavableValue.Size) {
                this._sprites.Replace(sprite, index);
            }
        }
    }
}