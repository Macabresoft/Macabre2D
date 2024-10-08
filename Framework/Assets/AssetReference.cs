namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Macabresoft.Core;

/// <summary>
/// Interface for an asset reference.
/// </summary>
public interface IAssetReference : INotifyPropertyChanged {
    /// <summary>
    /// An event called when an asset is loaded. The <see cref="bool" /> passed in indicates whether an asset exists or not.
    /// </summary>
    event EventHandler<bool>? AssetChanged;

    /// <summary>
    /// Gets the asset type.
    /// </summary>
    public Type AssetType { get; }

    /// <summary>
    /// Gets the content type.
    /// </summary>
    public Type ContentType { get; }

    /// <summary>
    /// Gets a value indicating whether this has content.
    /// </summary>
    public bool HasContent { get; }

    /// <summary>
    /// Clears the asset reference.
    /// </summary>
    void Clear();

    /// <summary>
    /// Deinitializes this reference.
    /// </summary>
    void Deinitialize();

    /// <summary>
    /// Gets the content identifiers associated with this instance.
    /// </summary>
    /// <returns>The content identifiers.</returns>
    public IEnumerable<Guid> GetContentIds();

    /// <summary>
    /// Initializes this reference with the asset manager.
    /// </summary>
    /// <param name="assetManager">The asset manager.</param>
    /// <param name="game">The game</param>
    void Initialize(IAssetManager assetManager, IGame game);
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
    /// Gets or sets the asset identifier.
    /// </summary>
    public Guid ContentId { get; set; }

    /// <summary>
    /// Loads this instance with the content identifier..
    /// </summary>
    /// <param name="contentId">The content identifier.</param>
    void LoadAsset(Guid contentId);

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

    /// <inheritdoc />
    public event EventHandler<bool>? AssetChanged;

    /// <inheritdoc />
    public abstract Type AssetType { get; }

    /// <inheritdoc />
    public abstract Type ContentType { get; }

    /// <inheritdoc />
    public abstract bool HasContent { get; }

    /// <summary>
    /// Gets the asset manager.
    /// </summary>
    protected IAssetManager AssetManager { get; private set; } = Framework.AssetManager.Empty;

    /// <inheritdoc />
    public abstract void Clear();

    /// <inheritdoc />
    public abstract void Deinitialize();

    /// <inheritdoc />
    public abstract IEnumerable<Guid> GetContentIds();

    /// <inheritdoc />
    public virtual void Initialize(IAssetManager assets, IGame game) {
        this.AssetManager = assets;
    }

    /// <summary>
    /// Raises the <see cref="AssetChanged" /> event.
    /// </summary>
    protected void RaiseAssetChanged() {
        this.AssetChanged.SafeInvoke(this, this.HasContent);
    }
}

/// <summary>
/// A reference to an asset using an identifier and type for serialization purposes.
/// </summary>
/// <typeparam name="TAsset">The type of the referenced asset.</typeparam>
/// <typeparam name="TContent">The type of the content this asset uses.</typeparam>
public class AssetReference<TAsset, TContent> : AssetReference, IAssetReference<TAsset> where TAsset : class, IAsset, IAsset<TContent> where TContent : class {
    private TAsset? _asset;
    private Guid _contentId = Guid.Empty;

    /// <inheritdoc />
    public override Type AssetType => typeof(TAsset);

    /// <inheritdoc />
    public override Type ContentType => typeof(TContent);

    /// <inheritdoc />
    public override bool HasContent => this._contentId != Guid.Empty;

    /// <inheritdoc />
    public TAsset? Asset {
        get => this._asset;
        private set => this.Set(ref this._asset, value);
    }

    /// <inheritdoc />
    [DataMember]
    public Guid ContentId {
        get => this._contentId;
        set {
            if (this.Set(ref this._contentId, value)) {
                this.LoadAsset(this.ContentId);
            }
        }
    }

    /// <inheritdoc />
    public override void Clear() {
        if (this.Asset != null) {
            this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
        }

        this.Asset = null;
        this.ContentId = Guid.Empty;
        this.RaiseAssetChanged();
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        if (this.Asset != null) {
            this.Asset.PropertyChanged -= this.Asset_PropertyChanged;
            this.Asset = null;
        }
    }

    /// <inheritdoc />
    public override IEnumerable<Guid> GetContentIds() => this.ContentId != Guid.Empty ? [this.ContentId] : [];

    /// <inheritdoc />
    public override void Initialize(IAssetManager assetManager, IGame game) {
        base.Initialize(assetManager, game);

        if (this.AssetManager.TryGetAsset(this, out var asset)) {
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
        this.AssetManager.LoadContentForAsset<TContent>(this.Asset);
        this.RaiseAssetChanged();
    }

    /// <inheritdoc />
    public void LoadAsset(Guid contentId) {
        this._contentId = contentId;
        this.RaisePropertyChanged(nameof(this.ContentId));

        if (this.ContentId == Guid.Empty) {
            this.Clear();
        }
        else if (this.AssetManager.TryGetAsset(this, out var asset)) {
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