namespace Macabresoft.Macabre2D.Framework {

    using System.Runtime.Serialization;

    /// <summary>
    /// A <see cref="IGameEntity" /> which implements <see cref="IGameUpdateable" />, <see
    /// cref="IEnableable" />, and can be sorted.
    /// </summary>
    public interface IGameUpdateableEntity : IGameEntity, IGameUpdateable {

        /// <summary>
        /// Gets the update order.
        /// </summary>
        /// <value>The update order.</value>
        int UpdateOrder { get => 0; }
    }
    
    /// <summary>
    /// A base implementation of <see cref="IGameUpdateableEntity" />.
    /// </summary>
    public abstract class GameUpdateableEntity : GameEntity, IGameUpdateableEntity {
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