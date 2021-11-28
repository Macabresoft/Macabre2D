namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// A sprite animator that loops a single animation.
    /// </summary>
    public sealed class LoopingSpriteAnimator : BaseSpriteAnimator {
        private bool _startPlaying = true;

        /// <summary>
        /// Gets the animation reference.
        /// </summary>
        [DataMember(Order = 10, Name = "Animation")]
        public SpriteAnimationReference AnimationReference { get; } = new();

        /// <inheritdoc />
        public override SpriteSheet? SpriteSheet => this.AnimationReference?.Asset;

        /// <summary>
        /// Gets or sets a value indicating whether or not this should start playing by default.
        /// </summary>
        public bool StartPlaying {
            get => this._startPlaying;
            set => this.Set(ref this._startPlaying, value);
        }

        /// <inheritdoc />
        public override void Initialize(IScene scene, IEntity parent) {
            base.Initialize(scene, parent);
            this.Scene.Assets.ResolveAsset<SpriteSheet, Texture2D>(this.AnimationReference);
            this.ResetStep();
            this.TryStart();
        }

        /// <inheritdoc />
        protected override SpriteAnimation? GetSpriteAnimation() {
            if (this.CurrentSpriteIndex == null) {
                this.ResetStep();
            }

            return this.AnimationReference.PackagedAsset;
        }

        /// <inheritdoc />
        protected override SpriteAnimation? HandleAnimationFinished() {
            this.ResetStep();
            return this.AnimationReference.PackagedAsset;
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            base.OnPropertyChanged(sender, e);

            if (e.PropertyName == nameof(this.IsEnabled)) {
                this.TryStart();
            }
        }

        private void ResetStep() {
            if (this.AnimationReference.PackagedAsset is SpriteAnimation animation && animation.Steps.FirstOrDefault() is SpriteAnimationStep step) {
                this.CurrentSpriteIndex = step.SpriteIndex;
                this.CurrentStepIndex = 0;
            }
        }

        private void TryStart() {
            if (this.StartPlaying && this.IsEnabled) {
                this.Play();
            }
        }
    }
}