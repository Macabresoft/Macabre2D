namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Animates sprites at the specified frame rate.
    /// </summary>
    [Display(Name = "Sprite Animator")]
    public sealed class SpriteAnimatorComponent : BaseSpriteComponent, IGameUpdateableComponent {
        private readonly Queue<QueueableSpriteAnimation> _queuedSpriteAnimations = new();
        private QueueableSpriteAnimation? _currentAnimation;
        private uint _currentFrameIndex;
        private byte _currentSpriteIndex;
        private uint _currentStepIndex;
        private byte _frameRate = 30;
        private bool _isPlaying;
        private uint _millisecondsPassed;
        private uint _millisecondsPerFrame;

        /// <summary>
        /// Gets the animation reference.
        /// </summary>
        [DataMember(Order = 10, Name = "Animation")]
        public SpriteSheetAssetReference<SpriteAnimation> AnimationReference { get; } = new();

        /// <summary>
        /// Gets the current animation.
        /// </summary>
        /// <value>The current animation.</value>
        public SpriteAnimation? CurrentAnimation => this._currentAnimation?.Animation;

        /// <inheritdoc />
        public int UpdateOrder => 0;

        /// <summary>
        /// Gets or sets the frame rate. This is represented in frames per second.
        /// </summary>
        /// <value>The frame rate.</value>
        [DataMember(Order = 11, Name = "Frame Rate")]
        public byte FrameRate {
            get => this._frameRate;

            set {
                if (value > 0) {
                    this.Set(ref this._frameRate, value);
                }
            }
        }

        /// <inheritdoc />
        protected override byte SpriteIndex => this._currentSpriteIndex;

        /// <inheritdoc />
        protected override SpriteSheet? SpriteSheet => this.AnimationReference.SpriteSheet;

        /// <summary>
        /// Enqueues the specified animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="shouldLoopIndefinitely">
        /// if set to <c>true</c> the sprite animation will loop indefinitely when no other
        /// animation has been queued.
        /// </param>
        public void Enqueue(SpriteAnimation animation, bool shouldLoopIndefinitely) {
            if (this.AnimationReference.SpriteSheet is SpriteSheet spriteSheet && spriteSheet.AssetId == animation.SpriteSheet?.AssetId) {
                this.Enqueue(new QueueableSpriteAnimation(animation, shouldLoopIndefinitely));
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
        /// <param name="numberOfLoops">The number of loops.</param>
        public void Enqueue(SpriteAnimation animation, bool shouldLoopIndefinitely, ushort numberOfLoops) {
            if (this.AnimationReference.SpriteSheet is SpriteSheet spriteSheet && spriteSheet.AssetId == animation.SpriteSheet?.AssetId) {
                this.Enqueue(new QueueableSpriteAnimation(animation, shouldLoopIndefinitely, numberOfLoops));
            }
        }

        /// <inheritdoc />
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            AssetManager.Instance.ResolveAsset<SpriteAnimation, Texture2D>(this.AnimationReference);

            this._millisecondsPerFrame = 1000u / this._frameRate;
            if (this.AnimationReference.Asset is SpriteAnimation animation) {
                this.Play(animation, true);

                var step = animation.Steps.FirstOrDefault();
                if (step != null) {
                    this._currentSpriteIndex = step.SpriteIndex;
                }
            }
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause() {
            this._isPlaying = false;
        }

        /// <summary>
        /// Plays this instance.
        /// </summary>
        public void Play() {
            this.IsEnabled = true;
            this._isPlaying = true;
        }

        /// <summary>
        /// Plays the specified animation, which clears out the current queue and replaces the
        /// previous animation. If the animation is a looping animation, it will continue to play
        /// until a new animation is queued. If the animation is not a looping animation, it will
        /// pause on the final frame.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="shouldLoop">A value indicating whether or not the animation should loop.</param>
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
            this.IsEnabled = false;
            this._millisecondsPassed = 0;
            this._currentFrameIndex = 0;
            this._currentStepIndex = 0;

            if (eraseQueue) {
                this._currentAnimation = null;
                this._queuedSpriteAnimations.Clear();
                this._currentStepIndex = 0;
            }
        }

        /// <inheritdoc />
        public void Update(FrameTime frameTime, InputState inputState) {
            if (this._isPlaying) {
                if (this._currentAnimation == null && this._queuedSpriteAnimations.Any()) {
                    this._currentAnimation = this._queuedSpriteAnimations.Dequeue();

                    var step = this._currentAnimation.Animation.Steps.FirstOrDefault();
                    if (step != null) {
                        this._currentSpriteIndex = step.SpriteIndex;
                    }
                }

                if (this._currentAnimation != null) {
                    this._millisecondsPassed += Convert.ToUInt32(frameTime.MillisecondsPassed);

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
                                this._currentSpriteIndex = currentStep.SpriteIndex;
                            }
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(this.IsVisible)) {
                this.RaisePropertyChanged(nameof(this.IsEnabled));
            }
        }

        /// <summary>
        /// Enqueues the specified queueable sprite animation.
        /// </summary>
        /// <param name="queueableSpriteAnimation">The queueable sprite animation.</param>
        private void Enqueue(QueueableSpriteAnimation queueableSpriteAnimation) {
            this._queuedSpriteAnimations.Enqueue(queueableSpriteAnimation);
        }
    }
}