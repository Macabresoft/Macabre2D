namespace Macabre2D.Framework;

using System;

/// <summary>
/// An entity command that can be recognized and called from the editor. Must be placed on a
/// parameterless method.
/// </summary>
/// <seealso cref="System.Attribute" />
[AttributeUsage(AttributeTargets.Method)]
public sealed class EntityCommandAttribute : Attribute {
    /// <summary>
    /// Initializes a new instance of the <see cref="EntityCommandAttribute" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public EntityCommandAttribute(string name) {
        this.Name = name;
    }

    /// <summary>
    /// Gets the name to appear in the editor.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; }
}