namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// An instance of a shader that is used by the project as a whole and not specific scenes.
/// </summary>
public class ProjectShader : IIdentifiable, INameable {
    /// <summary>
    /// Gets the reference to the shader.
    /// </summary>
    [DataMember]
    public ShaderReference Shader { get; } = new();

    /// <inheritdoc />
    [DataMember]
    [Browsable(false)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <inheritdoc />
    [DataMember]
    public string Name { get; set; } = "Shader";
}