namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Animates sprites at the specified framerate;
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.BaseComponent"/>
    public sealed class SpriteAnimator : BaseComponent, IUpdateableComponentAsync, IAssetComponent<SpriteAnimation>, IAssetComponent<Sprite> {
        private int _currentFrameIndex;
        private int _currentStepIndex;

        [DataMember]
        private SpriteAnimation _defaultAnimation;

        private int _frameRate = 30;
        private int _millisecondsPassed;
        private int _millisecondsPerFrame;
        private SpriteAnimation _spriteAnimation;

#pragma warning disable CS0649

        [Child]
        private SpriteRenderer _spriteRenderer;

#pragma warning restore CS0649

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAnimator"/> class.
        /// </summary>
        public SpriteAnimator() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAnimator"/> class.
        /// </summary>
        /// <param name="animation">The animation.</param>
        public SpriteAnimator(SpriteAnimation animation) : this() {
            this._defaultAnimation = animation;
        }

        /// <summary>
        /// Gets or sets the frame rate. This is represented in frames per second.
        /// </summary>
        /// <value>The frame rate.</value>
        [DataMember]
        public int FrameRate {
            get {
                return this._frameRate;
            }

            set {
                if (value > 0) {
                    this._frameRate = value;
                }
            }
        }

        /// <inheritdoc/>
        IEnumerable<Guid> IAssetComponent<SpriteAnimation>.GetOwnedAssetIds() {
            var ids = new HashSet<Guid>();
            if (this._defaultAnimation != null) {
                ids.Add(this._defaultAnimation.Id);
            }

            if (this._spriteAnimation != null) {
                ids.Add(this._spriteAnimation.Id);
            }

            return ids;
        }

        /// <inheritdoc/>
        IEnumerable<Guid> IAssetComponent<Sprite>.GetOwnedAssetIds() {
            var ids = new HashSet<Guid>();
            if (this._defaultAnimation != null) {
                ids.AddRange(this._defaultAnimation.GetSpriteIds());
            }

            if (this._spriteAnimation != null) {
                ids.AddRange(this._defaultAnimation.GetSpriteIds());
            }

            return ids;
        }

        /// <inheritdoc/>
        public bool HasAsset(Guid id) {
            return this._defaultAnimation?.Id == id ||
                this._spriteAnimation?.Id == id ||
                this._defaultAnimation?.HasSprite(id) == true ||
                this._spriteAnimation?.HasSprite(id) == true;
        }

        /// <inheritdoc/>
        public override void LoadContent() {
            if (this.Scene.IsInitialized) {
                this._defaultAnimation?.LoadContent();
                this._spriteAnimation?.LoadContent();
            }

            base.LoadContent();
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause() {
            this.IsEnabled = false;
        }

        /// <summary>
        /// Plays this instance.
        /// </summary>
        public void Play() {
            this.IsEnabled = true;
        }

        /// <summary>
        /// Plays the specified animation. IF the animation is a looping animation, it will continue
        /// to play. If the animation is not a looping animation, it will return to the default animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        public void Play(SpriteAnimation animation) {
            this.Stop();
            this._spriteAnimation = animation;

            if (this.Scene.IsInitialized) {
                this._spriteAnimation?.LoadContent();
            }

            if (this._spriteAnimation != null && this._spriteRenderer != null) {
                this._spriteRenderer.Sprite = this._spriteAnimation.Steps.FirstOrDefault()?.Sprite;
            }

            this.Play();
        }

        /// <inheritdoc/>
        void IAssetComponent<Sprite>.RefreshAsset(Sprite newInstance) {
            if (newInstance != null) {
                this._defaultAnimation?.RefreshSprite(newInstance);
                this._spriteAnimation?.RefreshSprite(newInstance);
            }
        }

        /// <inheritdoc/>
        void IAssetComponent<SpriteAnimation>.RefreshAsset(SpriteAnimation newInstance) {
            if (newInstance != null) {
                if (this._defaultAnimation?.Id == newInstance.Id) {
                    this._defaultAnimation = newInstance;
                }

                if (this._spriteAnimation?.Id == newInstance.Id) {
                    this._spriteAnimation = newInstance;
                }
            }
        }

        /// <inheritdoc/>
        public bool RemoveAsset(Guid id) {
            bool result;
            if (this._defaultAnimation?.Id == id) {
                this._defaultAnimation = null;
                result = true;
            }
            else {
                result = this._defaultAnimation?.RemoveSprite(id) ?? false;
            }

            if (this._spriteAnimation?.Id == id) {
                if (this.IsEnabled) {
                    this.Stop();
                }

                this._spriteAnimation = null;
                result = true;
            }
            else {
                result = result || (this._spriteAnimation?.RemoveSprite(id) ?? false);
            }

            return result;
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop() {
            this.Pause();
            this._millisecondsPassed = 0;
            this._currentFrameIndex = 0;
            this._currentStepIndex = 0;
        }

        /// <inheritdoc/>
        bool IAssetComponent<Sprite>.TryGetAsset(Guid id, out Sprite asset) {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        bool IAssetComponent<SpriteAnimation>.TryGetAsset(Guid id, out SpriteAnimation asset) {
            if (this._defaultAnimation?.Id == id) {
                asset = this._defaultAnimation;
            }
            else if (this._spriteAnimation?.Id == id) {
                asset = this._spriteAnimation;
            }
            else {
                asset = null;
            }

            return asset != null;
        }

        /// <summary>
        /// Updates this instance asynchronously.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <returns>The task.</returns>
        public Task UpdateAsync(GameTime gameTime) {
            return Task.Run(() => {
                if (this._spriteAnimation != null && this._spriteAnimation.Steps.Any() && this._spriteRenderer != null) {
                    this._millisecondsPassed += Convert.ToInt32(gameTime.ElapsedGameTime.TotalMilliseconds);

                    if (this._millisecondsPassed >= this._millisecondsPerFrame) {
                        while (this._millisecondsPassed >= this._millisecondsPerFrame) {
                            this._millisecondsPassed -= this._millisecondsPerFrame;
                            this._currentFrameIndex++;
                        }

                        var currentStep = this._spriteAnimation.Steps.ElementAt(this._currentStepIndex);
                        if (this._currentFrameIndex >= currentStep.Frames) {
                            this._currentFrameIndex = 0;
                            this._currentStepIndex++;

                            if (this._currentStepIndex >= this._spriteAnimation.Steps.Count) {
                                this._currentStepIndex = 0;
                                if (this._spriteAnimation.Id != this._defaultAnimation.Id && !this._spriteAnimation.ShouldLoop) {
                                    this._spriteAnimation = this._defaultAnimation;
                                    this._millisecondsPassed = 0;
                                }
                            }

                            currentStep = this._spriteAnimation.Steps.ElementAt(this._currentStepIndex);
                            this._spriteRenderer.Sprite = currentStep.Sprite;
                        }
                    }
                }
            });
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            if (this._frameRate <= 0) {
                throw new NotSupportedException("Hey, framerates gotta be positive. Stay positive. Don't be a zero. Don't be negative.");
            }

            this._millisecondsPerFrame = 1000 / this._frameRate;
            this._spriteAnimation = this._defaultAnimation;

            if (this._spriteAnimation != null && this._spriteRenderer != null) {
                this._spriteRenderer.Sprite = this._spriteAnimation.Steps.FirstOrDefault()?.Sprite;
            }
        }
    }
}