namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;

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
        set => this.Set(ref this._isVisible, value, this.IsEnabled);
    }

    /// <inheritdoc />
    [DataMember]
    public int RenderOrder {
        get => this._renderOrder;
        set => this.Set(ref this._renderOrder, value);
    }

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName == nameof(IEntity.IsEnabled) && this._isVisible) {
            this.RaisePropertyChanged(nameof(this.IsVisible));
        }
    }
}