namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Reference for a <see cref="SpriteSheet" /> which aids in emitting animations.
/// </summary>
public class AnimationEmitterReference : SpriteSheetReference {
    private readonly List<PositionalSpriteAnimation> _availableAnimations = [];
    private readonly Random _randomizer = new();
    private readonly List<PositionalSpriteAnimation> _runningAnimations = [];

    /// <summary>
    /// Gets or sets the emission offset.
    /// </summary>
    [DataMember]
    public Vector2 EmissionOffset { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of active animations.
    /// </summary>
    [DataMember]
    public byte MaxActiveAnimations { get; set; } = 5;

    public IReadOnlyCollection<PositionalSpriteAnimation> RunningAnimations => this._runningAnimations;

    /// <summary>
    /// Gets or sets the amount of positional variation that should be used when spawning.
    /// </summary>
    [DataMember]
    public Vector2 SpawnVariation { get; set; }

    /// <summary>
    /// Gets a timer for the time between sprite appearances.
    /// </summary>
    [DataMember]
    public GameTimer TimeBetweenEmissions { get; } = new() { TimeLimit = 0.5f };

    /// <summary>
    /// Increments the time on a given animation.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="millisecondsPerFrame">The number of milliseconds per frame.</param>
    public void IncrementTime(FrameTime frameTime, int millisecondsPerFrame) {
        for (var i = this._runningAnimations.Count - 1; i >= 0; i--) {
            var animation = this._runningAnimations[i];
            animation.Update(frameTime, millisecondsPerFrame, out var isAnimationOver);
            if (isAnimationOver) {
                this._runningAnimations.Remove(animation);
                this._availableAnimations.Add(animation);
            }
        }
    }

    /// <inheritdoc />
    public override void LoadAsset(SpriteSheet asset) {
        base.LoadAsset(asset);

        this._availableAnimations.Clear();
        this._runningAnimations.Clear();

        if (this.Asset is { } spriteSheet && spriteSheet.GetMemberCollection<SpriteAnimation>() is { } animations) {
            while (this._availableAnimations.Count < this.MaxActiveAnimations) {
                foreach (var animation in animations) {
                    this._availableAnimations.Add(new PositionalSpriteAnimation(animation));
                }
            }
        }
    }

    /// <summary>
    /// Moves any running animations to the next frame.
    /// </summary>
    public void NextFrame() {
        for (var i = this._runningAnimations.Count - 1; i >= 0; i--) {
            var animation = this._runningAnimations[i];
            animation.TryNextFrame(out var isAnimationOver);
            if (isAnimationOver) {
                this._runningAnimations.Remove(animation);
                this._availableAnimations.Add(animation);
            }
        }
    }

    /// <summary>
    /// Updates emissions without animating. This will simply create new emissions according to <see cref="TimeBetweenEmissions" />.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="timeMultiplier">A time multiplier for incrementing <see cref="TimeBetweenEmissions" />.</param>
    /// <param name="trySpawn">A value indicating whether this update should attempt a new spawn if ready.</param>
    /// <param name="spawnWorldPosition">The new spawn position.</param>
    /// <param name="velocity">The velocity.</param>
    /// <param name="orientation">The orientation.</param>
    /// <param name="project">The project.</param>
    /// <param name="spawnCallback">A callback for when a new animation is spawned.</param>
    public void UpdateEmissions(
        FrameTime frameTime,
        float timeMultiplier,
        bool trySpawn,
        Vector2 spawnWorldPosition,
        Vector2 velocity,
        SpriteEffects orientation,
        IGameProject project,
        Action<PositionalSpriteAnimation>? spawnCallback) {
        if (this.TryGetAvailableAnimation(frameTime, timeMultiplier, trySpawn, out var newAnimation)) {
            this.UpdateEmissions(newAnimation, spawnWorldPosition, velocity, orientation);
            spawnCallback?.Invoke(newAnimation);
        }
    }

    /// <summary>
    /// Updates emissions without animating. This will simply create new emissions according to <see cref="TimeBetweenEmissions" />.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="timeMultiplier">A time multiplier for incrementing <see cref="TimeBetweenEmissions" />.</param>
    /// <param name="trySpawn">A value indicating whether this update should attempt a new spawn if ready.</param>
    /// <param name="spawnWorldPositionCallback">The callback to get a world position if there is a new animation.</param>
    /// <param name="velocity">The velocity.</param>
    /// <param name="orientation">The orientation.</param>
    /// <param name="project">The project.</param>
    /// <param name="spawnCallback">A callback for when a new animation is spawned.</param>
    public void UpdateEmissions(
        FrameTime frameTime,
        float timeMultiplier,
        bool trySpawn,
        Func<Vector2> spawnWorldPositionCallback,
        Vector2 velocity,
        SpriteEffects orientation,
        IGameProject project,
        Action<PositionalSpriteAnimation>? spawnCallback) {
        if (this.TryGetAvailableAnimation(frameTime, timeMultiplier, trySpawn, out var newAnimation)) {
            this.UpdateEmissions(newAnimation, spawnWorldPositionCallback.Invoke(), velocity, orientation);
            spawnCallback?.Invoke(newAnimation);
        }
    }

    private bool TryGetAvailableAnimation(FrameTime frameTime, float timeMultiplier, bool trySpawn, [NotNullWhen(true)] out PositionalSpriteAnimation? animation) {
        this.TimeBetweenEmissions.Increment(frameTime, timeMultiplier);

        if (trySpawn && this.TimeBetweenEmissions.State == TimerState.Finished && this._availableAnimations.Any()) {
            var index = this._randomizer.Next(0, this._availableAnimations.Count - 1);
            animation = this._availableAnimations[index];
        }
        else {
            animation = null;
        }

        return animation != null;
    }

    private void UpdateEmissions(
        PositionalSpriteAnimation newAnimation,
        Vector2 spawnWorldPosition,
        Vector2 velocity,
        SpriteEffects orientation) {
        var x = spawnWorldPosition.X + this.EmissionOffset.X + (-this.SpawnVariation.X * 0.5f + this._randomizer.NextSingle() * this.SpawnVariation.X);
        var y = spawnWorldPosition.Y + this.EmissionOffset.Y + (-this.SpawnVariation.Y * 0.5f + this._randomizer.NextSingle() * this.SpawnVariation.Y);
        newAnimation.Position = new Vector2(x, y);
        newAnimation.Velocity = velocity;
        newAnimation.Orientation = orientation;
        newAnimation.Reset();
        this._runningAnimations.Add(newAnimation);
        this.TimeBetweenEmissions.Restart();
    }
}