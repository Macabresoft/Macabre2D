namespace Macabresoft.MonoGame.Core {

    using System.Runtime.Serialization;

    /// <summary>
    /// A <see cref="IGameComponent" /> that also implements <see cref="IGameUpdateable" /> and <see
    /// cref="IEnableable" />.
    /// </summary>
    public interface IGameUpdateableComponent : IGameUpdateable, IGameComponent {

        /// <summary>
        /// Gets the update order.
        /// </summary>
        /// <value>The update order.</value>
        int UpdateOrder { get; }
    }

    /// <summary>
    /// A base implementation of <see cref="IGameUpdateableComponent" />.
    /// </summary>
    public abstract class GameUpdateableComponent : GameComponent, IGameUpdateableComponent {

        [DataMember]
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