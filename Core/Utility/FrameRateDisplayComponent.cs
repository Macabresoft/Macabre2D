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

            if (this.Entity.TryGetAncestralComponent(out this._camera) && this._camera != null) {
                this._camera.PropertyChanged += this.Camera_PropertyChanged;
            }

            this._rollingAverage.Add(0f); // Must initialize it so that infinity is not an option.
            this.Entity.Scene.Game.ViewportSizeChanged += this.Game_ViewportSizeChanged;
            this.RenderSettings.OffsetType = PixelOffsetType.TopRight;
        }

        /// <inheritdoc />
        public void Update(FrameTime frameTime, InputState inputState) {
            if (this._camera != null && frameTime.FrameTimeSpan.TotalSeconds > 0d) {
                this.CurrentFrameRate = 1f / (float)frameTime.FrameTimeSpan.TotalSeconds;
                this._rollingAverage.Add(this.CurrentFrameRate);
                this.Text = $"FPS: {this.AverageFrameRate:F2}";
            }
        }

        private void AdjustPosition() {
            if (this._camera != null) {
                this.Entity.SetWorldPosition(new Vector2(this._camera.GetViewWidth(), this._camera.ViewHeight));
            }
        }

        private void Camera_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CameraComponent.ViewHeight)) {
                this.AdjustPosition();
            }
        }

        private void Game_ViewportSizeChanged(object? sender, Point e) {
            this.AdjustPosition();
        }
    }
}