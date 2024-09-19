namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for an entity which can be rendered.
/// </summary>
public interface IRenderableEntity : IBoundable, IEntity, IPixelSnappable {

    /// <summary>
    /// Gets the render order.
    /// </summary>
    int RenderOrder => 0;

    /// <summary>
    /// Gets or sets a value indicating whether this should be rendered when out of bounds.
    /// </summary>
    bool RenderOutOfBounds { get; set; }

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
    private int _renderOrder;
    private bool _shouldRender = true;

    /// <inheritdoc />
    public abstract event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public abstract BoundingArea BoundingArea { get; }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public PixelSnap PixelSnap { get; set; } = PixelSnap.Inherit;

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    [PredefinedInteger(PredefinedIntegerKind.RenderOrder)]
    public int RenderOrder {
        get => this._renderOrder;
        set => this.Set(ref this._renderOrder, value);
    }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public bool RenderOutOfBounds { get; set; }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public bool ShouldRender {
        get => this._shouldRender && this.IsEnabled;
        set => this.Set(ref this._shouldRender, value);
    }

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride);

    /// <inheritdoc />
    protected override void OnIsEnableChanged() {
        base.OnIsEnableChanged();

        if (this._shouldRender) {
            this.RaisePropertyChanged(nameof(this.ShouldRender));
        }
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IEnableable.IsEnabled) && this._shouldRender) {
            this.RaisePropertyChanged(nameof(this.ShouldRender));
        }
    }
}