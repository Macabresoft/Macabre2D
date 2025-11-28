namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// A reference to a <see cref="PhysicsMaterial"/>.
/// </summary>
public sealed class PhysicsMaterialReference : PropertyChangedNotifier, IGameObjectReference, IIdentifiable {
    private Guid _id = Guid.Empty;
    private IScene _scene = EmptyObject.Scene;

    /// <inheritdoc />
    [DataMember]
    public Guid Id {
        get => this._id;
        set {
            if (this._id != value) {
                this._id = value;
                this.ResetReference();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the actual <see cref="PhysicsMaterial" /> object.
    /// </summary>
    public PhysicsMaterial Material { get; private set; } = PhysicsMaterial.Default;

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
        this.Material = this._scene.Project.PhysicsMaterials.FirstOrDefault<PhysicsMaterial>(x => x.Id == this.Id) ?? PhysicsMaterial.Default;
    }
}