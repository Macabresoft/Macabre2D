namespace Macabre2D.Framework;

using System;
using System.Diagnostics.CodeAnalysis;
using Macabresoft.Core;

/// <summary>
/// Reference to a system using only its type.
/// </summary>
/// <typeparam name="TSystem">The type of system to reference.</typeparam>
public class SystemTypeReference<TSystem> : PropertyChangedNotifier, IGameObjectReference where TSystem : class, IGameSystem {
    /// <summary>
    /// Gets the referenced system.
    /// </summary>
    public TSystem? System { get; private set; }

    /// <summary>
    /// Gets the type of the system referenced.
    /// </summary>
    public Type Type { get; } = typeof(TSystem);

    /// <inheritdoc />
    public void Deinitialize() {
        this.System = null;
    }

    /// <inheritdoc />
    public void Initialize(IScene scene) {
        this.System = this.GetSystemFromScene(scene);
    }

    /// <summary>
    /// Tries to get the system.
    /// </summary>
    /// <param name="system">The system.</param>
    /// <returns>A value indicating whether the system was found.</returns>
    public bool TryGetValue([NotNullWhen(true)] out TSystem? system) {
        system = this.System;
        return system != null;
    }

    /// <summary>
    /// Gets the system from the scene.
    /// </summary>
    /// <param name="scene">The scene.</param>
    /// <returns>The system.</returns>
    protected virtual TSystem? GetSystemFromScene(IScene scene) => scene.GetSystem<TSystem>();
}