namespace Macabre2D.Framework;

using System;
using Macabresoft.Core;

/// <summary>
/// Base class for <see cref="ISceneSystem"/> references.
/// </summary>
public abstract class SceneSystemReference : PropertyChangedNotifier, IGameObjectReference {

    /// <summary>
    /// Gets or sets the system identifier.
    /// </summary>
    public Guid SystemId {
        get;
        set {
            if (field != value) {
                field = value;
                this.ResetSystem();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the type of the system referenced.
    /// </summary>
    public abstract Type Type { get; }

    /// <summary>
    /// Gets an untyped version of the system.
    /// </summary>
    public ISceneSystem? UntypedSystem { get; protected set; }

    /// <summary>
    /// Gets the scene.
    /// </summary>
    protected IScene Scene { get; private set; } = EmptyObject.Scene;

    /// <inheritdoc />
    public virtual void Deinitialize() {
        this.Scene = EmptyObject.Scene;
        this.UntypedSystem = null;
    }

    /// <inehrtidoc />
    public void Initialize(IGame game, IScene scene) {
        this.Scene = scene;
        this.ResetSystem();
    }

    /// <summary>
    /// Resets the system.
    /// </summary>
    protected abstract void ResetSystem();
}

/// <summary>
/// A reference to a system using an identifier and type for serialization purposes.
/// </summary>
/// <typeparam name="TSystem">The type of system.</typeparam>
public class SceneSystemReference<TSystem> : SceneSystemReference where TSystem : class, ISceneSystem {

    /// <summary>
    /// Gets the system.
    /// </summary>
    public TSystem? System {
        get;
        private set => this.Set(ref field, value);
    }

    /// <inheritdoc />
    public override Type Type => typeof(TSystem);

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this.System = null;
    }

    /// <inheritdoc />
    protected override void ResetSystem() {
        if (this.SystemId == Guid.Empty || Framework.Scene.IsNullOrEmpty(this.Scene)) {
            this.System = null;
        }
        else {
            this.System = this.Scene.FindSystem<TSystem>(this.SystemId);
        }

        this.UntypedSystem = this.System;
    }
}