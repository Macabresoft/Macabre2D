namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// A component which displays frame rate in the top right corner of the screen.
    /// </summary>
    public sealed class FrameRateDisplayComponent : TextRenderComponent, IGameUpdateableComponent {
        private readonly RollingMeanFloat _rollingAverage = new RollingMeanFloat(10);
        private CameraComponent? _camera;

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
        public override void Initialize(IGameEntity entity) {
            base.Initialize(entity);
            this.Entity.TryGetAncestralComponent(out this._camera);
            this.RenderSettings.OffsetType = PixelOffsetType.TopRight;
        }

        /// <inheritdoc />
        public void Update(FrameTime frameTime, InputState inputState) {
            if (this._camera != null && frameTime.FrameTimeSpan.TotalSeconds > 0d) {
                this.CurrentFrameRate = 1f / (float)frameTime.FrameTimeSpan.TotalSeconds;
                this._rollingAverage.Add(this.CurrentFrameRate);
                this.Text = $"FPS: {this.AverageFrameRate:F2}";
                this.AdjustPosition();
            }
        }

        private void AdjustPosition() {
            if (this._camera != null) {
                this.Entity.SetWorldPosition(this._camera.Entity.Transform.Position + new Vector2(this._camera.GetViewWidth(), this._camera.ViewHeight) * 0.5f);
            }
        }
    }
}