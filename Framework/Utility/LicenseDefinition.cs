namespace Macabresoft.Macabre2D.Framework {
    
    /// <summary>
    /// A license definition for displaying a product and its license.
    /// </summary>
    public sealed class LicenseDefinition {
        
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
        /// Gets the product.
        /// </summary>
        public string Product { get; }
        
        /// <summary>
        /// Gets the license.
        /// </summary>
        public string License { get; }
    }
}