namespace Macabresoft.MonoGame.Core {

    using System.Runtime.Serialization;

    /// <summary>
    /// A varient of <see cref="BaseModule"/> that implements some of the basics of <see cref="IUpdateableModule"/>.
    /// </summary>
    public abstract class BaseUpdateableModule : BaseModule, IUpdateableModule {
        private bool _isEnabled = true;
        private int _updateOrder;

        /// <inheritdoc/>
        [DataMember(Name = "Enabled")]
        public bool IsEnabled {
            get {
                return this._isEnabled;
            }
            set {
                this.Set(ref this._isEnabled, value);
            }
        }

        /// <inheritdoc/>
        [DataMember(Name = "Update Order")]
        public int UpdateOrder {
            get {
                return this._updateOrder;
            }

            set {
                this.Set(ref this._updateOrder, value);
            }
        }

        /// <inheritdoc/>
        public abstract void PostUpdate(FrameTime frameTime, InputState inputState);

        /// <inheritdoc/>
        public abstract void PreUpdate(FrameTime frameTime, InputState inputState);
    }
}