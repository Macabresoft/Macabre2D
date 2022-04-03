namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

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
    /// Gets a value indicating whether or not this should include the file extension in its content path.
    /// </summary>
    bool IncludeFileExtensionInContentPath { get; }

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
public abstract class Asset<TContent> : NotifyPropertyChanged, IAsset<TContent> {
    private IAssetManager _assetManager = AssetManager.Empty;
    private TContent? _content;

    /// <summary>
    /// Initializes a new instance of the <see cref="Asset{TContent}" /> class.
    /// </summary>
    protected Asset() {
        this.ContentId = Guid.NewGuid();
    }

    /// <inheritdoc />
    public abstract bool IncludeFileExtensionInContentPath { get; }

    /// <inheritdoc />
    public TContent? Content {
        get => this._content;
        protected set => this.Set(ref this._content, value);
    }

    /// <inheritdoc />
    [DataMember]
    [EditorExclude]
    public Guid ContentId { get; private set; }

    /// <inheritdoc />
    public virtual string GetContentBuildCommands(string contentPath, string fileExtension) {
        var contentStringBuilder = new StringBuilder();
        contentStringBuilder.AppendLine($"#begin {contentPath}");
        contentStringBuilder.AppendLine($@"/copy:{contentPath}");
        contentStringBuilder.AppendLine($"#end {contentPath}");
        return contentStringBuilder.ToString();
    }

    /// <inheritdoc />
    public virtual void LoadContent(TContent content) {
        this.Content = content;
    }

    /// <inheritdoc />
    protected override void OnDisposing() {
        base.OnDisposing();

        if (this.Content is IDisposable disposable) {
            disposable.Dispose();
        }
    }
}