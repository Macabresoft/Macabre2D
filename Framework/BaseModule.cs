namespace Macabre2D.Framework {

    using Macabre2D.Framework.Extensions;
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the most basic functionality of a module.
    /// </summary>
    /// <seealso cref="IModule"/>
    [DataContract]
    public class BaseModule : IIdentifiable {

        [DataMember]
        private bool _isEnabled = true;

        [DataMember]
        private int _updateOrder;

        /// <summary>
        /// Occurs when this instance becomes enabled or disabled.
        /// </summary>
        public event EventHandler IsEnabledChanged;

        /// <summary>
        /// Occurs when [update order changed].
        /// </summary>
        public event EventHandler UpdateOrderChanged;

        /// <inheritdoc/>
        [DataMember]
        public Guid Id { get; internal set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        public bool IsEnabled {
            get {
                return this._isEnabled;
            }
            set {
                if (this._isEnabled != value) {
                    this._isEnabled = value;
                    this.IsEnabledChanged.SafeInvoke(this);
                }
            }
        }

        /// <summary>
        /// Gets the scene.
        /// </summary>
        /// <value>The scene.</value>
        public IScene Scene { get; private set; }

        /// <summary>
        /// Gets or sets the update order.
        /// </summary>
        /// <value>The update order.</value>
        public int UpdateOrder {
            get {
                return this._updateOrder;
            }

            set {
                if (value != this._updateOrder) {
                    this._updateOrder = value;
                    this.UpdateOrderChanged.SafeInvoke(this);
                }
            }
        }

        /// <summary>
        /// Initializes the specified scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        public void Initialize(IScene scene) {
            this.Scene = scene;
        }

        /// <summary>
        /// Initializes the module after a scene is initialized.
        /// </summary>
        public virtual void PostInitialize() {
            return;
        }

        /// <summary>
        /// Updates after the normal update occurs for a scene.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public virtual void PostUpdate(GameTime gameTime) {
            return;
        }

        /// <summary>
        /// Initializes the module before a scene is initialized. It is recommended that you use this
        /// to store a reference to the scene if required.
        /// </summary>
        public virtual void PreInitialize() {
            return;
        }

        /// <summary>
        /// Updates before the normal update for a scene.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        public virtual void PreUpdate(GameTime gameTime) {
            return;
        }
    }
}