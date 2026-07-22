namespace Macabre2D.Framework;

using System.Collections.Generic;
using System.Runtime.Serialization;

/// <summary>
/// A container for a scene system asset.
/// </summary>
public class SystemPrefabContainer : SceneSystem {
    /// <summary>
    /// Gets the system.
    /// </summary>
    public ISceneSystem? System { get; private set; }

    /// <summary>
    /// Gets the system asset reference.
    /// </summary>
    [DataMember]
    public SystemPrefabAssetReference SystemPrefabReference { get; } = new();

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.System = null;
    }

    /// <inheritdoc />
    public override void Initialize(IScene scene) {
        base.Initialize(scene);
        if (this.SystemPrefabReference.TryGetSystem(out var system)) {
            this.System = system;

            if (!BaseGame.IsDesignMode) {
                this.Scene.InsertSystemAfter(system, this);
                this.Scene.RemoveSystem(this);
            }
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<IAssetReference> GetAssetReferences() {
        yield return this.SystemPrefabReference;
    }
}