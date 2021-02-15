namespace Macabresoft.Macabre2D.Framework {
    using System;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Interface for an object that is an asset.
    /// </summary>
    public interface IAsset : INotifyPropertyChanged {
        /// <summary>
        /// Gets the content identifier.
        /// </summary>
        /// <value>The content identifier.</value>
        Guid ContentId { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
    }

    /// <summary>
    /// Interface for an asset that contains content.
    /// </summary>
    public interface IAsset<TContent> : IAsset {
        /// <summary>
        /// Gets the content.
        /// </summary>
        TContent? Content { get; }

        /// <summary>
        /// Loads content for this asset.
        /// </summary>
        /// <param name="content">The content.</param>
        void LoadContent(TContent content);
    }

    /// <summary>
    /// A base implementation for assets that contains an identifier and name.
    /// </summary>
    [DataContract]
    public abstract class Asset<TContent> : NotifyPropertyChanged, IAsset<TContent>, IDisposable {
        private TContent? _content;
        private string _name = string.Empty;

        /// <inheritdoc />
        [DataMember]
        public Guid ContentId { get; } = Guid.NewGuid();

        /// <inheritdoc />
        public TContent? Content {
            get => this._content;
            set => this.Set(ref this._content, value);
        }

        /// <inheritdoc />
        [DataMember]
        public string Name {
            get => this._name;
            set => this.Set(ref this._name, value);
        }

        /// <inheritdoc />
        public virtual void LoadContent(TContent content) {
            this.Content = content;
        }
        
        /// <inheritdoc />
        public void Dispose() {
            if (this.Content is IDisposable disposable) {
                disposable.Dispose();
            }
            
            this.DisposePropertyChanged();
        }
    }
}