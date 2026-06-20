namespace Macabre2D.Framework;

using System;
using System.Linq;
using System.Runtime.Serialization;
using Macabre2D.Common;
using Macabresoft.Core;

/// <summary>
/// A reference to a <see cref="PhysicsMaterial" />.
/// </summary>
[DataContract]
public sealed class PhysicsMaterialReference : PropertyChangedNotifier, IGameObjectReference, IIdentifiable {
    private IGame _game = BaseGame.Empty;

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
        this._game = BaseGame.Empty;
    }

    /// <inheritdoc />
    public void Initialize(IGame game, IScene scene, IEntity entity) {
        this._game = game;
        this.ResetReference();
    }

    private void ResetReference() {
        if (this.Id == Guid.Empty) {
            this.Material = PhysicsMaterial.Default;
        }
        else {
            this.Material = this._game.Project.PhysicsMaterials.FirstOrDefault<PhysicsMaterial>(x => x.Id == this.Id) ?? PhysicsMaterial.Default;
        }
    }
}