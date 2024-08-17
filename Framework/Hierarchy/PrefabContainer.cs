namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Microsoft.Xna.Framework;

/// <summary>
/// An entity which loads a <see cref="PrefabAsset" />.
/// </summary>
public sealed class PrefabContainer : Entity, IRenderableEntity {
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
        get => BaseGame.IsDesignMode && this.IsEnabled;
        set { }
    }

    /// <inheritdoc />
    public bool RenderOutOfBounds { get; set; }

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.PrefabReference.AssetChanged -= this.PrefabReference_AssetChanged;
        this.DeinitializeChild();
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
        this.Reset();
        this.PrefabReference.AssetChanged += this.PrefabReference_AssetChanged;
    }

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

    /// <inheritdoc />
    protected override void OnTransformChanged() {
        base.OnTransformChanged();
        this.ResetBoundingArea();
    }

    private void DeinitializeChild() {
        if (this._prefabChild != null) {
            if (BaseGame.IsDesignMode) {
                this.ForceChildRemoval(this._prefabChild);
            }
            else {
                this.RemoveChild(this._prefabChild);
            }

            this._prefabChild = null;
        }
    }

    private void PrefabReference_AssetChanged(object? sender, bool hasAsset) {
        this.Reset();
    }

    private void Reset() {
        this.DeinitializeChild();

        if (this.PrefabReference.Asset?.Content is { } prefab && prefab.TryClone(out var clone)) {
            this._prefabChild = clone;
            if (BaseGame.IsDesignMode) {
                this._prefabChild.Initialize(this.Scene, this);
                this.ResetBoundingArea();
                this.Scene.UnregisterEntity(this._prefabChild);
                var cloneChildren = this._prefabChild.GetDescendants<Entity>();
                foreach (var child in cloneChildren) {
                    this.Scene.UnregisterEntity(child);
                }

                this.RaisePropertyChanged(nameof(this.IsVisible));
            }
            else {
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