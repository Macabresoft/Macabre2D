namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for an asset reference.
/// </summary>
/// <typeparam name="TAsset">The asset.</typeparam>
public interface IAssetReference<TAsset> : INotifyPropertyChanged where TAsset : class, IAsset {
    /// <summary>
    /// Gets the asset.
    /// </summary>
    TAsset? Asset { get; }

    /// <summary>
    /// Gets or sets the asset identifier.
    /// </summary>
    public Guid ContentId { get; set; }

    /// <summary>
    /// Clears the asset reference.
    /// </summary>
    void Clear();

    /// <summary>
    /// Initializes this reference with the asset manager.
    /// </summary>
    /// <param name="assetManager">The asset manager.</param>
    void Initialize(IAssetManager assetManager);

    /// <summary>
    /// Loads this instance with an asset.
    /// </summary>
    /// <param name="asset">The asset.</param>
    void LoadAsset(TAsset asset);
}

/// <summary>
/// A reference to an asset using an identifier and type for serialization purposes.
/// </summary>
/// <typeparam name="TAsset">The type of the referenced asset.</typeparam>
/// <typeparam name="TContent">The type of the content this asset uses.</typeparam>
[DataContract]
public class AssetReference<TAsset, TContent> : PropertyChangedNotifier, IAssetReference<TAsset> where TAsset : class, IAsset, IAsset<TContent> where TContent : class {
    private TAsset? _asset;
    private IAssetManager _assetManager = AssetManager.Empty;
    private Guid _contentId = Guid.Empty;

    /// <summary>
    /// An event called when an asset is loaded.
    /// </summary>
    public event EventHandler? AssetLoaded;

    /// <inheritdoc />
    public TAsset? Asset {
        get => this._asset;
        private set => this.Set(ref this._asset, value);
    }

    /// <inheritdoc />
    [DataMember]
    public Guid ContentId {
        get => this._contentId;
        set => this.Set(ref this._contentId, value);
    }

    /// <inheritdoc />
    public virtual void Clear() {
        if (this.Asset != null) {
            this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
        }

        this.Asset = null;
        this.ContentId = Guid.Empty;
    }

    /// <inheritdoc />
    public virtual void Initialize(IAssetManager assetManager) {
        this._assetManager = assetManager;

        if (this._assetManager.TryGetAsset(this, out var asset) && asset != null) {
            this.LoadAsset(asset);
        }
    }

    /// <inheritdoc />
    public virtual void LoadAsset(TAsset asset) {
        if (this.Asset != null) {
            this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
        }

        this.Asset = asset;
        this.Asset.PropertyChanged += this.Asset_PropertyChanged;
        this.ContentId = this.Asset.ContentId;
        this._assetManager.LoadContentForAsset<TContent>(this.Asset);
        this.AssetLoaded.SafeInvoke(this);
    }

    /// <summary>
    /// Called when an asset property changes.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnAssetPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.RaisePropertyChanged(sender ?? this, e);
    }

    /// <inheritdoc />
    protected override void OnDisposing() {
        base.OnDisposing();

        if (this.Asset != null) {
            this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
        }
    }

    private void Asset_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.OnAssetPropertyChanged(sender, e);
    }
}