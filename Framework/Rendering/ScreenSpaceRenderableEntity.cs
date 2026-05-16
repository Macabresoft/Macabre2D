namespace Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Macabre2D.Project.Common;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// A <see cref="RenderableEntity" /> that can also be rendered in screenspace via the <see cref="IScreenSpaceRenderer" /> interface.
/// </summary>
public abstract class ScreenSpaceRenderableEntity : RenderableEntity, IScreenSpaceRenderer {

    /// <inheritdoc />
    public event EventHandler? ShouldRenderInScreenSpaceChanged;

    /// <summary>
    /// Gets a value indicating whether this should render in screen space if the current font culture requires screen space rendering.
    /// </summary>
    [DataMember]
    public bool AllowScreenSpaceRendering {
        get;
        set {
            if (value != field) {
                field = value;
                this.ResetShouldRenderInScreenSpace();
            }
        }
    }

    /// <inheritdoc />
    public bool ShouldRenderInScreenSpace {
        get;
        private set {
            if (value != field) {
                field = value;
                this.OnShouldRenderInScreenSpaceChanged();
            }
        }
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        this.Game.CultureChanged -= this.Game_CultureChanged;

        base.Deinitialize();
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.ResetShouldRenderInScreenSpace();
        this.Game.CultureChanged += this.Game_CultureChanged;
    }

    /// <inheritdoc />
    public abstract void RenderInScreenSpace(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <inheritdoc />
    public abstract void RenderInScreenSpace(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride);

    /// <inheritdoc />
    protected override bool CheckShouldRender() => base.CheckShouldRender() && !this.ShouldRenderInScreenSpace;

    /// <summary>
    /// Called when <see cref="ShouldRenderInScreenSpace" /> changes.
    /// </summary>
    protected virtual void OnShouldRenderInScreenSpaceChanged() {
        this.ShouldRenderInScreenSpaceChanged.SafeInvoke(this);
        this.RaiseShouldRenderChanged();
    }

    private void Game_CultureChanged(object? sender, ResourceCulture e) {
        this.ResetShouldRenderInScreenSpace();
    }

    private void ResetShouldRenderInScreenSpace() {
        this.ShouldRenderInScreenSpace = this.AllowScreenSpaceRendering && this.Game.CurrentCultureRendersTextInScreenSpace;
    }
}