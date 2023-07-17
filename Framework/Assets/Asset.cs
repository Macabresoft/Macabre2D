namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using Macabresoft.Core;

/// <summary>
/// Interface for an object that is an asset.
/// </summary>
public interface IAsset : INotifyPropertyChanged {
    /// <summary>
    /// Gets the content identifier.
    /// </summary>
    Guid ContentId { get; }

    /// <summary>
    /// Gets a value indicating whether or not this should include the file extension in its content path.
    /// </summary>
    bool IncludeFileExtensionInContentPath { get; }

    /// <summary>
    /// Gets a value indicating whether or not this asset should be ignored when building a content file.
    /// </summary>
    bool IgnoreInBuild { get; }
    
    /// <summary>
    /// Gets the developer notes for this asset.
    /// </summary>
    string Notes { get; }

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
[Category("Asset")]
[DataContract]
public abstract class Asset<TContent> : PropertyChangedNotifier, IAsset<TContent> {
    private bool _ignoreInBuild;
    private string _notes = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="Asset{TContent}" /> class.
    /// </summary>
    protected Asset() {
        this.ContentId = Guid.NewGuid();
    }

    /// <inheritdoc />
    public abstract bool IncludeFileExtensionInContentPath { get; }

    /// <inheritdoc />
    public TContent? Content { get; protected set; }

    /// <inheritdoc />
    [DataMember]
    [EditorExclude]
    public Guid ContentId { get; private set; }

    /// <inheritdoc />
    [DataMember]
    public bool IgnoreInBuild {
        get => this._ignoreInBuild;
        set => this.Set(ref this._ignoreInBuild, value);
    }

    /// <inheritdoc />
    [DataMember]
    public string Notes {
        get => this._notes;
        set => this.Set(ref this._notes, value);
    }

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