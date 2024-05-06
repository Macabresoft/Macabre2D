namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// An instance of a shader that is used by the project as a whole and not specific scenes.
/// </summary>
public class ProjectShader : PropertyChangedNotifier, IIdentifiable, INameable {
    private string _name = "Shader";

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
    public string Name {
        get => this._name;
        set => this.Set(ref this._name, value);
    }
}