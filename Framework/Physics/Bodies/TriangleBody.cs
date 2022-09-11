namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// Enum for the peak position for a <see cref="TriangleBody" />.
/// </summary>
public enum TrianglePeakPosition {
    Left,
    Center,
    Right
}

/// <summary>
/// A base for triangle bodies.
/// </summary>
public class TriangleBody : PhysicsBody {
    private readonly LineCollider _bottomCollider = new();
    private readonly ResettableLazy<BoundingArea> _boundingArea;
    private readonly LineCollider _leftCollider = new();
    private readonly LineCollider _rightCollider = new();
    private float _height = 1f;
    private TrianglePeakPosition _peakPosition = TrianglePeakPosition.Center;
    private float _width = 1f;

    /// <summary>
    /// Initializes a new instance of <see cref="TriangleBody" />.
    /// </summary>
    protected TriangleBody() : base() {
        this.OverrideLayersBottomEdge.PropertyChanged += this.OnLayerOverrideChanged;
        this.OverrideLayersLeftEdge.PropertyChanged += this.OnLayerOverrideChanged;
        this.OverrideLayersRightEdge.PropertyChanged += this.OnLayerOverrideChanged;
        this._boundingArea = new ResettableLazy<BoundingArea>(this.CreateBoundingArea);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="TrapezoidBody" />.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    public TriangleBody(float width, float height) : this() {
        this._width = width;
        this._height = height;
    }

    /// <inheritdoc />
    public override BoundingArea BoundingArea => this._boundingArea.Value;

    /// <inheritdoc />
    public override bool HasCollider => this.GetColliders().Any();

    /// <summary>
    /// Gets the bottom edge's overriden layer.
    /// </summary>
    [DataMember(Name = "Bottom Layers", Order = 102)]
    public LayersOverride OverrideLayersBottomEdge { get; } = new();

    /// <summary>
    /// Gets the left edge's overriden layer.
    /// </summary>
    [DataMember(Name = "Left Layers", Order = 100)]
    public LayersOverride OverrideLayersLeftEdge { get; } = new();

    /// <summary>
    /// Gets the right edge's overriden layer.
    /// </summary>
    [DataMember(Name = "Right Layers", Order = 101)]
    public LayersOverride OverrideLayersRightEdge { get; } = new();

    /// <summary>
    /// Gets or sets the height. Setting this is fairly expensive and should be avoided during
    /// runtime if possible.
    /// </summary>
    [DataMember]
    public float Height {
        get => this._height;
        set {
            if (value > 0 && Math.Abs(value - this.Height) > Defaults.FloatComparisonTolerance) {
                this._height = value;
                this.ResetColliders();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the peak position of the triangle. Setting this is fairly expensive and should be avoided during
    /// runtime if possible.
    /// </summary>
    [DataMember]
    public TrianglePeakPosition PeakPosition {
        get => this._peakPosition;
        set {
            if (this.Set(ref this._peakPosition, value)) {
                this.ResetColliders();
            }
        }
    }

    /// <summary>
    /// Gets or sets the width. Setting this is fairly expensive and should be avoided during
    /// runtime if possible.
    /// </summary>
    [DataMember]
    public float Width {
        get => this._width;
        set {
            if (value > 0 && Math.Abs(value - this.Width) > Defaults.FloatComparisonTolerance) {
                this._width = value;
                this.ResetColliders();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <inheritdoc />
    public override IEnumerable<Collider> GetColliders() {
        if (this._width > 0f && this._height > 0f) {
            var colliders = new List<Collider>();

            if (!this.OverrideLayersLeftEdge.IsEnabled || this.OverrideLayersLeftEdge.Value != Layers.None) {
                colliders.Add(this._leftCollider);
            }

            if (!this.OverrideLayersRightEdge.IsEnabled || this.OverrideLayersRightEdge.Value != Layers.None) {
                colliders.Add(this._rightCollider);
            }

            if (!this.OverrideLayersBottomEdge.IsEnabled || this.OverrideLayersBottomEdge.Value != Layers.None) {
                colliders.Add(this._bottomCollider);
            }

            return colliders;
        }

        return Enumerable.Empty<Collider>();
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.ResetColliders();
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        base.OnPropertyChanged(sender, e);

        if (e.PropertyName is nameof(ITransformable.Transform)) {
            this.ResetColliders();
        }
    }

    private BoundingArea CreateBoundingArea() {
        return BoundingArea.Combine(
            this._leftCollider.BoundingArea,
            this._rightCollider.BoundingArea,
            this._bottomCollider.BoundingArea);
    }

    private void OnLayerOverrideChanged(object? sender, PropertyChangedEventArgs e) {
        this.ResetColliders();
    }

    private void ResetColliders() {
        if (this._width > 0f && this._height > 0f) {
            switch (this.PeakPosition) {
                case TrianglePeakPosition.Center: {
                    var halfWidth = 0.5f * this._width;
                    this._leftCollider.Start = Vector2.Zero;
                    this._leftCollider.End = new Vector2(halfWidth, this._height);
                    break;
                }
                case TrianglePeakPosition.Left:
                    this._leftCollider.Start = Vector2.Zero;
                    this._leftCollider.End = new Vector2(0f, this._height);
                    break;
                default:
                    this._leftCollider.Start = Vector2.Zero;
                    this._leftCollider.End = new Vector2(this._width, this._height);
                    break;
            }

            this._rightCollider.Start = this._leftCollider.End;
            this._rightCollider.End = new Vector2(this._width, 0f);

            this._bottomCollider.Start = this._leftCollider.Start;
            this._bottomCollider.End = this._rightCollider.End;


            this._leftCollider.Layers = this.OverrideLayersLeftEdge.IsEnabled ? this.OverrideLayersLeftEdge.Value : this.Layers;
            this._rightCollider.Layers = this.OverrideLayersRightEdge.IsEnabled ? this.OverrideLayersRightEdge.Value : this.Layers;
            this._bottomCollider.Layers = this.OverrideLayersBottomEdge.IsEnabled ? this.OverrideLayersBottomEdge.Value : this.Layers;

            this._leftCollider.Initialize(this);
            this._rightCollider.Initialize(this);
            this._bottomCollider.Initialize(this);
            this._boundingArea.Reset();
        }
    }
}