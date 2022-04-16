﻿namespace Macabresoft.Macabre2D.Libraries.Platformer;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// A camera which uses a color fill shader to add a drop shadow to foreground elements.
/// </summary>
public class DropShadowPlatformerCamera : PlatformerCamera {
    /// <summary>
    /// Gets the shader reference.
    /// </summary>
    [DataMember]
    [Category("Drop Shadow")]
    public SingleColorShaderReference DropShadowShader { get; } = new();

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

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.DropShadowShader.Initialize(this.Scene.Assets);
    }

    /// <inheritdoc />
    public override void Render(FrameTime frameTime, SpriteBatch? spriteBatch, IReadOnlyCollection<IRenderableEntity> entities) {
        if (this.ForegroundLayer != Layers.None || this.BackgroundLayer != Layers.None) {
            if (this.BackgroundLayer != Layers.None) {
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
            }

            if (this.ForegroundLayer != Layers.None) {
                var foregroundEntities = entities.Where(x => x.Layers.HasFlag(this.ForegroundLayer)).ToList();
                if (foregroundEntities.Any()) {
                    var offsetTransform = new Transform(this.Transform.Position + new Vector2(this.Settings.InversePixelsPerUnit, -this.Settings.InversePixelsPerUnit), this.Transform.Scale);

                    spriteBatch?.Begin(
                        SpriteSortMode.Deferred,
                        BlendState.AlphaBlend,
                        this.SamplerState,
                        null,
                        RasterizerState.CullNone,
                        this.DropShadowShader.GetEffect(),
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
                        this.ShaderReference.Asset?.Content,
                        this.GetViewMatrix());

                    foreach (var entity in foregroundEntities) {
                        entity.Render(frameTime, this.BoundingArea);
                    }

                    spriteBatch?.End();
                }
            }
        }
        else {
            base.Render(frameTime, spriteBatch, entities);
        }
    }
}