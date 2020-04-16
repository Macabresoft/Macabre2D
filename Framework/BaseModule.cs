namespace Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the most basic functionality of a module.
    /// </summary>
    [DataContract]
    public abstract class BaseModule : BaseIdentifiable {

        /// <summary>
        /// Gets a value indicating whether this instance is initialized.
        /// </summary>
        /// <value><c>true</c> if this instance is initialized; otherwise, <c>false</c>.</value>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        protected IScene Scene { get; private set; } = EmptyScene.Instance;

        /// <summary>
        /// Initializes the specified scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        public void Initialize(IScene scene) {
            this.Scene = scene;
            this.IsInitialized = true;
        }

        /// <summary>
        /// Initializes the module after a scene is initialized.
        /// </summary>
        public virtual void PostInitialize() {
            return;
        }

        /// <summary>
        /// Initializes the module before a scene is initialized. It is recommended that you use
        /// this to store a reference to the scene if required.
        /// </summary>
        public virtual void PreInitialize() {
            return;
        }
    }
}