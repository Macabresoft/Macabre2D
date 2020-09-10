namespace Macabresoft.MonoGame.Core {

    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for a descendent of <see cref="IGameEntity" />.
    /// </summary>
    public interface IGameComponent : IEnableable, IIdentifiable, INotifyPropertyChanged, IDisposable {

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>The entity.</value>
        IGameEntity Entity { get => GameEntity.Empty; }

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
        private bool _isEnabled = true;

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

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
        protected bool IsDisposed { get; private set; }

        /// <inheritdoc />
        public void Dispose() {
            if (!this.IsDisposed) {
                this.Entity.PropertyChanged -= this.Entity_PropertyChanged;
                this.PropertyChanged -= this.Self_PropertyChanged;

                Dispose(true);
                this.IsDisposed = true;
            }

            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public virtual void Initialize(IGameEntity entity) {
            this.Entity = entity;
            this.PropertyChanged += this.Self_PropertyChanged;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
        /// only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing) {
            return;
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

        /// <summary>
        /// Is called when this instance has a property change.
        /// </summary>
        /// <param name="e">
        /// The <see cref="PropertyChangedEventArgs" /> instance containing the event data.
        /// </param>
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

        private void Self_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.OnPropertyChanged(e);
        }

        internal class EmptyGameComponent : IGameComponent {

            /// <inheritdoc />
            public event PropertyChangedEventHandler? PropertyChanged;

            /// <inheritdoc />
            public Guid Id {
                get {
                    return Guid.Empty;
                }

                set {
                    return;
                }
            }

            /// <inheritdoc />
            public bool IsEnabled {
                get {
                    return false;
                }

                set {
                    return;
                }
            }

            /// <inheritdoc />
            public void Dispose() {
                return;
            }

            /// <inheritdoc />
            public void Initialize(IGameEntity entity) {
                return;
            }
        }
    }
}