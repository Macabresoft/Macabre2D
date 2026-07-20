namespace Macabre2D.Framework;

using System.Runtime.Serialization;

/// <summary>
/// A scene system prefab, which wraps a <see cref="ISceneSystem" /> to be saved as a content file and loaded as part of an asset.
/// </summary>
[DataContract]
public class SceneSystemPrefab {
    /// <summary>
    /// Initializes a new instance of the <see cref="SceneSystemPrefab" /> class.
    /// </summary>
    /// <param name="system">The system.</param>
    public SceneSystemPrefab(ISceneSystem system) {
        this.System = system;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SceneSystemPrefab" /> class.
    /// </summary>
    public SceneSystemPrefab() {
    }

    /// <summary>
    /// Gets the system.
    /// </summary>
    [DataMember]
    public ISceneSystem? System { get; internal set; }
}