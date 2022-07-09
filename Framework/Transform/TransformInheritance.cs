namespace Macabresoft.Macabre2D.Framework;

/// <summary>
/// Represents the kind of inheritance a <see cref="ITransformable" /> has for its parent transform.
/// </summary>
/// <remarks>
/// This applies to both scale and position. If X, Y, or Both is chosen, the <see cref="ITransformable" />
/// will apply it's parent's transform to itself before evaluating local transformations.
/// </remarks>
public enum TransformInheritance {
    None,
    X,
    Y,
    Both
}