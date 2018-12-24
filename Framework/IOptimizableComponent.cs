using System.Collections.Generic;

namespace Macabre2D.Framework {

    public interface IOptimizableComponent {

        /// <summary>
        /// Optimizes this instance. Used for complicated components that can be optimized in the
        /// compiled version of the scene.
        /// </summary>
        /// <returns>A list of components that will replace this current component in the scene.</returns>
        IEnumerable<BaseComponent> Optimize();
    }
}