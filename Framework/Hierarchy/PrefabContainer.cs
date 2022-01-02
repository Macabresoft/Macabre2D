namespace Macabresoft.Macabre2D.Framework;

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
        this.PrefabReference.PropertyChanged += this.PrefabReference_PropertyChanged;
    }

    /// <summary>
    /// Gets a reference to the prefab this entity contains.
    /// </summary>
    [DataMember(Order = 0, Name = "Prefab")]
    public AssetReference<PrefabAsset> PrefabReference { get; } = new();

    /// <inheritdoc />
    public override void Initialize(IScene scene, IEntity parent) {
        base.Initialize(scene, parent);

        this.PrefabReference.PropertyChanged -= this.PrefabReference_PropertyChanged;
        this.Reset();
        this.PrefabReference.PropertyChanged += this.PrefabReference_PropertyChanged;
    }

    private void PrefabReference_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(this.PrefabReference.ContentId)) {
            this.Scene.Assets.ResolveAsset<PrefabAsset, IEntity>(this.PrefabReference);
        }
    }

    private void Reset() {
        this.Scene.Assets.ResolveAsset<PrefabAsset, IEntity>(this.PrefabReference);

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