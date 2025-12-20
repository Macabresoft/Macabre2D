namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Linq;
using System.Runtime.Serialization;

/// <summary>
/// A reference to a <see cref="ScreenShaderRenderStep" />.
/// </summary>
[DataContract]
public class RenderStepReference {

    /// <summary>
    /// Gets or sets the screen shader identifier.
    /// </summary>
    [DataMember]
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the render step.
    /// </summary>
    public RenderStep? Step {
        get;
        set {
            field = value;
            this.Id = field?.Id ?? Guid.Empty;
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