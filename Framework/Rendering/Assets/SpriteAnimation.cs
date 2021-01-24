namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// An animation that explicitly uses sprites.
    /// </summary>
    public sealed class SpriteAnimation : BaseSpriteSheetAsset {
        [DataMember]
        private readonly List<SpriteAnimationStep> _steps = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteAnimation" /> class.
        /// </summary>
        /// <param name="steps">The steps.</param>
        public SpriteAnimation(IEnumerable<SpriteAnimationStep> steps) {
            this._steps.AddRange(steps);
        }

        /// <summary>
        /// Gets the steps.
        /// </summary>
        /// <value>The steps.</value>
        public IReadOnlyCollection<SpriteAnimationStep> Steps => this._steps;

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
    }
}