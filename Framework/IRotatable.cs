namespace Macabresoft.Macabre2D.Framework {

    /// <summary>
    /// Represents something that can be rotated using <see cref="Rotation"/>.
    /// </summary>
    public interface IRotatable {

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        float Rotation { get; set; }
    }
}