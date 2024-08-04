namespace Macabresoft.Macabre2D.Framework;

using System;
using Macabresoft.Core;

/// <summary>
/// Base class for system references.
/// </summary>
public abstract class SystemReference : PropertyChangedNotifier {
    private Guid _systemId;

    /// <summary>
    /// Gets the type of the system referenced.
    /// </summary>
    public abstract Type Type { get; }

    /// <summary>
    /// Gets or sets the system identifier.
    /// </summary>
    public Guid SystemId {
        get => this._systemId;
        set {
            if (this._systemId != value) {
                this._systemId = value;
                this.ResetSystem();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets an untyped version of the system.
    /// </summary>
    public IGameSystem? UntypedSystem { get; protected set; }

    /// <summary>
    /// Gets the scene.
    /// </summary>
    protected IScene Scene { get; private set; } = Framework.Scene.Empty;

    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="scene">The scene.</param>
    public void Initialize(IScene scene) {
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
public class SystemReference<TSystem> : SystemReference where TSystem : class, IGameSystem {
    private TSystem? _system;

    /// <inheritdoc />
    public override Type Type => typeof(TSystem);

    /// <summary>
    /// Gets the system.
    /// </summary>
    public TSystem? System {
        get => this._system;
        private set => this.Set(ref this._system, value);
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