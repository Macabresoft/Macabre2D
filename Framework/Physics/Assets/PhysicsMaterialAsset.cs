namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// An asset which contains a <see cref="PhysicsMaterial" />.
/// </summary>
public class PhysicsMaterialAsset : Asset<PhysicsMaterial> {

    /// <summary>
    /// The file extension for a serialized <see cref="PhysicsMaterial" />.
    /// </summary>
    public const string FileExtension = ".m2dpmat";
    
    /// <inheritdoc />
    public override bool IncludeFileExtensionInContentPath => true;
}