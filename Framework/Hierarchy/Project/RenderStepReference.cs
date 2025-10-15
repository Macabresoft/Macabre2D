namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;
using System.Runtime.Serialization;

/// <summary>
/// A reference to a <see cref="ScreenShader"/>.
/// </summary>
[DataContract]
public class RenderStepReference {
    private RenderStep? _step;
    
    /// <summary>
    /// Gets or sets the screen shader identifier.
    /// </summary>
    [DataMember]
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the render step.
    /// </summary>
    public RenderStep? Step {
        get => this._step;
        set {
            this._step = value;
            this.Id = this._step?.Id ?? Guid.Empty;
        }
    }
    
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <param name="project">The project.</param>
    public void Initialize(IGameProject project) {
        this.Step = project.RenderSteps.OfType<RenderStep>().FirstOrDefault(x => x.Id == this.Id);
    }
}