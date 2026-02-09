namespace Macabre2D.Framework;

using Macabresoft.Core;

/// <summary>
/// A license definition for displaying a product and its license.
/// </summary>
public sealed class LicenseDefinition : PropertyChangedNotifier {
    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingArea" /> class.
    /// </summary>
    /// <param name="product">The product.</param>
    /// <param name="license">The license.</param>
    public LicenseDefinition(string product, string license) {
        this.Product = product;
        this.License = license;
    }

    /// <summary>
    /// Gets the license.
    /// </summary>
    public string License { get; }

    /// <summary>
    /// Gets the product.
    /// </summary>
    public string Product { get; }
}