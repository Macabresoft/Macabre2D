namespace Macabre2D.UI.GameEditorLibrary.Models {

    using Macabre2D.Framework;
    using Macabre2D.UI.CommonLibrary.Common;
    using Macabre2D.UI.GameEditorLibrary.Models.FrameworkWrappers;
    using MahApps.Metro.IconPacks;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public sealed class AutoTileSetAsset : AddableAsset<AutoTileSet> {
        private readonly ObservableRangeCollection<IndexedWrapper<SpriteWrapper>> _indexedSprites = new ObservableRangeCollection<IndexedWrapper<SpriteWrapper>>();

        public override string FileExtension {
            get {
                return FileHelper.AutoTileSetExtension;
            }
        }

        public override PackIconMaterialKind Icon {
            get {
                return PackIconMaterialKind.FileImage;
            }
        }

        public IReadOnlyCollection<IndexedWrapper<SpriteWrapper>> IndexedSprites {
            get {
                return this._indexedSprites;
            }
        }

        public bool UseIntermediateDirections {
            get {
                return this.SavableValue.UseIntermediateDirections;
            }

            set {
                if (this.UseIntermediateDirections != value) {
                    this.SavableValue.UseIntermediateDirections = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public override void Refresh(AssetManager assetManager) {
            base.Refresh(assetManager);

            var spritesToAdd = new IndexedWrapper<SpriteWrapper>[this.SavableValue.Size];
            var root = this.GetRootFolder();
            var spriteWrappers = root.GetAssetsOfType<ImageAsset>().SelectMany(x => x.Sprites).Where(x => x.Sprite != null).ToList();

            for (byte i = 0; i < spritesToAdd.Length; i++) {
                var sprite = this.SavableValue.GetSprite(i);
                var spriteWrapper = spriteWrappers.FirstOrDefault(x => x.Sprite.Id == sprite?.Id);
                var indexedWrapper = this._indexedSprites.FirstOrDefault(x => x.Index == i);

                if (indexedWrapper == null) {
                    indexedWrapper = new IndexedWrapper<SpriteWrapper>(spriteWrapper, i);
                    indexedWrapper.PropertyChanged += this.IndexedWrapper_PropertyChanged;
                }
                else {
                    indexedWrapper.WrappedObject = spriteWrapper;
                }

                var displayIndex = this.GetDisplayIndex(i);
                spritesToAdd[displayIndex] = indexedWrapper;
            }

            this._indexedSprites.Reset(spritesToAdd);
            this.SavableValue.Load();
        }

        internal override void RemoveAssetReference(Guid id) {
            foreach (var indexedSprite in this._indexedSprites) {
                if (indexedSprite.WrappedObject?.Id == id) {
                    indexedSprite.WrappedObject = null;
                }
            }
        }

        private int GetDisplayIndex(int storedIndex) {
            // Look, these are stored in a kind of arbitrary order for bit math and these index
            // values make it prettier, so just go with it.
            var result = storedIndex;
            if (!this.UseIntermediateDirections) {
                switch (storedIndex) {
                    case 0:
                        result = 15;
                        break;

                    case 1:
                        result = 11;
                        break;

                    case 2:
                        result = 14;
                        break;

                    case 3:
                        result = 10;
                        break;

                    case 4:
                        result = 12;
                        break;

                    case 5:
                        result = 8;
                        break;

                    case 6:
                        result = 13;
                        break;

                    case 7:
                        result = 9;
                        break;

                    case 8:
                        result = 3;
                        break;

                    case 9:
                        result = 7;
                        break;

                    case 10:
                        result = 2;
                        break;

                    case 11:
                        result = 6;
                        break;

                    case 12:
                        result = 0;
                        break;

                    case 13:
                        result = 4;
                        break;

                    case 14:
                        result = 1;
                        break;

                    case 15:
                        result = 5;
                        break;
                }
            }

            return result;
        }

        private void IndexedWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (this.SavableValue != null && sender is IndexedWrapper<SpriteWrapper> indexedWrapper && e.PropertyName == nameof(IndexedWrapper<SpriteWrapper>.WrappedObject)) {
                this.SavableValue.SetSprite(indexedWrapper.WrappedObject?.Sprite, (byte)indexedWrapper.Index);
                this.RaisePropertyChanged(nameof(this.IndexedSprites));
            }
        }
    }
}