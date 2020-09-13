namespace Macabresoft.MonoGame.Core {

    using System.Runtime.Serialization;

    /// <summary>
    /// A base implementation of <see cref="IGameUpdateableComponent" />.
    /// </summary>
    public abstract class GameUpdateableComponent : GameComponent, IGameUpdateableComponent {
        private int _updateOrder = 0;

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
        public abstract void Update(FrameTime frameTime, InputState inputState);
    }
}