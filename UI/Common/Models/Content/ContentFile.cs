namespace Macabresoft.Macabre2D.UI.Common {
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using Macabresoft.Macabre2D.Framework;

    /// <summary>
    /// A content file for the project.
    /// </summary>
    [DataContract(Name = "File")]
    public class ContentFile : ContentNode {
        private bool _hasChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentFile" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="metadata">The metadata.</param>
        public ContentFile(IContentDirectory parent, ContentMetadata metadata) {
            this.Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            this.Asset.PropertyChanged += this.Asset_PropertyChanged;
            this.Initialize(metadata.GetFileName(), parent);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentFile" /> class.
        /// </summary>
        /// <remarks>
        /// This exists purely for data context purposes in XAML.
        /// </remarks>
        public ContentFile() : base() {
        }

        /// <summary>
        /// Gets the asset.
        /// </summary>
        [DataMember]
        [Category(nameof(Asset))]
        public IAsset Asset => this.Metadata?.Asset;

        /// <inheritdoc />
        public override Guid Id => this.Metadata?.ContentId ?? Guid.Empty;

        /// <summary>
        /// The metadata.
        /// </summary>
        public ContentMetadata Metadata { get; }

        /// <summary>
        /// Gets a value indicating whether or not this content has changes.
        /// </summary>
        public bool HasChanges {
            get => this._hasChanges;
            set => this.Set(ref this._hasChanges, value);
        }

        /// <inheritdoc />
        protected override string GetFileExtension() {
            return this.Metadata.ContentFileExtension;
        }

        /// <inheritdoc />
        protected override string GetNameWithoutExtension() {
            return Path.GetFileNameWithoutExtension(this.Name);
        }

        /// <inheritdoc />
        protected override void OnPathChanged(string originalPath) {
            this.Metadata.SetContentPath(this.GetContentPath());
            base.OnPathChanged(originalPath);
        }

        private void Asset_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            this.HasChanges = true;
        }
    }

    /// <summary>
    /// A typed content file.
    /// </summary>
    /// <typeparam name="T">The type of asset associated with the content.</typeparam>
    public class ContentFile<T> : ContentFile where T : class, IAsset {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentNode" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="metadata">The metadata.</param>
        public ContentFile(IContentDirectory parent, ContentMetadata metadata) : base(parent, metadata) {
            this.TypedAsset = metadata.Asset as T ?? throw new ArgumentException("Typed content files must be initialized with an asset.");
        }

        /// <summary>
        /// Gets the typed asset.
        /// </summary>
        public T TypedAsset { get; }
    }
}