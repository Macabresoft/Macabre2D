namespace Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for a system which runs update operations.
/// </summary>
public interface IBaseSystem : INameable, IIdentifiable {

    /// <summary>
    /// Deinitializes this system.
    /// </summary>
    void Deinitialize();

    /// <summary>
    /// Loads assets for this system before initialization.
    /// </summary>
    /// <param name="assets">The assets.</param>
    /// <param name="game">The game.</param>
    void LoadAssets(IAssetManager assets, IGame game);

    /// <summary>
    /// Called when the scene tree is loaded and ready for interactions.
    /// </summary>
    void OnSceneTreeLoaded();
}

/// <summary>
/// Base class for a system which runs operations.
/// </summary>
[DataContract]
[Category("System")]
public abstract class BaseSystem : PropertyChangedNotifier, IBaseSystem {
    /// <summary>
    /// Initializes a new instance of the <see cref="SceneSystem" /> class.
    /// </summary>
    protected BaseSystem() {
        this.Name = this.GetType().Name;
    }

    /// <inheritdoc />
    [DataMember]
    [Browsable(false)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    [DataMember]
    public string Name {
        get;
        set {
            field = value;

            if (BaseGame.IsDesignMode && this.IsInitialized) {
                this.RaisePropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this is initialized.
    /// </summary>
    protected bool IsInitialized { get; set; }

    /// <inheritdoc />
    public virtual void Deinitialize() {
        foreach (var assetReference in this.GetAssetReferences()) {
            assetReference.Deinitialize();
        }

        this.IsInitialized = false;
    }

    /// <inheritdoc />
    public virtual void LoadAssets(IAssetManager assets, IGame game) {
        foreach (var assetReference in this.GetAssetReferences()) {
            assetReference.Initialize(assets, game);
        }
    }

    /// <inheritdoc />
    public virtual void OnSceneTreeLoaded() {
    }

    /// <summary>
    /// Gets the asset references for initialization and deinitialization.
    /// </summary>
    /// <returns>The asset references</returns>
    protected virtual IEnumerable<IAssetReference> GetAssetReferences() {
        yield break;
    }
}