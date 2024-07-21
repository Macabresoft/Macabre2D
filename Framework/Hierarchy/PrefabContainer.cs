namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// An entity which loads a <see cref="PrefabAsset" />.
/// </summary>
public sealed class PrefabContainer : Entity, IRenderableEntity {
    private bool _isVisible;
    private IEntity? _prefabChild;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrefabContainer" /> class.
    /// </summary>
    public PrefabContainer() : base() {
    }

    /// <inheritdoc />
    public PixelSnap PixelSnap { get; } = PixelSnap.No;

    /// <summary>
    /// Gets a reference to the prefab this entity contains.
    /// </summary>
    [DataMember(Order = 0, Name = "Prefab")]
    public PrefabReference PrefabReference { get; } = new();

    /// <inheritdoc />
    public BoundingArea BoundingArea { get; private set; } = BoundingArea.Empty;

    /// <inheritdoc />
    public bool IsVisible {
        get => this._isVisible && this.IsEnabled;
        set => this.Set(ref this._isVisible, value);
    }

    /// <inheritdoc />
    public bool RenderOutOfBounds { get; set; }

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.PrefabReference.PropertyChanged -= this.PrefabReference_PropertyChanged;
        this.DeinitializeChild();
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.IsVisible = BaseGame.IsDesignMode;
        this.Reset();
        this.PrefabReference.PropertyChanged += this.PrefabReference_PropertyChanged;
    }

    /// <summary>
    /// Gets a value indicating whether or not the other entity is a descendent of this container's prefab.
    /// </summary>
    /// <param name="otherEntity">The other entity.</param>
    /// <returns>A value indicating whether the other entity is a descendent of this container's prefab.</returns>
    public bool IsPartOfPrefab(IEntity otherEntity) => this._prefabChild != null && otherEntity.IsDescendentOf(this._prefabChild);

    /// <inheritdoc />
    public void Render(FrameTime frameTime, BoundingArea viewBoundingArea) {
        this.Render(frameTime, viewBoundingArea, Color.White);
    }

    /// <inheritdoc />
    public void Render(FrameTime frameTime, BoundingArea viewBoundingArea, Color colorOverride) {
        if (BaseGame.IsDesignMode && this._prefabChild != null) {
            var renderables = this._prefabChild.GetDescendants<IRenderableEntity>().OrderBy(x => x.RenderOrder);

            foreach (var renderable in renderables) {
                renderable.Render(frameTime, viewBoundingArea);
            }
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.PrefabReference;
    }

    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.ResetBoundingArea();
    }

    private void DeinitializeChild() {
        if (this._prefabChild != null) {
            this._prefabChild.Deinitialize();
            this.RemoveChild(this._prefabChild);
            this._prefabChild = null;
        }
    }

    private void PrefabReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.PrefabReference.ContentId)) {
            this.Reset();
        }
    }

    private void Reset() {
        this.DeinitializeChild();

        if (this.PrefabReference.Asset?.Content is { } prefab && prefab.TryClone(out var clone)) {
            if (BaseGame.IsDesignMode) {
                this._prefabChild = clone;
                this._prefabChild.Initialize(this.Scene, this);
                this.ResetBoundingArea();
            }
            else {
                this._prefabChild = clone;
                this.AddChild(this._prefabChild);
            }
        }
    }

    private void ResetBoundingArea() {
        if (BaseGame.IsDesignMode) {
            var boundingArea = BoundingArea.Empty;

            if (this._prefabChild != null) {
                var renderables = this._prefabChild.GetDescendants<IRenderableEntity>();
                foreach (var renderable in renderables) {
                    boundingArea = boundingArea.Combine(renderable.BoundingArea);
                }
            }

            this.BoundingArea = boundingArea;
            this.BoundingAreaChanged.SafeInvoke(this);
        }
    }
}