namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Animates sprites at the specified framerate;
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.BaseComponent"/>
    public class SpriteAnimationComponent : SpriteRenderComponent, IUpdateableComponent {
        private readonly Queue<QueueableSpriteAnimation> _queuedSpriteAnimations = new Queue<QueueableSpriteAnimation>();
        private QueueableSpriteAnimation _currentAnimation;
        private uint _currentFrameIndex;
        private uint _currentStepIndex;
        private byte _frameRate = 30;
        private uint _millisecondsPassed;
        private uint _millisecondsPerFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAnimationComponent"/> class.
        /// </summary>
        public SpriteAnimationComponent() : base() {
        }

        /// <summary>
        /// Gets the current animation.
        /// </summary>
        /// <value>The current animation.</value>
        public SpriteAnimation CurrentAnimation {
            get {
                return this._currentAnimation?.Animation;
            }
        }

        /// <summary>
        /// Gets or sets the frame rate. This is represented in frames per second.
        /// </summary>
        /// <value>The frame rate.</value>
        [DataMember(Order = 1, Name = "Frame Rate")]
        public byte FrameRate {
            get {
                return this._frameRate;
            }

            set {
                if (value > 0) {
                    this._frameRate = value;
                }
            }
        }

        /// <summary>
        /// Enqueues the specified animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="shouldLoopIndefinitely">
        /// if set to <c>true</c> the sprite animation will loop indefinitely when no other
        /// animation has been queued.
        /// </param>
        public void Enqueue(SpriteAnimation animation, bool shouldLoopIndefinitely) {
            this.Enqueue(new QueueableSpriteAnimation(animation, shouldLoopIndefinitely));
        }

        /// <summary>
        /// Enqueues the specified queueable sprite animation.
        /// </summary>
        /// <param name="queueableSpriteAnimation">The queueable sprite animation.</param>
        public void Enqueue(QueueableSpriteAnimation queueableSpriteAnimation) {
            this._queuedSpriteAnimations.Enqueue(queueableSpriteAnimation);
        }

        /// <summary>
        /// Enqueues the specified animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="shouldLoopIndefinitely">
        /// if set to <c>true</c> the sprite animation will loop indefinitely when no other
        /// animation has been queued.
        /// </param>
        /// <param name="numberOfLoops">The number of loops.</param>
        public void Enqueue(SpriteAnimation animation, bool shouldLoopIndefinitely, ushort numberOfLoops) {
            this.Enqueue(new QueueableSpriteAnimation(animation, shouldLoopIndefinitely, numberOfLoops));
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
        /// Plays the specified animation, which clears out the current queue and replaces the
        /// previous animation. If the animation is a looping animation, it will continue to play
        /// until a new animation is queued. If the animation is not a looping animation, it will
        /// pause on the final frame.
        /// </summary>
        /// <param name="animation">The animation.</param>
        public void Play(SpriteAnimation animation, bool shouldLoop) {
            this.Stop(true);
            this._queuedSpriteAnimations.Clear();
            this._queuedSpriteAnimations.Enqueue(new QueueableSpriteAnimation(animation, shouldLoop));
            this.Play();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop(bool eraseQueue) {
            this.Pause();
            this._millisecondsPassed = 0;
            this._currentFrameIndex = 0;
            this._currentStepIndex = 0;

            if (eraseQueue) {
                this._currentAnimation = null;
                this._queuedSpriteAnimations.Clear();
                this.Sprite = null;
            }
        }

        /// <inheritdoc/>
        public void Update(GameTime gameTime) {
            if (this._currentAnimation == null && this._queuedSpriteAnimations.Any()) {
                this._currentAnimation = this._queuedSpriteAnimations.Dequeue();
                this._currentAnimation.Animation.LoadContent();
                this.Sprite = this._currentAnimation.Animation.Steps.FirstOrDefault()?.Sprite;
            }

            if (this._currentAnimation != null) {
                this._millisecondsPassed += Convert.ToUInt32(gameTime.ElapsedGameTime.TotalMilliseconds);

                if (this._millisecondsPassed >= this._millisecondsPerFrame) {
                    while (this._millisecondsPassed >= this._millisecondsPerFrame) {
                        this._millisecondsPassed -= this._millisecondsPerFrame;
                        this._currentFrameIndex++;
                    }

                    var currentStep = this._currentAnimation.Animation.Steps.ElementAt((int)this._currentStepIndex);
                    if (this._currentFrameIndex >= currentStep.Frames) {
                        this._currentFrameIndex = 0;
                        this._currentStepIndex++;

                        if (this._currentStepIndex >= this._currentAnimation.Animation.Steps.Count) {
                            this._currentStepIndex = 0;
                            if (this._queuedSpriteAnimations.Any()) {
                                this._currentAnimation = this._queuedSpriteAnimations.Dequeue();
                                this._millisecondsPassed = 0;
                            }
                            else if (!this._currentAnimation.ShouldLoopIndefinitely) {
                                this._currentAnimation = null;
                            }
                        }

                        currentStep = this._currentAnimation?.Animation.Steps.ElementAt((int)this._currentStepIndex);

                        if (currentStep != null) {
                            this.Sprite = currentStep?.Sprite;
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        protected override void Initialize() {
            base.Initialize();
            this._millisecondsPerFrame = 1000u / this._frameRate;
        }
    }
}