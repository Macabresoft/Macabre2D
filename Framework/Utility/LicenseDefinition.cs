namespace Macabresoft.Macabre2D.Framework; 

/// <summary>
/// A license definition for displaying a product and its license.
/// </summary>
public sealed class LicenseDefinition : NotifyPropertyChanged {
    private bool _isExpanded = true;

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

    /// <summary>
    /// Gets a value indicating whether or not this should be expanded in a list of licenses.
    /// </summary>
    public bool IsExpanded {
        get => this._isExpanded;
        set => this.Set(ref this._isExpanded, value);
    }
}