namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// A sprite animator that loops a single animation.
/// </summary>
public class LoopingSpriteAnimator : BaseSpriteAnimator {
    private QueueableSpriteAnimation? _queueableAnimation;

    /// <summary>
    /// Gets the animation reference.
    /// </summary>
    [DataMember(Order = 10, Name = "Animation")]
    public SpriteAnimationReference AnimationReference { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether this should start playing by default.
    /// </summary>
    [DataMember]
    public bool StartPlaying { get; set; } = true;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.AnimationReference.PropertyChanged -= this.AnimationReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        if (this.TryResetAnimation()) {
            this._queueableAnimation?.Reset();
            this.TryStart();
        }

        this.AnimationReference.PropertyChanged += this.AnimationReference_PropertyChanged;
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.AnimationReference;
    }

    /// <inheritdoc />
    protected override QueueableSpriteAnimation? GetCurrentAnimation() => this._queueableAnimation;

    /// <inheritdoc />
    protected override void HandleAnimationFinished() {
        if (this._queueableAnimation != null) {
            var millisecondsPassed = this._queueableAnimation.MillisecondsPassed;
            this._queueableAnimation.Reset();
            this._queueableAnimation.MillisecondsPassed = millisecondsPassed;
        }
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
            if (this.TryResetAnimation()) {
                this.TryStart();
            }
        }
    }

    private bool TryResetAnimation() {
        if (this.AnimationReference.PackagedAsset is { } animation) {
            this._queueableAnimation = new QueueableSpriteAnimation(this.AnimationReference.PackagedAsset, true);
        }
        else {
            this._queueableAnimation = null;
        }

        return this._queueableAnimation != null;
    }

    private void TryStart() {
        if (this.StartPlaying && this.IsEnabled) {
            this.Play();
        }
    }
}