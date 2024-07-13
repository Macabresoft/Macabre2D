namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// A base renderable tile map.
/// </summary>
[Category(CommonCategories.Rendering)]
public abstract class RenderableTileMap : TileableEntity, IRenderableEntity {
    private bool _isVisible = true;
    private int _renderOrder;

    /// <inheritdoc />
    [DataMember]
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
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride);

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(IEntity.IsEnabled) && this._isVisible) {
            this.RaisePropertyChanged(nameof(this.IsVisible));
        }
    }
}