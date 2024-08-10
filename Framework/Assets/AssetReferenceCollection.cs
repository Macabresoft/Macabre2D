namespace Macabresoft.Macabre2D.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

/// <summary>
/// Interface for a collection of assets.
/// </summary>
public interface IAssetReferenceCollection : IAssetReference {
    /// <summary>
    /// Gets the content identifiers associated with this collection.
    /// </summary>
    IReadOnlyCollection<Guid> ContentIds { get; }

    /// <summary>
    /// Removes the asset associated with the specified identifier.
    /// </summary>
    /// <param name="contentId">The content identifier.</param>
    void AddAsset(Guid contentId);

    /// <summary>
    /// Removes the asset associated with the specified identifier.
    /// </summary>
    /// <param name="contentId">The content identifier.</param>
    void RemoveAsset(Guid contentId);
}

/// <summary>
/// Interface for a collection of assets.
/// </summary>
/// <typeparam name="TAsset">The type of asset.</typeparam>
public interface IAssetReferenceCollection<TAsset> : IAssetReferenceCollection where TAsset : class, IAsset {
    /// <summary>
    /// Gets the assets.
    /// </summary>
    IReadOnlyCollection<TAsset> Assets { get; }

    /// <summary>
    /// Adds the asset.
    /// </summary>
    /// <param name="asset">The asset.</param>
    void AddAsset(TAsset asset);

    /// <summary>
    /// Removes the asset.
    /// </summary>
    /// <param name="asset">The asset.</param>
    void RemoveAsset(TAsset asset);
}

/// <summary>
/// Collection of references to assets.
/// </summary>
/// <typeparam name="TAsset">The type of asset.</typeparam>
/// <typeparam name="TContent">The type of content.</typeparam>
public class AssetReferenceCollection<TAsset, TContent> : AssetReference, IAssetReferenceCollection<TAsset> where TAsset : class, IAsset, IAsset<TContent> where TContent : class {
    private readonly List<TAsset> _assets = new();

    [DataMember]
    private readonly HashSet<Guid> _contentIds = new();

    private bool _isInitialized;

    /// <inheritdoc />
    public IReadOnlyCollection<TAsset> Assets => this._assets;

    /// <inheritdoc />
    public override Type AssetType => typeof(TAsset);

    /// <inheritdoc />
    public IReadOnlyCollection<Guid> ContentIds => this._contentIds;

    /// <inheritdoc />
    public override Type ContentType => typeof(TContent);

    /// <inheritdoc />
    public override bool HasContent => this._contentIds.Any();

    /// <inheritdoc />
    public void AddAsset(TAsset asset) {
        if (this._contentIds.Add(asset.ContentId) && this._isInitialized) {
            this.LoadAndAddAsset(asset);
        }
        
        this.RaisePropertyChanged(nameof(this.HasContent));
    }

    /// <inheritdoc />
    public void AddAsset(Guid contentId) {
        if (this.AssetManager.TryGetAsset<TAsset, TContent>(contentId, out var asset)) {
            this.AddAsset(asset);
        }
    }

    /// <inheritdoc />
    public override void Clear() {
        this._contentIds.Clear();
        this._assets.Clear();
    }

    /// <inheritdoc />
    public override void Deinitialize() {
        this._assets.Clear();
        this._isInitialized = false;
    }

    /// <inheritdoc />
    public override void Initialize(IAssetManager assetManager) {
        try {
            base.Initialize(assetManager);

            foreach (var contentId in this._contentIds) {
                if (this.AssetManager.TryGetAsset<TAsset, TContent>(contentId, out var asset)) {
                    this.LoadAndAddAsset(asset);
                }
            }
        }
        finally {
            this._isInitialized = true;
        }
    }

    /// <inheritdoc />
    public void RemoveAsset(TAsset asset) {
        this._contentIds.Remove(asset.ContentId);
        this._assets.Remove(asset);
        this.RaisePropertyChanged(nameof(this.HasContent));
    }

    /// <inheritdoc />
    public void RemoveAsset(Guid contentId) {
        this._contentIds.Remove(contentId);
        foreach (var asset in this._assets.Where(x => x.ContentId == contentId).ToList()) {
            this._assets.Remove(asset);
        }

        this.RaisePropertyChanged(nameof(this.HasContent));
    }

    private void LoadAndAddAsset(TAsset asset) {
        this.AssetManager.LoadContentForAsset<TContent>(asset);
        this._assets.Add(asset);
    }
}