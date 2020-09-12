using System.Runtime.Serialization;

namespace Macabresoft.MonoGame.Core {

    /// <summary>
    /// A system which does a sorted update loop over enabled updateable components.
    /// </summary>
    public class UpdateSystem : GameSystem, IDynamicGameUpdateable {
        private bool _isEnabled = true;
        private int _updateOrder = 0;

        /// <inheritdoc />
        [DataMember]
        public bool IsEnabled {
            get {
                return this._isEnabled;
            }

            set {
                this.Set(ref this._isEnabled, value);
            }
        }

        /// <inheritdoc />
        [DataMember]
        public int UpdateOrder {
            get {
                return this._updateOrder;
            }

            set {
                this.Set(ref this._updateOrder, value);
            }
        }

        /// <inheritdoc />
        public virtual void Update(FrameTime frameTime, InputState inputState) {
            foreach (var component in this.Scene.UpdateableComponents) {
                component.Update(frameTime, inputState);
            }
        }
    }
}