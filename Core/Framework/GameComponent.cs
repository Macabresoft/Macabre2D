namespace Macabresoft.MonoGame.Core {

    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for a descendent of <see cref="IGameEntity" />.
    /// </summary>
    public interface IGameComponent : IEnableable, IIdentifiable, INotifyPropertyChanged {

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
    public abstract class GameComponent : BaseIdentifiable, IGameComponent {
        private IGameEntity _entity = GameEntity.Empty;

        [DataMember]
        private bool _isEnabled;

        /// <inheritdoc />
        public IGameEntity Entity {
            get {
                return this._entity;
            }

            set {
                this._entity.PropertyChanged -= this.Entity_PropertyChanged;
                this.Set(ref this._entity, value);
                this._entity.PropertyChanged += this.Entity_PropertyChanged;
            }
        }

        /// <inheritdoc />
        public bool IsEnabled {
            get {
                return this._isEnabled && this.Entity.IsEnabled;
            }

            set {
                this.Set(ref this._isEnabled, value, this.Entity.IsEnabled);
            }
        }

        /// <inheritdoc />
        public virtual void Initialize(IGameEntity entity) {
            this.Entity = entity;
            this.PropertyChanged += this.GameComponent_PropertyChanged;
        }

        /// <summary>
        /// Is called when the <see cref="Entity" /> has a property change according to <see
        /// cref="INotifyPropertyChanged" />.
        /// </summary>
        /// <param name="e">
        /// The <see cref="PropertyChangedEventArgs" /> instance containing the event data.
        /// </param>
        protected virtual void OnEntityPropertyChanged(PropertyChangedEventArgs e) {
            return;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
            return;
        }

        private void Entity_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(IGameEntity.IsEnabled)) {
                if (this._isEnabled) {
                    this.RaisePropertyChanged(nameof(this.IsEnabled));
                }
            }

            this.OnEntityPropertyChanged(e);
        }

        private void GameComponent_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.OnPropertyChanged(e);
        }
    }
}