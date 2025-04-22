namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// A base renderable entity with all the benefits of being tileable.
/// </summary>
[Category(CommonCategories.Rendering)]
public abstract class RenderableTileableEntity : TileableEntity, IRenderableEntity {
    private bool _shouldRender = true;

    /// <inheritdoc />
    public event EventHandler? ShouldRenderChanged;

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
    [DataMember]
    [Category(CommonCategories.Rendering)]
    public RenderPriority RenderPriority { get; set; }

    /// <inheritdoc />
    [DataMember]
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
        this.RaisePropertyChanged(nameof(this.ShouldRender));
    }
}