namespace Macabresoft.MonoGame.Core2D {

    using Microsoft.Xna.Framework;
    using System;

    /// <summary>
    /// Wraps a <see cref="GameTime"/> to provide seconds passed in regards to a predefined game speed.
    /// </summary>
    public struct FrameTime {

        /// <summary>
        /// The frame time span. This represents the time that has passed since the last frame.
        /// </summary>
        public readonly TimeSpan FrameTimeSpan;

        /// <summary>
        /// A value indicating whether or not this is running slowly.
        /// </summary>
        public readonly bool IsRunningSlowly;

        /// <summary>
        /// The milliseconds passed with game speed accounted for.
        /// </summary>
        public readonly double MillisecondsPassed;

        /// <summary>
        /// The seconds passed with game speed accounted for.
        /// </summary>
        public readonly double SecondsPassed;

        /// <summary>
        /// The total time span. This represents the total time that has passed since running.
        /// </summary>
        public readonly TimeSpan TotalTimeSpan;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameTime"/> struct.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <param name="gameSpeed">The game speed.</param>
        public FrameTime(GameTime gameTime, double gameSpeed) {
            this.IsRunningSlowly = gameTime.IsRunningSlowly;
            this.FrameTimeSpan = gameTime.ElapsedGameTime;
            this.TotalTimeSpan = gameTime.TotalGameTime;
            this.SecondsPassed = this.FrameTimeSpan.TotalSeconds * gameSpeed;
            this.MillisecondsPassed = this.FrameTimeSpan.TotalMilliseconds * gameSpeed;
        }
    }
}