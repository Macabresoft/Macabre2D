namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Macabresoft.Macabre2D.Project.Common;

/// <summary>
/// A key used internally by <see cref="ProjectFonts" /> to access <see cref="ProjectFontDefinition" />.
/// </summary>
[DataContract]
internal struct ProjectFontKey {

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFontKey" /> class.
    /// </summary>
    public ProjectFontKey() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectFontKey" /> class.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <param name="culture">The culture.</param>
    public ProjectFontKey(FontCategory category, ResourceCulture culture) {
        this.Culture = culture;
        this.Category = category;
    }

    /// <summary>
    /// The culture.
    /// </summary>
    [DataMember]
    public readonly ResourceCulture Culture;

    /// <summary>
    /// The category.
    /// </summary>
    [DataMember]
    public readonly FontCategory Category;

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(this.Category, this.Culture);
}