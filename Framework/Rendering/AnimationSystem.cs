namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// A system that handles animations.
/// </summary>
public class AnimationSystem : UpdateSystem {
    private byte _frameRate = 30;

    /// <inheritdoc />
    public override UpdateSystemKind Kind => UpdateSystemKind.PostUpdate;

    /// <summary>
    /// Gets the bottom edge's overriden layer.
    /// </summary>
    [DataMember(Name = "Layers to Animate")]
    public LayersOverride LayersToUpdate { get; } = new();

    /// <summary>
    /// Gets or sets the frame rate. This is represented in frames per second.
    /// </summary>
    /// <value>The frame rate.</value>
    [DataMember(Order = 11, Name = "Frame Rate")]
    public byte FrameRate {
        get => this._frameRate;
        set {
            if (value != this._frameRate) {
                this._frameRate = value;
                this.ResetFrameRate();
            }
        }
    }

    /// <summary>
    /// Gets or sets the milliseconds that have passed for this animation.
    /// </summary>
    public double MillisecondsPassed { get; private set; }

    /// <summary>
    /// Get the number of milliseconds in a single frame.
    /// </summary>
    protected int MillisecondsPerFrame { get; private set; }

    /// <inheritdoc />
    public override void Initialize(IScene scene) {
        base.Initialize(scene);
        this.MillisecondsPassed = 0;
        this.ResetFrameRate();
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        var animatables = this.GetEntitiesToAnimate();

        foreach (var independentAnimatable in animatables.Where(x => x.FrameRateOverride.IsEnabled)) {
            independentAnimatable.IncrementTime(frameTime);
        }

        if (this.MillisecondsPerFrame > 0) {
            this.MillisecondsPassed += frameTime.MillisecondsPassed;
            var framesPushed = 0;

            while (this.MillisecondsPassed > this.MillisecondsPerFrame) {
                this.MillisecondsPassed -= this.MillisecondsPerFrame;
                framesPushed++;
            }

            if (framesPushed > 0) {
                foreach (var aninimatable in animatables.Where(x => !x.FrameRateOverride.IsEnabled)) {
                    for (var i = 0; i < framesPushed; i++) {
                        aninimatable.NextFrame();
                    }
                }
            }
        }
    }

    private IReadOnlyCollection<IAnimatableEntity> GetEntitiesToAnimate() {
        return !this.LayersToUpdate.IsEnabled ? this.Scene.AnimatableEntities : this.Scene.AnimatableEntities.Where(x => (x.Layers & this.LayersToUpdate.Value) != Layers.None).ToList();
    }

    private void ResetFrameRate() {
        this.MillisecondsPerFrame = this._frameRate > 0 ? this.MillisecondsPerFrame = 1000 / this._frameRate : 0;
    }
}