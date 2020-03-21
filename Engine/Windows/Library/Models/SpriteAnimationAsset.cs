namespace Macabre2D.Engine.Windows.Models {

    using Macabre2D.Engine.Windows.Common;
    using Macabre2D.Engine.Windows.Models.FrameworkWrappers;
    using Macabre2D.Framework;
    using MahApps.Metro.IconPacks;
    using System;
    using System.Collections.Specialized;
    using System.Linq;

    public sealed class SpriteAnimationAsset : AddableAsset<SpriteAnimation> {

        public override string FileExtension {
            get {
                return FileHelper.SpriteAnimationExtension;
            }
        }

        public override PackIconMaterialKind Icon {
            get {
                return PackIconMaterialKind.FileImage;
            }
        }

        public ObservableRangeCollection<SpriteAnimationStepWrapper> Steps { get; } = new ObservableRangeCollection<SpriteAnimationStepWrapper>();

        public SpriteAnimationStepWrapper AddStep() {
            var step = new SpriteAnimationStepWrapper(this, new SpriteAnimationStep());
            this.Steps.Add(step);
            return step;
        }

        public void AddStep(SpriteAnimationStepWrapper step) {
            this.Steps.Add(step);
        }

        public void AddStep(SpriteAnimationStepWrapper step, int index) {
            if (index >= this.Steps.Count) {
                this.AddStep(step);
            }
            else {
                this.Steps.Insert(index, step);
            }
        }

        public override void Refresh(AssetManager assetManager) {
            base.Refresh(assetManager);
            this.Steps.CollectionChanged -= this.Steps_CollectionChanged;
            this.Steps.Clear();

            if (this.SavableValue.Steps.Any()) {
                var root = this.GetRootFolder();
                var sprites = root.GetAssetsOfType<ImageAsset>().SelectMany(x => x.Sprites);

                foreach (var step in this.SavableValue.Steps) {
                    SpriteWrapper spriteWrapper = null;
                    if (step.Sprite != null) {
                        spriteWrapper = sprites.FirstOrDefault(x => x.Sprite.Id == step.Sprite.Id);
                    }

                    var wrapper = new SpriteAnimationStepWrapper(this, step, spriteWrapper);
                    this.Steps.Add(wrapper);
                    wrapper.PropertyChanged += this.Step_PropertyChanged;
                }
            }

            this.Steps.CollectionChanged += this.Steps_CollectionChanged;
            this.SavableValue.LoadContent();
        }

        public bool RemoveStep(SpriteAnimationStepWrapper step) {
            return this.Steps.Remove(step);
        }

        internal override void RemoveAssetReference(Guid id) {
            foreach (var step in this.Steps) {
                if (step.Sprite?.Id == id) {
                    step.Sprite = null;
                }
            }
        }

        private void Step_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            this.RaisePropertyChanged(nameof(this.Steps));
        }

        private void Steps_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (var step in e.NewItems.Cast<SpriteAnimationStepWrapper>()) {
                    var index = this.Steps.IndexOf(step);
                    this.SavableValue.AddStep(step.Step, index);
                    step.PropertyChanged += this.Step_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var step in e.OldItems.Cast<SpriteAnimationStepWrapper>()) {
                    this.SavableValue.RemoveStep(step.Step);
                    step.PropertyChanged -= this.Step_PropertyChanged;
                }
            }

            this.RaisePropertyChanged(nameof(this.Steps));
        }
    }
}