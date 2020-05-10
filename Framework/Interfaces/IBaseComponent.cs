namespace Macabre2D.Framework {

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Interface for a base component. Contains shared properties between all components.
    /// </summary>
    public interface IBaseComponent : INotifyPropertyChanged, IIdentifiableComponent, IWorldTransformable {

        /// <summary>
        /// Occurs when initialized.
        /// </summary>
        event EventHandler OnInitialized;

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
    }
}