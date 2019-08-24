namespace Macabre2D.Framework {

    using System.Collections.Generic;

    /// <summary>
    /// Interface for a base component. Contains shared properties between all components.
    /// </summary>
    public interface IBaseComponent : IIdentifiableComponent, IWorldTransformable {

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        IReadOnlyCollection<BaseComponent> Children { get; }

        /// <summary>
        /// Gets the layers.
        /// </summary>
        /// <value>The layers.</value>
        Layers Layers { get; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        BaseComponent Parent { get; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>The clone.</returns>
        BaseComponent Clone();
    }
}