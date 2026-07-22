namespace Macabre2D.Framework;

/// <summary>
/// An asset for prefabricated <see cref="IEntity" />.
/// </summary>
public sealed class EntityPrefabAsset : Asset<EntityPrefab> {
    /// <summary>
    /// The file extension for a serialized <see cref="IEntity" /> as a prefab.
    /// </summary>
    public const string FileExtension = ".m2dentity";

    /// <inheritdoc />
    public override bool IncludeFileExtensionInContentPath => true;
}