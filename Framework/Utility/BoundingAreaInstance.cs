namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// An instance that contains a <see cref="BoundingArea" /> that it maintains via height, width, and offset options.
/// </summary>
public sealed class BoundingAreaInstance : IBoundable, IGameObjectReference {
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private IEntity _entity = EmptyObject.Entity;
    private float _height;
    private ICommonMeasurements _measurements = EmptyObject.Instance;
    private bool _shouldRespondToOffsetChange = true;
    private float _width;

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
        get => this._height;
        set {
            this._height = value;
            this.OnSizeChanged();
        }
    }

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    [DataMember]
    public float Width {
        get => this._width;
        set {
            this._width = value;
            this.OnSizeChanged();
        }
    }

    /// <inheritdoc />
    public void Deinitialize() {
        this.OffsetOptions.PropertyChanged -= this.OffsetOptions_PropertyChanged;
        this._entity.TransformChanged -= this.Entity_TransformChanged;
        this._entity = EmptyObject.Entity;
        this._measurements = EmptyObject.Instance;
    }

    /// <inheritdoc />
    public void Initialize(IGame game, IScene scene, IEntity entity) {
        try {
            this._shouldRespondToOffsetChange = false;
            this._entity = entity;
            this._measurements = game.Measurements;

            this.OffsetOptions.Initialize(this.CreateSize);
            this.OffsetOptions.PropertyChanged += this.OffsetOptions_PropertyChanged;
            this._entity.TransformChanged += this.Entity_TransformChanged;
        }
        finally {
            this._shouldRespondToOffsetChange = true;
        }
    }

    /// <summary>
    /// Sets the bounding area of this instance to match another bounding area.
    /// </summary>
    /// <param name="boundingArea">The bounding area to match.</param>
    public void SetBoundingArea(BoundingArea boundingArea) {
        this.SetBoundingArea(boundingArea.Width, boundingArea.Height, boundingArea.Minimum - this._entity.WorldPosition);
    }

    /// <summary>
    /// Sets the bounding area of this instance to match a collider's bounding area.
    /// </summary>
    /// <param name="collider">The collider to match.</param>
    public void SetBoundingArea(Collider collider) {
        this.SetBoundingArea(collider.BoundingArea.Width, collider.BoundingArea.Height, collider.Offset);
    }

    /// <summary>
    /// Sets the bounding area of this instance to match a width, height, and offset.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="offset">The offset.</param>
    public void SetBoundingArea(float width, float height, Vector2 offset) {
        try {
            this._shouldRespondToOffsetChange = false;
            this._width = width;
            this._height = height;
            this.OffsetOptions.Offset = offset;
            this.OnSizeChanged();
        }
        finally {
            this._shouldRespondToOffsetChange = true;
        }
    }

    private BoundingArea CreateBoundingArea() => this.OffsetOptions.CreateBoundingArea(this._entity);

    private Vector2 CreateSize() => new Vector2(this.Width, this.Height) * this._measurements.PixelsPerUnit;

    private void Entity_TransformChanged(object? sender, EventArgs e) {
        this.OnSizeChanged();
    }

    private void OffsetOptions_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (this._shouldRespondToOffsetChange && e.PropertyName == nameof(this.OffsetOptions.Offset)) {
            this.OnSizeChanged();
        }
    }

    private void OnSizeChanged() {
        try {
            this._shouldRespondToOffsetChange = false;
            this.OffsetOptions.InvalidateSize();
            this._boundingArea.Reset();
            this.BoundingAreaChanged.SafeInvoke(this);
        }
        finally {
            this._shouldRespondToOffsetChange = true;
        }
    }
}