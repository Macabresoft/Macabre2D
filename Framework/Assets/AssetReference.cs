namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for an asset reference.
/// </summary>
public interface IAssetReference : INotifyPropertyChanged {

    /// <summary>
    /// Gets the asset type.
    /// </summary>
    public Type AssetType { get; }

    /// <summary>
    /// Gets the content type.
    /// </summary>
    public Type ContentType { get; }

    /// <summary>
    /// Gets or sets the asset identifier.
    /// </summary>
    public Guid ContentId { get; set; }

    /// <summary>
    /// Clears the asset reference.
    /// </summary>
    void Clear();

    /// <summary>
    /// Deinitializes this reference.
    /// </summary>
    void Deinitialize();

    /// <summary>
    /// Initializes this reference with the asset manager.
    /// </summary>
    /// <param name="assetManager">The asset manager.</param>
    void Initialize(IAssetManager assetManager);

    /// <summary>
    /// Loads this instance with the content identifier..
    /// </summary>
    /// <param name="contentId">The content identifier.</param>
    void LoadAsset(Guid contentId);
}

/// <summary>
/// Interface for an asset reference.
/// </summary>
/// <typeparam name="TAsset">The asset.</typeparam>
public interface IAssetReference<TAsset> : IAssetReference where TAsset : class, IAsset {
    /// <summary>
    /// Gets the asset.
    /// </summary>
    TAsset? Asset { get; }

    /// <summary>
    /// Loads this instance with an asset.
    /// </summary>
    /// <param name="asset">The asset.</param>
    void LoadAsset(TAsset asset);
}

/// <summary>
/// A reference to an asset using an identifier and type for serialization purposes.
/// </summary>
[DataContract]
public abstract class AssetReference : PropertyChangedNotifier, IAssetReference {
    private Guid _contentId = Guid.Empty;

    /// <inheritdoc />
    public abstract Type AssetType { get; }

    /// <inheritdoc />
    public abstract Type ContentType { get; }

    /// <inheritdoc />
    [DataMember]
    public Guid ContentId {
        get => this._contentId;
        set => this.Set(ref this._contentId, value);
    }

    /// <summary>
    /// Gets the asset manager.
    /// </summary>
    protected IAssetManager Assets { get; private set; } = AssetManager.Empty;

    /// <inheritdoc />
    public abstract void Clear();

    /// <inheritdoc />
    public abstract void Deinitialize();

    /// <inheritdoc />
    public virtual void Initialize(IAssetManager assetManager) {
        this.Assets = assetManager;
    }

    /// <inheritdoc />
    public abstract void LoadAsset(Guid contentId);
}

/// <summary>
/// A reference to an asset using an identifier and type for serialization purposes.
/// </summary>
/// <typeparam name="TAsset">The type of the referenced asset.</typeparam>
/// <typeparam name="TContent">The type of the content this asset uses.</typeparam>
public class AssetReference<TAsset, TContent> : AssetReference, IAssetReference<TAsset> where TAsset : class, IAsset, IAsset<TContent> where TContent : class {
    private TAsset? _asset;

    /// <summary>
    /// An event called when an asset is loaded.
    /// </summary>
    public event EventHandler? AssetLoaded;

    /// <inheritdoc />
    public override Type AssetType => typeof(TAsset);

    /// <inheritdoc />
    public override Type ContentType => typeof(TContent);

    /// <inheritdoc />
    public TAsset? Asset {
        get => this._asset;
        private set => this.Set(ref this._asset, value);
    }

    /// <inheritdoc />
    public override void Clear() {
        if (this.Asset != null) {
            this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
        }

        this.Asset = null;
        this.ContentId = Guid.Empty;
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        if (this.Asset != null) {
            this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
        }
    }

    /// <inheritdoc />
    public override void Initialize(IAssetManager assetManager) {
        base.Initialize(assetManager);

        if (this.Assets.TryGetAsset(this, out var asset)) {
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
        this.Assets.LoadContentForAsset<TContent>(this.Asset);
        this.AssetLoaded.SafeInvoke(this);
    }

    /// <inheritdoc />
    public override void LoadAsset(Guid contentId) {
        this.ContentId = contentId;

        if (this.ContentId == Guid.Empty) {
            this.Clear();
        }
        else if (this.Assets.TryGetAsset(this, out var asset)) {
            this.LoadAsset(asset);
        }
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