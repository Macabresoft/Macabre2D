namespace Macabre2D.Framework;

/// <summary>
/// Represents the step of initialization a scene is currently performing.
/// </summary>
public enum SceneInitializationStep : byte {
    NotStarted = 0,
    InitializeProject = 1,
    LoadSystemAssets = 2,
    InitializeSystems = 3,
    InitializeState = 4,
    LoadEntityAssets = 5,
    InitializeEntities = 6,
    LoadSceneTree = 7,
    Done = 8
}