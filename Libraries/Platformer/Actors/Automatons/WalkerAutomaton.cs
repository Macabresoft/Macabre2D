namespace Macabresoft.Macabre2D.Libraries.Platformer.Automatons;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Defines ways walkers can move.
/// </summary>
public enum WalkerMovement {
    Idle,
    Walking
}

/// <summary>
/// A simple automaton that moves forward until it hits a wall, then it moves the opposite direction.
/// </summary>
public class WalkerAutomaton : PlatformerActor {
    private bool _avoidLedges = true;
    private WalkerMovement _currentMovement = WalkerMovement.Idle;
    private QueueableSpriteAnimator? _spriteAnimator;
    private float _velocity = 1f;

    /// <summary>
    /// Gets the idle animation reference.
    /// </summary>
    [DataMember(Order = 10, Name = "Idle Animation")]
    public SpriteAnimationReference IdleAnimationReference { get; } = new();

    /// <summary>
    /// Gets the walking animation reference.
    /// </summary>
    [DataMember(Order = 11, Name = "Walking Animation")]
    public SpriteAnimationReference WalkingAnimationReference { get; } = new();

    /// <summary>
    /// Gets or sets a value indicating whether or not this automaton should avoid ledges or if it should walk off of them.
    /// </summary>
    [DataMember]
    public bool AvoidsLedges {
        get => this._avoidLedges;
        set => this.Set(ref this._avoidLedges, value);
    }

    /// <summary>
    /// Gets or sets the velocity while walking.
    /// </summary>
    [DataMember]
    public float Velocity {
        get => this._velocity;
        set => this.Set(ref this._velocity, value);
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.IdleAnimationReference);
        this.Scene.Assets.ResolveAsset<SpriteSheetAsset, Texture2D>(this.WalkingAnimationReference);

        this._spriteAnimator = this.GetOrAddChild<QueueableSpriteAnimator>();
        this._spriteAnimator.RenderSettings.OffsetType = PixelOffsetType.Center;

        if (this.IdleAnimationReference.PackagedAsset is { } animation) {
            this._spriteAnimator.Play(animation, true);
        }
    }

    /// <inheritdoc />
    public override void Update(FrameTime frameTime, InputState inputState) {
        throw new NotImplementedException();
    }
}