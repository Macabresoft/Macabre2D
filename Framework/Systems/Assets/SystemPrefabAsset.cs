namespace Macabre2D.Framework;

/// <summary>
/// An asset which contains a <see cref="SceneSystem" />.
/// </summary>
public sealed class SystemPrefabAsset  : Asset<SystemPrefab> {
    /// <summary>
    /// The file extension for a serialized <see cref="SceneSystem" />.
    /// </summary>
    public const string FileExtension = ".m2dsystem";

    /// <inheritdoc />
    public override bool IncludeFileExtensionInContentPath => true;
}