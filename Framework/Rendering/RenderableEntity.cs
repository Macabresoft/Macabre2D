namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for an entity which can be rendered.
/// </summary>
public interface IRenderableEntity : IBoundableEntity {

    /// <summary>
    /// Called when <see cref="ShouldRender" /> changes.
    /// </summary>
    event EventHandler? ShouldRenderChanged;

    /// <summary>
    /// Gets or sets the render order.
    /// </summary>
    int RenderOrder { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this should be rendered when out of bounds.
    /// </summary>
    bool RenderOutOfBounds { get; set; }

    /// <summary>
    /// Gets the render priority.
    /// </summary>
    RenderPriority RenderPriority { get; set; }

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
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public bool ShouldRender {
        get => this._shouldRender && this.IsEnabled;
        set {
            if (this.Set(ref this._shouldRender, value)) {
                this.ShouldRenderChanged.SafeInvoke(this);
            }
        }
    }

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride);

    /// <inheritdoc />
    protected override void OnIsEnableChanged() {
        base.OnIsEnableChanged();

        if (this._shouldRender) {
            this.ShouldRenderChanged.SafeInvoke(this);
        }
    }
}