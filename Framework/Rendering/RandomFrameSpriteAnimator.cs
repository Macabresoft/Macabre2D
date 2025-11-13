namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;

/// <summary>
/// A looping sprite animator which picks a random sprite out of the animation for each new frame.
/// </summary>
public class RandomFrameSpriteAnimator : LoopingSpriteAnimator {
    private const byte MinimumFramesToHold = 1;
    private readonly Random _random = new();
    private byte _framesHeld;
    private byte _numberOfFramesToHold = 1;

    /// <summary>
    /// Gets or sets the number of frames to hold onto a given sprite.
    /// </summary>
    [DataMember]
    public byte NumberOfFramesToHold {
        get => this._numberOfFramesToHold;
        set => this._numberOfFramesToHold = Math.Max(value, MinimumFramesToHold);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this._framesHeld = 0;
    }

    /// <inheritdoc />
    public override void NextFrame() {
        if (this._framesHeld >= this.NumberOfFramesToHold && this.GetCurrentAnimation() is { } animation) {
            var step = this._random.Next(0, animation.Animation.Steps.Count);
            animation.Reset(step, 0, 0d);
            this._framesHeld = 0;
        }
        else {
            this._framesHeld++;
        }
    }
}