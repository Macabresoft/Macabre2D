namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// An asset for prefabricated <see cref="IEntity"/>.
/// </summary>
public sealed class PrefabAsset : Asset<IEntity> {
    
    /// <summary>
    /// The file extension for a serialized <see cref="IEntity" /> as a prefab.
    /// </summary>
    public const string FileExtension = ".m2dprefab";
    
    /// <inheritdoc />
    public override bool IncludeFileExtensionInContentPath => true;
}