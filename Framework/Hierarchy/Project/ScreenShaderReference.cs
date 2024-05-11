namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;
using System.Runtime.Serialization;

/// <summary>
/// A reference to a <see cref="ScreenShader"/>.
/// </summary>
[DataContract]
public class ScreenShaderReference {
    private ScreenShader? _shader;
    
    /// <summary>
    /// Gets or sets the screen shader identifier.
    /// </summary>
    [DataMember]
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the shader.
    /// </summary>
    public ScreenShader? Shader {
        get => this._shader;
        set {
            this._shader = value;
            this.Id = this._shader?.Id ?? Guid.Empty;
        }
    }
    
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="project">The project.</param>
    public void Initialize(IGameProject project) {
        this.Shader = project.ScreenShaders.OfType<ScreenShader>().FirstOrDefault(x => x.Id == this.Id);
    }
}