namespace Macabre2D.UI.GameEditorLibrary.Models.FrameworkWrappers {

    using Macabre2D.Framework;

    public sealed class SpriteAnimationStepWrapper : NotifyPropertyChanged {
        private readonly SpriteAnimationAsset _asset;
        private SpriteWrapper _spriteWrapper;

        public SpriteAnimationStepWrapper(SpriteAnimationAsset asset, SpriteAnimationStep step) {
            this._asset = asset;
            this.Step = step;
        }

        public SpriteAnimationStepWrapper(
            SpriteAnimationAsset asset,
            SpriteAnimationStep step,
            SpriteWrapper spriteWrapper) : this(asset, step) {
            this._spriteWrapper = spriteWrapper;
        }

        public int Frames {
            get {
                return this.Step.Frames;
            }

            set {
                this.Step.Frames = value;
                this.RaisePropertyChanged();
            }
        }

        public SpriteWrapper Sprite {
            get {
                return this._spriteWrapper;
            }

            set {
                this._spriteWrapper = value;
                this.Step.Sprite = this._spriteWrapper?.Sprite;
                this.RaisePropertyChanged();
            }
        }

        internal SpriteAnimationStep Step { get; }
    }
}