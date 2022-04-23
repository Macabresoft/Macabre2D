namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

/// <summary>
/// A reference to an asset using identifier and type. Held by entities for serialization.
/// </summary>
/// <typeparam name="TAsset">The type of the referenced asset.</typeparam>
/// <typeparam name="TContent">The type of the content this asset uses.</typeparam>
[DataContract]
public class AssetReference<TAsset, TContent> : NotifyPropertyChanged where TAsset : class, IAsset, IAsset<TContent> where TContent : class {
    private TAsset? _asset;
    private IAssetManager _assetManager = AssetManager.Empty;
    private Guid _contentId;

    /// <summary>
    /// Gets the asset.
    /// </summary>
    public TAsset? Asset {
        get => this._asset;
        private set => this.Set(ref this._asset, value);
    }

    /// <summary>
    /// Gets or sets the asset identifier.
    /// </summary>
    [DataMember]
    public Guid ContentId {
        get => this._contentId;
        private set => this.Set(ref this._contentId, value);
    }

    /// <summary>
    /// Clears the asset reference.
    /// </summary>
    public virtual void Clear() {
        if (this.Asset != null) {
            this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
        }

        this.Asset = null;
        this.ContentId = Guid.Empty;
    }

    /// <summary>
    /// Initializes this reference with the asset manager.
    /// </summary>
    /// <param name="assetManager"></param>
    public void Initialize(IAssetManager assetManager) {
        this._assetManager = assetManager;

        if (this._assetManager.TryGetAsset(this, out var asset) && asset != null) {
            this.LoadAsset(asset);
        }
    }

    /// <summary>
    /// Loads this instance with an asset.
    /// </summary>
    /// <param name="asset">The asset.</param>
    public virtual void LoadAsset(TAsset asset) {
        if (this.Asset != null) {
            this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
        }

        this.Asset = asset;
        this.Asset.PropertyChanged += this.Asset_PropertyChanged;
        this.ContentId = this.Asset.ContentId;
        this._assetManager.LoadContentForAsset<TContent>(this.Asset);
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