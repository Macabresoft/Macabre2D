namespace Macabre2D.Framework {

    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// An animation that explicitly uses sprites.
    /// </summary>
    public sealed class SpriteAnimation : BaseIdentifiable {

        [DataMember]
        private readonly List<SpriteAnimationStep> _steps = new List<SpriteAnimationStep>();

        private bool _isLoaded = false;

        [DataMember]
        private bool _shouldLoop = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAnimation"/> class.
        /// </summary>
        public SpriteAnimation() {
        }

        internal SpriteAnimation(IEnumerable<SpriteAnimationStep> steps, bool shouldLoop) {
            this._steps.AddRange(steps);
            this._shouldLoop = shouldLoop;
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

        internal IEnumerable<Guid> GetSpriteIds() {
            return this._steps.Where(x => x.Sprite != null).Select(x => x.Sprite.Id);
        }

        internal bool HasSprite(Guid id) {
            return this._steps.Any(x => x.Sprite?.Id == id);
        }

        internal void LoadContent() {
            if (!this._isLoaded) {
                try {
                    foreach (var sprite in this._steps.Select(x => x.Sprite).Where(x => x?.Texture != null)) {
                        sprite.Texture = AssetManager.Instance.Load<Texture2D>(sprite.ContentId);
                    }
                }
                finally {
                    this._isLoaded = true;
                }
            }
        }

        internal void RefreshSprite(Sprite sprite) {
            var staps = this._steps.Where(x => x.Sprite != null && x.Sprite.Id == sprite.Id);
            foreach (var step in staps) {
                step.Sprite = sprite;
            }
        }

        internal bool RemoveSprite(Guid id) {
            var result = false;
            var staps = this._steps.Where(x => x.Sprite?.Id == id);
            foreach (var step in staps) {
                result = true;
                step.Sprite = null;
            }

            return result;
        }

        internal bool TryGetSprite(Guid spriteId, out Sprite sprite) {
            sprite = this._steps.FirstOrDefault(x => x.Sprite?.Id == spriteId)?.Sprite;
            return sprite != null;
        }
    }
}