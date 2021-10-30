﻿namespace Macabresoft.Macabre2D.Framework {
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a collection of <see cref="INameable"/>.
    /// </summary>
    public interface INameableCollection : IReadOnlyCollection<INameable> {
        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        string Name { get; }
    }
}