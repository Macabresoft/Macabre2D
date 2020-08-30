namespace Macabresoft.MonoGame.Core {

    using Macabresoft.Core;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for a descendent of <see cref="IGameEntity" />.
    /// </summary>
    public interface IGameComponent : INotifyPropertyChanged {

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>The entity.</value>
        IGameEntity Entity { get; }

        /// <summary>
        /// Initializes this component as a descendent of <paramref name="scene" /> and <paramref
        /// name="entity" />.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Initialize(IGameEntity entity);
    }

    /// <summary>
    /// A descendent of <see cref="IGameEntity" />.
    /// </summary>
    [DataContract]
    public abstract class GameComponent : PropertyChangedNotifier, IGameComponent {
        private IGameEntity _entity;

        /// <inheritdoc />
        public IGameEntity Entity {
            get {
                return this._entity;
            }

            set {
                this.Set(ref this._entity, value);
            }
        }

        /// <inheritdoc />
        public virtual void Initialize(IGameEntity entity) {
            this.Entity = entity;
        }
    }
}