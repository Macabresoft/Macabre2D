namespace Macabre2D.Framework;

using System;
using System.Linq;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for a render step reference.
/// </summary>
public interface IRenderStepReference : IGameObjectReference {
    /// <summary>
    /// Gets or sets the render step identifier.
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// Gets the type of the render step.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets an untyped version of the render step.
    /// </summary>
    RenderStep? UntypedStep { get; }
}

/// <summary>
/// Base class for referencing render steps.
/// </summary>
[DataContract]
public abstract class RenderStepReference : PropertyChangedNotifier, IRenderStepReference {

    /// <inheritdoc />
    [DataMember]
    public Guid Id {
        get;
        set {
            if (field != value) {
                field = value;
                this.ResetRenderStep();
                this.RaisePropertyChanged();
            }
        }
    }

    /// <inheritdoc />
    public abstract Type Type { get; }

    /// <inheritdoc />
    public abstract RenderStep? UntypedStep { get; }

    /// <summary>
    /// Gets the scene.
    /// </summary>
    protected IScene Scene { get; private set; } = EmptyObject.Scene;

    /// <inheritdoc />
    public virtual void Deinitialize() {
        this.Scene = EmptyObject.Scene;
    }

    /// <inheritdoc />
    public virtual void Initialize(IScene scene) {
        this.Scene = scene;
        this.ResetRenderStep();
    }

    /// <summary>
    /// Sets the render step.
    /// </summary>
    protected abstract void ResetRenderStep();
}

/// <summary>
/// A reference to a <see cref="ScreenShaderRenderStep" />.
/// </summary>
public class RenderStepReference<TStep> : RenderStepReference where TStep : RenderStep {
    private TStep? _step;

    /// <summary>
    /// Gets the render step.
    /// </summary>
    public TStep? Step {
        get => this._step;
        private set => this.Set(ref this._step, value);
    }

    /// <inheritdoc />
    public override Type Type => typeof(TStep);

    /// <inheritdoc />
    public override RenderStep? UntypedStep => this.Step;

    /// <inheritdoc />
    public override void Deinitialize() {
        base.Deinitialize();
        this._step = null;
    }

    /// <inheritdoc />
    protected override void ResetRenderStep() {
        this.Step = this.Scene.Project.RenderSteps.OfType<TStep>().FirstOrDefault(x => x.Id == this.Id);
    }
}