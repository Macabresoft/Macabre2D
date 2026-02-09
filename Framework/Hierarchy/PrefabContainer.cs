namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabre2D.Project.Common;
using Microsoft.Xna.Framework;

/// <summary>
/// Interface for an entity which contains another entity.
/// </summary>
public interface IPrefabContainer : IEntity {
    /// <summary>
    /// Gets a value indicating whether the other entity is a descendent of this container's prefab.
    /// </summary>
    /// <remarks>
    /// This can be very slow and it is recommended to avoid calling this during active gameplay.
    /// </remarks>
    /// <param name="otherEntity">The other entity.</param>
    /// <returns>A value indicating whether the other entity is a descendent of this container's prefab.</returns>
    bool IsPartOfPrefab(IEntity otherEntity);
}

/// <summary>
/// An entity which loads a <see cref="PrefabAsset" />.
/// </summary>
public sealed class PrefabContainer : Entity, IPrefabContainer, IRenderableEntity {
    private Entity? _prefabChild;

    /// <inheritdoc />
    public event EventHandler? BoundingAreaChanged;

    /// <inheritdoc />
    public event EventHandler? ShouldRenderChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrefabContainer" /> class.
    /// </summary>
    public PrefabContainer() : base() {
    }

    /// <inheritdoc />
    public BoundingArea BoundingArea { get; private set; } = BoundingArea.Empty;

    /// <summary>
    /// Gets a reference to the prefab this entity contains.
    /// </summary>
    [DataMember(Order = 0, Name = "Prefab")]
    public PrefabReference PrefabReference { get; } = new();

    /// <inheritdoc />
    public int RenderOrder { get; set; }

    /// <inheritdoc />
    public bool RenderOutOfBounds { get; set; }

    /// <inheritdoc />
    public RenderPriority RenderPriority { get; set; }

    /// <inheritdoc />
    public bool ShouldRender {
        get => BaseGame.IsDesignMode && this.IsEnabled;
        set { }
    }

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
    public bool IsPartOfPrefab(IEntity otherEntity) => this._prefabChild != null && (otherEntity.Id == this._prefabChild.Id || otherEntity.IsDescendentOf(this._prefabChild));

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
    protected override void OnIsEnableChanged() {
        base.OnIsEnableChanged();
        if (BaseGame.IsDesignMode) {
            this.ShouldRenderChanged.SafeInvoke(this);
        }
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
                this._prefabChild.LoadAssets(this.Scene.Assets, this.Game);
                this._prefabChild.Initialize(this.Scene, this);
                this.ResetBoundingArea();
                this.Scene.UnregisterEntity(this._prefabChild);
                var cloneChildren = this._prefabChild.GetDescendants<Entity>();
                foreach (var child in cloneChildren) {
                    this.Scene.UnregisterEntity(child);
                }

                this.RaisePropertyChanged(nameof(this.ShouldRender));
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