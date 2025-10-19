namespace Macabresoft.Macabre2D.Framework; 

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A non-looping version of <see cref="QueueableSpriteAnimation" /> which includes a world space position.
/// </summary>
public class PositionalSpriteAnimation : QueueableSpriteAnimation {
    /// <summary>
    /// Initializes a new instance of the <see cref="PositionalSpriteAnimation" /> class.
    /// </summary>
    /// <param name="animation">The animation.</param>
    public PositionalSpriteAnimation(SpriteAnimation animation) : base(animation, AnimationLoopKind.None) {
    }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    public Vector2 Position { get; set; }
    
    /// <summary>
    /// Gets or sets the velocity.
    /// </summary>
    public Vector2 Velocity { get; set; }
    
    /// <summary>
    /// Gets or sets a value indicating whether this should be flipped horizontally when rendering.
    /// </summary>
    public SpriteEffects Orientation { get; set; }

    public override void Update(FrameTime frameTime, int millisecondsPerFrame, out bool isAnimationOver) {
        base.Update(frameTime, millisecondsPerFrame, out isAnimationOver);
        this.Position += this.Velocity * (float)frameTime.SecondsPassed;
    }
}