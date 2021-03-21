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

        /// <summary>
        /// Gets the content build commands used by MGCB to compile this piece of content.
        /// </summary>
        /// <param name="contentPath">The content path.</param>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns>The content build commands.</returns>
        string GetContentBuildCommands(string contentPath, string fileExtension);
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Asset{TContent}" /> class.
        /// </summary>
        protected Asset() {
            this.ContentId = Guid.NewGuid();
        }

        /// <inheritdoc />
        public TContent? Content {
            get => this._content;
            protected set => this.Set(ref this._content, value);
        }

        /// <inheritdoc />
        [DataMember]
        public Guid ContentId { get; private set; }

        /// <inheritdoc />
        [DataMember]
        public string Name {
            get => this._name;
            set => this.Set(ref this._name, value);
        }

        /// <inheritdoc />
        public void Dispose() {
            if (this.Content is IDisposable disposable) {
                disposable.Dispose();
            }

            this.DisposePropertyChanged();
        }

        /// <inheritdoc />
        public abstract string GetContentBuildCommands(string contentPath, string fileExtension);

        /// <inheritdoc />
        public virtual void LoadContent(TContent content) {
            this.Content = content;
        }
    }
}