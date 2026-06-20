namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabre2D.Project.Common;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for an entity which can be rendered.
/// </summary>
public interface IRenderableEntity : IBaseRenderable {

    /// <summary>
    /// Called when <see cref="ShouldRender" /> changes.
    /// </summary>
    event EventHandler? ShouldRenderChanged;

    /// <summary>
    /// Gets or sets a value indicating whether this instance should render.
    /// </summary>
    bool ShouldRender { get; set; }

    /// <summary>
    /// Renders this instance.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="viewBoundingArea">The view bounding area.</param>
    void Render(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <summary>
    /// Renders this instance with a specific color.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="viewBoundingArea">The view bounding area.</param>
    /// <param name="colorOverride">The color override.</param>
    void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride);
}

/// <summary>
/// A <see cref="IEntity" /> which has a default implementation of
/// <see cref="IRenderableEntity" />.
/// </summary>
[Category(CommonCategories.Rendering)]
public abstract class RenderableEntity : Entity, IRenderableEntity {
    [DataMember(Name = nameof(ShouldRender))]
    private bool _shouldRender = true;

    /// <inheritdoc />
    public abstract event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? ShouldRenderChanged;

    /// <inheritdoc />
    public abstract BoundingArea BoundingArea { get; }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    [PredefinedInteger(PredefinedIntegerKind.RenderOrder)]
    public int RenderOrder { get; set; }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public bool RenderOutOfBounds { get; set; }

    /// <inheritdoc />
    public abstract RenderPriority RenderPriority { get; set; }

    /// <inheritdoc />
    [Category(CommonCategories.Rendering)]
    public bool ShouldRender {
        get => this._shouldRender && this.CheckShouldRender();
        set {
            if (this.Set(ref this._shouldRender, value)) {
                this.RaiseShouldRenderChanged();
            }
        }
    }

    /// <summary>
    /// Gets the internal value of <see cref="ShouldRender" />. This can return true even if <see cref="ShouldRender" /> returns false.
    /// </summary>
    protected bool InternalShouldRender => this._shouldRender;

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride);

    /// <summary>
    /// A modifiable check on whether <see cref="ShouldRender" /> should return true.
    /// </summary>
    /// <returns>A value indicating whether <see cref="ShouldRender" /> should return true.</returns>
    protected virtual bool CheckShouldRender() => this.IsEnabled;

    /// <inheritdoc />
    protected override void OnIsEnableChanged() {
        base.OnIsEnableChanged();

        if (this.ShouldRender) {
            this.ShouldRenderChanged.SafeInvoke(this);
        }
    }

    /// <summary>
    /// Raises the <see cref="ShouldRenderChanged" /> event.
    /// </summary>
    protected virtual void RaiseShouldRenderChanged() {
        this.ShouldRenderChanged.SafeInvoke(this);
    }
}