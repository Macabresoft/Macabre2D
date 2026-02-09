namespace Macabre2D.Framework;

using System;
using System.Runtime.Serialization;
using Macabre2D.Project.Common;

/// <summary>
/// A key used internally by <see cref="ProjectFonts" /> to access <see cref="ProjectFontDefinition" />.
/// </summary>
[DataContract]
public readonly struct ProjectFontKey : IEquatable<ProjectFontKey> {

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

    /// <inheritdoc />
    public bool Equals(ProjectFontKey other) => this.Culture == other.Culture && this.Category == other.Category;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is ProjectFontKey other && this.Equals(other);

    /// <summary>
    /// Equality operator.
    /// </summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    /// <returns>A value indicating whether the two values are equal.</returns>
    public static bool operator ==(ProjectFontKey left, ProjectFontKey right) => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    /// <returns>A value indicating whether the two values are not equal.</returns>
    public static bool operator !=(ProjectFontKey left, ProjectFontKey right) => !(left == right);
}