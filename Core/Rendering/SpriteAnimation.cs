namespace Macabresoft.MonoGame.Core {

    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// An animation that explicitly uses sprites.
    /// </summary>
    public sealed class SpriteAnimation : BaseIdentifiable, IAsset {

        [DataMember]
        private readonly List<SpriteAnimationStep> _steps = new List<SpriteAnimationStep>();

        private bool _isLoaded = false;

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
        public SpriteAnimation(IEnumerable<SpriteAnimationStep> steps) {
            this._steps.AddRange(steps);
        }

        /// <inheritdoc/>
        [DataMember]
        public Guid AssetId { get; set; }

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

            if (this._isLoaded) {
                step.Sprite?.Load();
            }
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

            if (this._isLoaded) {
                step.Sprite?.Load();
            }
        }

        /// <summary>
        /// Gets the sprite ids.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Guid> GetSpriteIds() {
            return this._steps.Where(x => x.Sprite != null).Select(x => x.Sprite.Id);
        }

        /// <summary>
        /// Determines whether this instance has a sprite with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// <c>true</c> if this instance has a sprite with the specified identifier; otherwise, <c>false</c>.
        /// </returns>
        public bool HasSprite(Guid id) {
            return this._steps.Any(x => x.Sprite?.Id == id);
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        public void LoadContent() {
            if (!this._isLoaded) {
                try {
                    foreach (var sprite in this._steps.Select(x => x.Sprite).Where(x => x?.Texture != null)) {
                        sprite.Texture = AssetManager.Instance.Load<Texture2D>(sprite.AssetId);
                    }
                }
                finally {
                    this._isLoaded = true;
                }
            }
        }

        /// <summary>
        /// Refreshes the sprite.
        /// </summary>
        /// <param name="sprite">The sprite.</param>
        public void RefreshSprite(Sprite sprite) {
            var staps = this._steps.Where(x => x.Sprite != null && x.Sprite.Id == sprite.Id);
            foreach (var step in staps) {
                step.Sprite = sprite;
            }
        }

        /// <summary>
        /// Removes the sprite.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A value indicating whether or not the sprite was removed.</returns>
        public bool RemoveSprite(Guid id) {
            var result = false;
            var staps = this._steps.Where(x => x.Sprite?.Id == id);
            foreach (var step in staps) {
                result = true;
                step.Sprite = null;
            }

            return result;
        }

        /// <summary>
        /// Removes the step.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <returns>A value indicating whether or not the step was removed.</returns>
        public bool RemoveStep(SpriteAnimationStep step) {
            return this._steps.Remove(step);
        }

        /// <summary>
        /// Tries the get sprite.
        /// </summary>
        /// <param name="spriteId">The sprite identifier.</param>
        /// <param name="sprite">The sprite.</param>
        /// <returns>A value indicating whether or not the sprite was found.</returns>
        public bool TryGetSprite(Guid spriteId, out Sprite sprite) {
            sprite = this._steps.FirstOrDefault(x => x.Sprite?.Id == spriteId)?.Sprite;
            return sprite != null;
        }
    }
}