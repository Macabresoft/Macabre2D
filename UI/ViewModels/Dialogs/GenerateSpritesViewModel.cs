namespace Macabre2D.UI.ViewModels.Dialogs {

    using Macabre2D.UI.Models;

    public sealed class GenerateSpritesViewModel : OKCancelDialogViewModel {
        private int _numberOfColumns = 1;
        private int _numberOfRows = 1;
        private bool _replaceExistingSprites;

        public GenerateSpritesViewModel(ImageAsset imageAsset) {
            this.ImageAsset = imageAsset;
        }

        public ImageAsset ImageAsset { get; }

        public int NumberOfColumns {
            get {
                return this._numberOfColumns;
            }

            set {
                if (value > 0 && value <= this.ImageAsset.Width) {
                    this.Set(ref this._numberOfColumns, value);
                }
            }
        }

        public int NumberOfRows {
            get {
                return this._numberOfRows;
            }

            set {
                if (value > 0 && value <= this.ImageAsset.Height) {
                    this.Set(ref this._numberOfRows, value);
                }
            }
        }

        public bool ReplaceExistingSprites {
            get {
                return this._replaceExistingSprites;
            }

            set {
                this.Set(ref this._replaceExistingSprites, value);
            }
        }
    }
}