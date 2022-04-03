namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A sprite animator that loops a single animation.
/// </summary>
public sealed class LoopingSpriteAnimator : BaseSpriteAnimator {
    private bool _startPlaying = true;

    /// <summary>
    /// Gets the animation reference.
    /// </summary>
    [DataMember(Order = 10, Name = "Animation")]
    public SpriteAnimationReference AnimationReference { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether or not this should start playing by default.
    /// </summary>
    public bool StartPlaying {
        get => this._startPlaying;
        set => this.Set(ref this._startPlaying, value);
    }

    /// <inheritdoc />
    protected override SpriteSheetAsset? SpriteSheet => this.AnimationReference?.Asset;

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.AnimationReference.PropertyChanged -= this.AnimationReference_PropertyChanged;
        this.AnimationReference.Initialize(this.Scene.Assets);
        this.ResetStep();
        this.TryStart();
        this.AnimationReference.PropertyChanged += this.AnimationReference_PropertyChanged;
    }

    /// <inheritdoc />
    protected override SpriteAnimation? GetSpriteAnimation() {
        if (this.CurrentSpriteIndex == null) {
            this.ResetStep();
        }

        return this.AnimationReference.PackagedAsset;
    }

    /// <inheritdoc />
    protected override SpriteAnimation? HandleAnimationFinished() {
        this.ResetStep();
        return this.AnimationReference.PackagedAsset;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(this.IsEnabled)) {
            this.TryStart();
        }
    }

    private void AnimationReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName is nameof(SpriteAnimationReference.Asset) or nameof(SpriteAnimationReference.PackagedAsset) or nameof(SpriteSheetAsset.SpriteSize)) {
            this.TryStart();
        }
    }

    private void ResetStep() {
        if (this.AnimationReference.PackagedAsset is { } animation && animation.Steps.FirstOrDefault() is { } step) {
            this.CurrentSpriteIndex = step.SpriteIndex;
            this.CurrentStepIndex = 0;
        }
    }

    private void TryStart() {
        if (this.StartPlaying && this.IsEnabled) {
            this.Play();
        }
    }
}