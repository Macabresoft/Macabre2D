namespace Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// An instance that contains a <see cref="BoundingArea" /> that it maintains via height, width, and offset options.
/// </summary>
public sealed class BoundingAreaInstance : IBoundable, IGameObjectReference {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private IEntity _entity = EmptyObject.Entity;
    private ICommonMeasurements _measurements = EmptyObject.Instance;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingAreaInstance" /> class.
    /// </summary>
    public BoundingAreaInstance() {
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <inheritdoc />
    public BoundingArea BoundingArea => this._boundingArea.Value;

    /// <summary>
    /// Gets the offset.
    /// </summary>
    [DataMember]
    public OffsetOptions OffsetOptions { get; } = new();

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    [DataMember]
    public float Height {
        get;
        set {
            field = value;
            this.OnSizeChanged();
        }
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    [DataMember]
    public float Width {
        get;
        set {
            field = value;
            this.OnSizeChanged();
        }
    }

    /// <inheritdoc />
    public void Deinitialize() {
        this._entity.TransformChanged -= this.Entity_TransformChanged;
        this._entity = EmptyObject.Entity;
        this._measurements = EmptyObject.Instance;
    }

    /// <inheritdoc />
    public void Initialize(IGame game, IScene scene, IEntity entity) {
        this._entity = entity;
        this._measurements = game.Measurements;

        this.OffsetOptions.Initialize(this.CreateSize);
        this._entity.TransformChanged += this.Entity_TransformChanged;
    }

    private BoundingArea CreateBoundingArea() => this.OffsetOptions.CreateBoundingArea(this._entity);

    private Vector2 CreateSize() => new Vector2(this.Width, this.Height) * this._measurements.PixelsPerUnit;

    private void Entity_TransformChanged(object? sender, EventArgs e) {
        this.OnSizeChanged();
    }

    private void OnSizeChanged() {
        this.OffsetOptions.InvalidateSize();
        this._boundingArea.Reset();
        this.BoundingAreaChanged.SafeInvoke(this);
    }
}