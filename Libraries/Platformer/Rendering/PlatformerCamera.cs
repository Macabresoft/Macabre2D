namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A <see cref="ICamera" /> for platformers.
/// </summary>
public class PlatformerCamera : Camera {
    private float _horizontalDistanceAllowed = 2f;
    private float _lerpSpeed = 5f;
    private float _verticalDistanceAllowed = 4f;

    /// <summary>
    /// Gets or sets the background layer.
    /// </summary>
    [DataMember]
    public Layers BackgroundLayer { get; set; }

    /// <summary>
    /// Gets or sets the foreground layer.
    /// </summary>
    [DataMember]
    public Layers ForegroundLayer { get; set; }

    /// <summary>
    /// Gets or sets the horizontal distance allowed from the player.
    /// </summary>
    [DataMember]
    public float HorizontalDistanceAllowed {
        get => this._horizontalDistanceAllowed;
        set => this.Set(ref this._horizontalDistanceAllowed, value);
    }

    /// <summary>
    /// Gets or sets the lerp speed.
    /// </summary>
    [DataMember]
    public float LerpSpeed {
        get => this._lerpSpeed;
        set => this.Set(ref this._lerpSpeed, value);
    }

    /// <summary>
    /// Gets or sets the vertical distance allowed from the player.
    /// </summary>
    [DataMember]
    public float VerticalDistanceAllowed {
        get => this._verticalDistanceAllowed;
        set => this.Set(ref this._verticalDistanceAllowed, value);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, SpriteBatch? spriteBatch, IReadOnlyCollection<IRenderableEntity> entities) {
        if (this.ShaderReference.Asset?.Content is { } effect) {
            var backgroundEntities = entities.Where(x => x.Layers.HasFlag(this.BackgroundLayer)).ToList();
            if (backgroundEntities.Any()) {
                spriteBatch?.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    this.SamplerState,
                    null,
                    RasterizerState.CullNone,
                    null,
                    this.GetViewMatrix());

                foreach (var entity in backgroundEntities) {
                    entity.Render(frameTime, this.BoundingArea);
                }

                spriteBatch?.End();
            }

            var foregroundEntities = entities.Where(x => x.Layers.HasFlag(this.ForegroundLayer)).ToList();
            if (foregroundEntities.Any()) {
                var offsetTransform = new Transform(this.Transform.Position + new Vector2(this.Settings.InversePixelsPerUnit, -this.Settings.InversePixelsPerUnit), this.Transform.Scale);

                // TODO: customize this
                //effect.Parameters["FillColor"].SetValue(Color.HotPink.ToVector3());
                spriteBatch?.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    this.SamplerState,
                    null,
                    RasterizerState.CullNone,
                    effect,
                    this.GetViewMatrix(offsetTransform));

                foreach (var entity in foregroundEntities) {
                    entity.Render(frameTime, this.BoundingArea);
                }

                spriteBatch?.End();

                spriteBatch?.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    this.SamplerState,
                    null,
                    RasterizerState.CullNone,
                    null,
                    this.GetViewMatrix());

                foreach (var entity in foregroundEntities) {
                    entity.Render(frameTime, this.BoundingArea);
                }

                spriteBatch?.End();
            }
        }
        else {
            base.Render(frameTime, spriteBatch, entities);
        }
    }

    /// <summary>
    /// Updates the position according to the specified actor state.
    /// </summary>
    /// <param name="previousState">The previous state.</param>
    /// <param name="currentState">The current state.</param>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="isOnPlatform">A value indicating whether or not the actor is on a platform.</param>
    public void UpdateDesiredPosition(ActorState previousState, ActorState currentState, FrameTime frameTime, bool isOnPlatform) {
        var x = this.LocalPosition.X;
        if (this.HorizontalDistanceAllowed != 0f) {
            if (isOnPlatform) {
                if (x != 0f) {
                    x = this.Lerp(this.LocalPosition.X, 0f, (float)frameTime.SecondsPassed * this._lerpSpeed);
                }
            }
            else {
                x = Math.Clamp(this.LocalPosition.X + currentState.Position.X - previousState.Position.X, -this.HorizontalDistanceAllowed, this.HorizontalDistanceAllowed);
            }
        }

        var y = this.LocalPosition.Y;
        if (this.VerticalDistanceAllowed != 0f) {
            if (isOnPlatform) {
                if (y != 0f) {
                    y = this.Lerp(this.LocalPosition.Y, 0f, (float)frameTime.SecondsPassed * this._lerpSpeed);
                }
            }
            else {
                var yMovement = currentState.Position.Y - previousState.Position.Y;
                if (Math.Abs(yMovement) > 0.001f) {
                    if (currentState.Velocity.Y < 0f && Math.Abs(currentState.Velocity.Y - previousState.Velocity.Y) < 0.001f) {
                        y = this.Lerp(this.LocalPosition.Y, -this.VerticalDistanceAllowed, (float)frameTime.SecondsPassed);
                    }
                    else {
                        y = Math.Clamp(this.LocalPosition.Y + yMovement, -this.VerticalDistanceAllowed, this.VerticalDistanceAllowed);
                    }
                }
                else if (y != 0f) {
                    y = this.Lerp(this.LocalPosition.Y, 0f, (float)frameTime.SecondsPassed * this._lerpSpeed);
                }
            }
        }

        this.LocalPosition = new Vector2(x, y);
    }

    private float Lerp(float current, float desired, float amount) {
        var result = current + (desired - current) * amount;

        if (Math.Abs(result - desired) < this.Settings.InversePixelsPerUnit * 0.5f) {
            result = desired;
        }

        return result;
    }
}