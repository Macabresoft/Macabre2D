namespace Macabre2D.Framework;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Reference to a <see cref="SystemPrefabAsset" />.
/// </summary>
public class SystemPrefabAssetReference : AssetReference<SystemPrefabAsset, SystemPrefab> {


    /// <summary>
    /// Tries to get the system.
    /// </summary>
    /// <param name="system">The system.</param>
    /// <returns>A value indicating whether the system was found.</returns>
    public bool TryGetSystem([NotNullWhen(true)] out ISceneSystem? system) {
        system = this.Asset?.Content?.System;
        return system != null;
    }
}