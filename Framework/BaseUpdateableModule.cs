namespace Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A varient of <see cref="BaseModule"/> that implements some of the basics of <see cref="IUpdateableModule"/>.
    /// </summary>
    public abstract class BaseUpdateableModule : BaseModule, IUpdateableModule {

        [DataMember]
        private bool _isEnabled = true;

        [DataMember]
        private int _updateOrder;

        /// <inheritdoc/>
        public event EventHandler IsEnabledChanged;

        /// <inheritdoc/>
        public event EventHandler UpdateOrderChanged;

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public abstract void PostUpdate(FrameTime frameTime);

        /// <inheritdoc/>
        public abstract void PreUpdate(FrameTime frameTime);
    }
}