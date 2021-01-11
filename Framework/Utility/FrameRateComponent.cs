namespace Macabresoft.Macabre2D.Framework {
    using System.ComponentModel.DataAnnotations;
    using Macabresoft.Core;

    /// <summary>
    /// A component which measures frame rate.
    /// </summary>
    [Display(Name = "Frame Rate Component")]
    public class FrameRateComponent : GameUpdateableComponent {
        private readonly RollingMeanFloat _rollingAverage = new RollingMeanFloat(10);

        /// <summary>
        /// Gets the average frame rate over the course of 30 frames.
        /// </summary>
        /// <value>The average frame rate over the course of 30 frames.</value>
        public float AverageFrameRate => _rollingAverage.MeanValue;

        /// <summary>
        /// Gets the current frame rate.
        /// </summary>
        /// <value>The current frame rate.</value>
        public float CurrentFrameRate { get; private set; }

        /// <inheritdoc />
        public override void Update(FrameTime frameTime, InputState inputState) {
            this.CurrentFrameRate = 1f / (float)frameTime.FrameTimeSpan.TotalSeconds;
            this._rollingAverage.Add(this.CurrentFrameRate);
        }
    }
}