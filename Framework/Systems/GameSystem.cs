namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for an system which runs operations for a <see cref="IScene" />.
/// </summary>
public interface IGameSystem : IUpdateableGameObject, INameable, IIdentifiable {
    /// <summary>
    /// Gets the system.
    /// </summary>
    /// <value>The system.</value>
    GameSystemKind Kind { get; }

    /// <summary>
    /// Deinitializes this service.
    /// </summary>
    void Deinitialize();

    /// <summary>
    /// Initializes this service as a descendent of <paramref name="scene" />.
    /// </summary>
    /// <param name="scene">The scene.</param>
    void Initialize(IScene scene);

    /// <summary>
    /// Called when the scene tree is loaded and ready for interactions.
    /// </summary>
    void OnSceneTreeLoaded();
}

/// <summary>
/// Base class for a system which runs operations for a <see cref="IScene" />.
/// </summary>
[DataContract]
[Category("System")]
public abstract class GameSystem : PropertyChangedNotifier, IGameSystem {
    private bool _shouldUpdate = true;

    /// <inheritdoc />
    public event EventHandler? ShouldUpdateChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameSystem" /> class.
    /// </summary>
    protected GameSystem() {
        this.Name = this.GetType().Name;
    }

    /// <inheritdoc />
    [DataMember]
    [Browsable(false)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    public abstract GameSystemKind Kind { get; }

    /// <inheritdoc />
    [DataMember]
    public string Name { get; set; }

    /// <inheritdoc />
    [DataMember]
    public bool ShouldUpdate {
        get => this._shouldUpdate;
        set {
            if (this.Set(ref this._shouldUpdate, value)) {
                this.ShouldUpdateChanged.SafeInvoke(this);
            }
        }
    }

    /// <summary>
    /// Gets the game.
    /// </summary>
    protected IGame Game => this.Scene.Game;

    /// <summary>
    /// Gets the scene.
    /// </summary>
    /// <value>The scene.</value>
    protected IScene Scene { get; private set; } = EmptyObject.Scene;

    /// <inheritdoc />
    public virtual void Deinitialize() {
    }

    /// <inheritdoc />
    public virtual void Initialize(IScene scene) {
        this.Scene = scene;
    }

    /// <inheritdoc />
    public virtual void OnSceneTreeLoaded() {
    }

    /// <inheritdoc />
    public abstract void Update(FrameTime frameTime, InputState inputState);
}