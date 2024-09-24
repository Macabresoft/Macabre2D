namespace Macabresoft.Macabre2D.Framework;

using System.Runtime.Serialization;

/// <summary>
/// Enables <see cref="TextAreaRenderer" /> to scroll by adjusting its <see cref="TextAreaRenderer.VerticalOffset" /> over time.
/// </summary>
public class TextAreaScroller : UpdateableEntity {
    private bool _isScrollingDown = true;

    /// <summary>
    /// Gets the time to hold at the edges of the scroll.
    /// </summary>
    [DataMember]
    public GameTimer HoldTimer { get; } = new(1f);

    /// <summary>
    /// Gets the time to scroll from the top of the text area to the bototm.
    /// </summary>
    [DataMember]
    public GameTimer ScrollTimer { get; } = new(4f);

    /// <summary>
    /// Gets a value indicating whether this should begin with a hold. If true, this will begin scrolling after the initial hold timer.
    /// </summary>
    [DataMember]
    public bool BeginWithHold { get; set; } = true;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this._isScrollingDown = true;

        if (this.BeginWithHold) {
            this.ScrollTimer.Stop();
            this.HoldTimer.Restart();
        }
        else {
            this.ScrollTimer.Restart();
            this.HoldTimer.Stop();
        }
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        if (this.Parent is TextAreaRenderer textArea) {
            if (this.HoldTimer.State == TimerState.Running) {
                this.HoldTimer.Increment(frameTime);

                if (this.HoldTimer.State == TimerState.Finished) {
                    this.ScrollTimer.Restart();
                }
            }
            else if (this.ScrollTimer.State == TimerState.Running) {
                this.ScrollTimer.Increment(frameTime);
                if (this._isScrollingDown) {
                    textArea.VerticalOffset = (textArea.TextHeight - textArea.BoundingArea.Height) * this.ScrollTimer.PercentComplete;
                }
                else {
                    textArea.VerticalOffset = (textArea.TextHeight - textArea.BoundingArea.Height) * (1f - this.ScrollTimer.PercentComplete);
                }
            }
            else {
                this.HoldTimer.Restart();
                this._isScrollingDown = !this._isScrollingDown;
            }
        }
    }
}