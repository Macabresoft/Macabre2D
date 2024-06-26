namespace Macabresoft.Macabre2D.Framework;

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// An entity which loads a <see cref="PrefabAsset" />.
/// </summary>
public sealed class PrefabContainer : Entity {
    private IEntity? _prefabChild;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrefabContainer" /> class.
    /// </summary>
    public PrefabContainer() : base() {
    }

    /// <summary>
    /// Gets a reference to the prefab this entity contains.
    /// </summary>
    [DataMember(Order = 0, Name = "Prefab")]
    public PrefabReference PrefabReference { get; } = new();

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.PrefabReference.PropertyChanged -= this.PrefabReference_PropertyChanged;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);
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
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.PrefabReference;
    }

    private void PrefabReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.PrefabReference.ContentId)) {
            this.Reset();
        }
    }

    private void Reset() {
        if (this._prefabChild != null) {
            this.RemoveChild(this._prefabChild);
        }

        if (this.PrefabReference.Asset?.Content is { } prefab) {
            if (BaseGame.IsDesignMode) {
                this._prefabChild = prefab;
                this._prefabChild.Initialize(this.Scene, this);
            }
            else if (prefab.TryClone(out var entity)) {
                this._prefabChild = entity;
                this.AddChild(this._prefabChild);
            }
        }
    }
}