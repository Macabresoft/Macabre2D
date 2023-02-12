namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// Interface for an entity which can be rendered.
/// </summary>
public interface IRenderableEntity : IBoundable, IEntity, IPixelSnappable {
    /// <summary>
    /// Gets a value indicating whether this instance is visible.
    /// </summary>
    /// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
    bool IsVisible { get; }

    /// <summary>
    /// Gets the render order.
    /// </summary>
    /// <value>The render order.</value>
    int RenderOrder => 0;

    /// <summary>
    /// Gets a value indicating whether or not this should be rendered when out of bounds.
    /// </summary>
    bool RenderOutOfBounds { get; }

    /// <summary>
    /// Renders this instance.
    /// </summary>
    /// <param name="frameTime">The frame time.</param>
    /// <param name="viewBoundingArea">The view bounding area.</param>
    void Render(FrameTime frameTime, BoundingArea viewBoundingArea);
}

/// <summary>
/// A <see cref="IEntity" /> which has a default implementation of
/// <see cref="IRenderableEntity" />.
/// </summary>
[Category(CommonCategories.Rendering)]
public abstract class RenderableEntity : Entity, IRenderableEntity {
    private bool _isVisible = true;
    private int _renderOrder;

    /// <inheritdoc />
    public abstract event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public abstract BoundingArea BoundingArea { get; }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public bool IsVisible {
        get => this._isVisible && this.IsEnabled;
        set => this.Set(ref this._isVisible, value);
    }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public PixelSnap PixelSnap { get; set; } = PixelSnap.Inherit;

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public int RenderOrder {
        get => this._renderOrder;
        set => this.Set(ref this._renderOrder, value);
    }

    /// <inheritdoc />
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public bool RenderOutOfBounds { get; set; }

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(IEnableable.IsEnabled) && this._isVisible) {
            this.RaisePropertyChanged(nameof(this.IsVisible));
        }
    }
}