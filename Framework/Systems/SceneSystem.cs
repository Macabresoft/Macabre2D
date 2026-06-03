namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for a system which runs operations for a <see cref="IScene" />.
/// </summary>
public interface ISceneSystem : IBaseSystem {
    /// <summary>
    /// Initializes this system as a descendent of <paramref name="scene" />.
    /// </summary>
    /// <param name="scene">The scene.</param>
    void Initialize(IScene scene);
}

/// <summary>
/// Base class for a system which runs operations for a <see cref="IScene" />.
/// </summary>
[DataContract]
[Category("System")]
public abstract class SceneSystem : BaseSystem, ISceneSystem {
    /// <summary>
    /// Gets the game.
    /// </summary>
    protected IGame Game => this.Scene.Game;

    /// <summary>
    /// Gets the common measurements from <see cref="Game" />.
    /// </summary>
    protected ICommonMeasurements Measurements => this.Game.Measurements;

    /// <summary>
    /// Gets the scene.
    /// </summary>
    /// <value>The scene.</value>
    protected IScene Scene { get; private set; } = EmptyObject.Scene;

    public override void Deinitialize() {
        base.Deinitialize();
        
        foreach (var entityReference in this.GetGameObjectReferences()) {
            entityReference.Deinitialize();
        }
    }


    /// <inheritdoc />
    public virtual void Initialize(IScene scene) {
        try {
            if (this.IsInitialized) {
                this.Deinitialize();
            }

            this.Scene = scene;

            foreach (var entityReference in this.GetGameObjectReferences()) {
                entityReference.Initialize(this.Scene);
            }
        }
        finally {
            this.IsInitialized = true;
        }
    }
    
    /// <summary>
    /// Gets the game object references for initialization and deinitialization.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerable<IGameObjectReference> GetGameObjectReferences() {
        yield break;
    }
}