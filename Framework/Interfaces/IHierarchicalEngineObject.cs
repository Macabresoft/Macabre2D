namespace Macabre2D.Framework {

    /// <summary>
    /// A hierarchic
    /// </summary>
    public enum EngineObjectType {

        /// <summary>
        /// The game type.
        /// </summary>
        Game = 0,

        /// <summary>
        /// The scene type.
        /// </summary>
        Scene = 1,

        /// <summary>
        /// The component type.
        /// </summary>
        Component = 2,

        /// <summary>
        /// The module type.
        /// </summary>
        Module = 4
    }

    /// <summary>
    /// Interface for an object in the engine hierarchy. This is the most basic interface with no
    /// child or parent.
    /// </summary>
    public interface IHierarchicalEngineObject {

        /// <summary>
        /// Gets the type of the engine object.
        /// </summary>
        /// <value>The type of the engine object.</value>
        EngineObjectType EngineObjectType { get; }
    }
}