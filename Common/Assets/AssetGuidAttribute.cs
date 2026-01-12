namespace Macabresoft.Macabre2D.Common;

/// <summary>
/// Attribute for <see cref="Guid" /> asset references.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class AssetGuidAttribute : Attribute {
    /// <summary>
    /// Initializes a new instance of the <see cref="AssetGuidAttribute" /> class.
    /// </summary>
    /// <param name="assetType">The asset type.</param>
    public AssetGuidAttribute(Type assetType) {
        this.AssetType = assetType;
    }

    /// <summary>
    /// Gets the type of asset this <see cref="Guid" /> references.
    /// </summary>
    public Type AssetType { get; }
}