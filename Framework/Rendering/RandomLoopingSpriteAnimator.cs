﻿namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// Loops all the animations associated with a <see cref="SpriteSheet" />, choosing a new animation every time the current animation finishes.
/// </summary>
public class RandomLoopingSpriteAnimator : BaseSpriteAnimator {
    private readonly List<QueueableSpriteAnimation> _availableAnimations = new();
    private readonly Random _random = new();
    private QueueableSpriteAnimation? _currentAnimation;

    /// <summary>
    /// Gets the sprite sheet reference.
    /// </summary>
    [DataMember(Order = 10, Name = "Sprite Sheet")]
    public SpriteSheetReference SpriteSheetReference { get; } = new();

    [DataMember]
    public bool IsRandom { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether this should start playing by default.
    /// </summary>
    [DataMember]
    public bool StartPlaying { get; set; } = true;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.SpriteSheetReference.PropertyChanged -= this.AnimationReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        if (this.TryResetAnimations()) {
            this.TryStart();
        }

        this.SpriteSheetReference.PropertyChanged += this.AnimationReference_PropertyChanged;
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.SpriteSheetReference;
    }

    /// <inheritdoc />
    protected override QueueableSpriteAnimation? GetCurrentAnimation() => this._currentAnimation;

    /// <inheritdoc />
    protected override void HandleAnimationFinished() {
        this.TryAssignNewAnimation();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(this.IsEnabled)) {
            this.TryStart();
        }
    }

    private void AnimationReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(SpriteAnimationReference.Asset) or nameof(SpriteAnimationReference.PackagedAsset) or nameof(Framework.SpriteSheet.SpriteSize)) {
            if (this.TryResetAnimations()) {
                this.TryStart();
            }
        }
    }

    private bool TryAssignNewAnimation() {
        if (this._availableAnimations.Count != 0) {
            if (this.IsRandom) {
                var index = this._random.Next(0, this._availableAnimations.Count);
                this._currentAnimation = this._availableAnimations[index];
            }
            else if (this._currentAnimation != null) {
                var index = this._availableAnimations.IndexOf(this._currentAnimation) + 1;
                if (index >= this._availableAnimations.Count) {
                    index = 0;
                }

                this._currentAnimation = this._availableAnimations[index];
            }
            else {
                this._currentAnimation = this._availableAnimations[0];
            }

            this._currentAnimation?.Reset();
            return true;
        }

        return false;
    }

    private bool TryResetAnimations() {
        this._availableAnimations.Clear();

        if (this.SpriteSheetReference.Asset is { } spriteSheet) {
            foreach (var animation in spriteSheet.GetAssets<SpriteAnimation>()) {
                this._availableAnimations.Add(new QueueableSpriteAnimation(animation, false));
            }
        }

        return this.TryAssignNewAnimation();
    }

    private void TryStart() {
        if (this.StartPlaying && this.IsEnabled) {
            this.Play();
        }
    }
}