namespace Macabre2D.Framework.Rendering {

    using Microsoft.Xna.Framework;
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    /// <summary>
    /// Animates sprites at the specified framerate;
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.BaseComponent"/>
    public sealed class SpriteAnimator : BaseComponent, IUpdateableComponentAsync {
        private int _currentFrameIndex;
        private int _currentStepIndex;

        [DataMember]
        private SpriteAnimation _defaultAnimation;

        private int _frameRate = 30;
        private int _millisecondsPassed;
        private int _millisecondsPerFrame;
        private SpriteAnimation _spriteAnimation;

        [Child]
        private SpriteRenderer _spriteRenderer;

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
        public override void LoadContent() {
            var contentManager = this._scene?.Game?.Content;
            if (contentManager != null) {
                this._defaultAnimation?.LoadContent(contentManager);
                this._spriteAnimation?.LoadContent(contentManager);
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

            var contentManager = this._scene?.Game?.Content;
            if (contentManager != null) {
                this._spriteAnimation?.LoadContent(contentManager);
            }

            if (this._spriteAnimation != null && this._spriteRenderer != null) {
                this._spriteRenderer.Sprite = this._spriteAnimation.Steps.FirstOrDefault()?.Sprite;
            }

            this.Play();
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