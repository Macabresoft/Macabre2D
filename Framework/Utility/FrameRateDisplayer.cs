namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel.DataAnnotations;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// An entity which displays frame rate in the top right corner of the screen.
/// </summary>
public sealed class FrameRateDisplayEntity : TextRenderer, IUpdateableEntity {
    private readonly RollingMeanFloat _rollingAverage = new(10);
    private Camera? _camera;

    /// <summary>
    /// Gets the average frame rate over the course of 30 frames.
    /// </summary>
    /// <value>The average frame rate over the course of 30 frames.</value>
    public float AverageFrameRate => this._rollingAverage.MeanValue;

    /// <summary>
    /// Gets the current frame rate.
    /// </summary>
    /// <value>The current frame rate.</value>
    public float CurrentFrameRate { get; private set; }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity entity) {
        base.Initialize(scene, entity);
        this.TryGetAncestor(out this._camera);
        this.RenderOptions.OffsetType = PixelOffsetType.TopRight;
    }

    /// <inheritdoc />
    public bool ShouldUpdate => this.IsEnabled;

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
            this.SetWorldPosition(this._camera.WorldPosition + new Vector2(this._camera.ViewWidth, this._camera.ActualViewHeight) * 0.5f);
        }
    }
}