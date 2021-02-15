namespace Macabresoft.Macabre2D.Framework {

    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a prefab that can be loaded as content. Stores a <see cref="BaseComponent" />
    /// that can be copied many times over.
    /// </summary>
    public sealed class Prefab : BaseIdentifiable, IAsset, IDisposable {
        private bool _disposedValue = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Prefab" /> class.
        /// </summary>
        public Prefab() : this(Guid.NewGuid(), GameEntity.Empty) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Prefab" /> class.
        /// </summary>
        /// <param name="assetId">The asset identifier.</param>
        /// <param name="component">The component.</param>
        public Prefab(Guid assetId, IGameEntity component) {
            this.ContentId = assetId;
            this.Entity = component;
            this.Name = this.Entity.Name;
        }

        /// <summary>
        /// Gets or sets the asset identifier.
        /// </summary>
        /// <value>The asset identifier.</value>
        [DataMember]
        public Guid ContentId { get; set; }

        /// <summary>
        /// Gets or sets the component this is a prefab for.
        /// </summary>
        /// <value>The component.</value>
        [DataMember]
        public IGameEntity Entity { get; set; }

        [DataMember]
        public string Name { get; set; }

        /// <inheritdoc />
        public void Dispose() {
            this.Dispose(true);
        }

        private void Dispose(bool disposing) {
            if (!this._disposedValue) {
                if (disposing) {
                    if (this.Entity is IDisposable disposable) {
                        disposable.Dispose();
                    }
                }

                this._disposedValue = true;
            }
        }
    }
}