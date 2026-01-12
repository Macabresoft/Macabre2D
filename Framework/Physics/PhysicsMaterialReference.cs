namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;
using Macabresoft.Macabre2D.Common;

/// <summary>
/// A reference to a <see cref="PhysicsMaterial" />.
/// </summary>
[DataContract]
public sealed class PhysicsMaterialReference : PropertyChangedNotifier, IGameObjectReference, IIdentifiable {
    private IScene _scene = EmptyObject.Scene;

    /// <inheritdoc />
    [DataMember]
    [PhysicsMaterialGuid]
    public Guid Id {
        get;
        set {
            if (field != value) {
                field = value;
                this.ResetReference();
                this.RaisePropertyChanged();
            }
        }
    } = Guid.Empty;

    /// <summary>
    /// Gets the actual <see cref="PhysicsMaterial" /> object.
    /// </summary>
    public PhysicsMaterial Material { get; private set; } = PhysicsMaterial.Default;

    /// <summary>
    /// Clears the reference.
    /// </summary>
    public void Clear() {
        this.Id = Guid.Empty;
        this.Material = PhysicsMaterial.Default;
    }

    /// <inheritdoc />
    public void Deinitialize() {
        this._scene = EmptyObject.Scene;
    }

    /// <inheritdoc />
    public void Initialize(IScene scene) {
        this._scene = scene;
        this.ResetReference();
    }

    private void ResetReference() {
        if (this.Id == Guid.Empty) {
            this.Material = PhysicsMaterial.Default;
        }
        else {
            this.Material = this._scene.Project.PhysicsMaterials.FirstOrDefault<PhysicsMaterial>(x => x.Id == this.Id) ?? PhysicsMaterial.Default;
        }
    }
}