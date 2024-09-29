namespace Macabresoft.Macabre2D.Framework;

using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// A base renderable entity with all the benefits of being tileable.
/// </summary>
[Category(CommonCategories.Rendering)]
public abstract class RenderableTileableEntity : TileableEntity, IRenderableEntity {
    private int _renderOrder;
    private bool _shouldRender = true;

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
    public bool ShouldRender {
        get => this._shouldRender && this.IsEnabled;
        set => this.Set(ref this._shouldRender, value);
    }

    /// <inheritdoc />
    protected override void OnIsEnableChanged() {
        base.OnIsEnableChanged();
        this.RaisePropertyChanged(nameof(this.ShouldRender));
    }

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea);

    /// <inheritdoc />
    public abstract void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride);
}