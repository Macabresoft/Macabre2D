namespace Macabre2D.Framework.Rendering {

    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// An animation that explicitly uses sprites.
    /// </summary>
    /// <seealso cref="Macabre2D.Framework.IIdentifiable"/>
    [DataContract]
    public sealed class SpriteAnimation : IIdentifiable {

        [DataMember]
        private readonly List<SpriteAnimationStep> _steps = new List<SpriteAnimationStep>();

        [DataMember]
        private Guid _id = Guid.NewGuid();

        private bool _isLoaded = false;

        [DataMember]
        private bool _shouldLoop = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAnimation"/> class.
        /// </summary>
        public SpriteAnimation() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAnimation"/> class.
        /// </summary>
        /// <param name="steps">The steps.</param>
        /// <param name="shouldLoop">if set to <c>true</c> [should loop].</param>
        internal SpriteAnimation(IEnumerable<SpriteAnimationStep> steps, bool shouldLoop) {
            this._steps.AddRange(steps);
            this._shouldLoop = shouldLoop;
        }

        /// <inheritdoc/>
        public Guid Id {
            get {
                return this._id;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this animation should loop.
        /// </summary>
        /// <value><c>true</c> if [should loop]; otherwise, <c>false</c>.</value>
        public bool ShouldLoop {
            get {
                return this._shouldLoop;
            }

            set {
                this._shouldLoop = value;
            }
        }

        /// <summary>
        /// Gets the steps.
        /// </summary>
        /// <value>The steps.</value>
        public IReadOnlyCollection<SpriteAnimationStep> Steps {
            get {
                return this._steps;
            }
        }

        /// <summary>
        /// Adds the step.
        /// </summary>
        /// <returns>The added step.</returns>
        public SpriteAnimationStep AddStep() {
            var step = new SpriteAnimationStep();
            this._steps.Add(step);
            return step;
        }

        /// <summary>
        /// Adds the step.
        /// </summary>
        /// <param name="step">The step.</param>
        public void AddStep(SpriteAnimationStep step) {
            this._steps.Add(step);
        }

        /// <summary>
        /// Adds the step.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="index">The index.</param>
        public void AddStep(SpriteAnimationStep step, int index) {
            if (index >= this._steps.Count) {
                this._steps.Add(step);
            }
            else {
                this._steps.Insert(index, step);
            }
        }

        /// <summary>
        /// Removes the step.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>A value indicating whether or not the step was removed.</returns>
        public bool RemoveStep(SpriteAnimationStep step) {
            return this._steps.Remove(step);
        }

        internal void LoadContent(ContentManager contentManager) {
            if (!this._isLoaded) {
                try {
                    foreach (var sprite in this._steps.Select(x => x.Sprite).Where(x => x?.Texture != null)) {
                        sprite.Texture = contentManager.Load<Texture2D>(sprite.ContentPath);
                    }
                }
                finally {
                    this._isLoaded = true;
                }
            }
        }
    }
}