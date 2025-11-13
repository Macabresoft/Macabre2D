namespace Macabresoft.Macabre2D.Framework;

using System;

/// <summary>
/// A looping sprite animator which picks a random sprite out of the animation for each new frame.
/// </summary>
public class RandomFrameLoopingSpriteAnimator : LoopingSpriteAnimator {
    private readonly Random _random = new();

    /// <inheritdoc />
    public override void NextFrame() {
        if (this.GetCurrentAnimation() is { } animation) {
            var step = this._random.Next(0, animation.Animation.Steps.Count);
            animation.Reset(step, 0, 0d);
        }
    }
}