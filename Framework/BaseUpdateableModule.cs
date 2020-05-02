namespace Macabre2D.Framework {

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
        public bool IsEnabled {
            get {
                return this._isEnabled;
            }
            set {
                this.Set(ref this._isEnabled, value);
            }
        }

        /// <inheritdoc/>
        public int UpdateOrder {
            get {
                return this._updateOrder;
            }

            set {
                this.Set(ref this._updateOrder, value);
            }
        }

        /// <inheritdoc/>
        public abstract void PostUpdate(FrameTime frameTime);

        /// <inheritdoc/>
        public abstract void PreUpdate(FrameTime frameTime);
    }
}