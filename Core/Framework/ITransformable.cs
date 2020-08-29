namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// Interface for an object which has a <see cref="Transform" />.
    /// </summary>
    public interface ITransformable {

        /// <summary>
        /// Gets the transform.
        /// </summary>
        /// <value>The transform.</value>
        Transform Transform { get; }
    }
}