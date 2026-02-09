namespace Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// Interface for a <see cref="IAssetReference{SpriteSheet}" />.
/// </summary>
public interface ISpriteSheetAssetReference : IAssetReference<SpriteSheet> {

    /// <summary>
    /// Gets the asset name.
    /// </summary>
    string AssetName { get; }

    /// <summary>
    /// Gets or sets the packaged asset identifier.
    /// </summary>
    Guid PackagedAssetId { get; set; }

    /// <summary>
    /// Gets or sets the packaged asset type.
    /// </summary>
    Type PackagedAssetType { get; }
}

/// <summary>
/// An asset reference for an asset packaged inside a <see cref="SpriteSheet" />.
/// </summary>
/// <typeparam name="TPackagedAsset">The type of packaged asset.</typeparam>
public class SpriteSheetAssetReference<TPackagedAsset> : AssetReference<SpriteSheet, Texture2D>, ISpriteSheetAssetReference where TPackagedAsset : SpriteSheetMember {
    private Guid _packagedAssetId;

    /// <inheritdoc />
    public string AssetName => this.PackagedAsset?.Name ?? string.Empty;

    /// <summary>
    /// Gets the packaged asset.
    /// </summary>
    public TPackagedAsset? PackagedAsset {
        get;
        private set {
            var originalAsset = field;

            if (this.Set(ref field, value)) {
                if (originalAsset != null) {
                    originalAsset.PropertyChanged -= this.SpriteSheet_OnPropertyChanged;
                }

                if (field != null) {
                    field.PropertyChanged += this.SpriteSheet_OnPropertyChanged;
                }

                this.RaisePropertyChanged(nameof(this.AssetName));
            }
        }
    }

    /// <inheritdoc />
    [DataMember]
    public Guid PackagedAssetId {
        get => this._packagedAssetId;
        set {
            if (this.Set(ref this._packagedAssetId, value)) {
                this.ResolvePackagedAsset();
            }
        }
    }

    /// <inheritdoc />
    public Type PackagedAssetType => typeof(TPackagedAsset);

    /// <inheritdoc />
    public override void Clear() {
        base.Clear();
        this.PackagedAsset = null;
        this._packagedAssetId = Guid.Empty;
    }

    /// <inheritdoc />
    public override void LoadAsset(SpriteSheet asset) {
        base.LoadAsset(asset);
        this.ResolvePackagedAsset();
    }

    /// <summary>
    /// Loads the asset via <see cref="Guid" /> for the content and the packaged asset at once.
    /// </summary>
    /// <param name="spriteSheetId">The content identifier.</param>
    /// <param name="packagedAssetId">The packaged asset identifier.</param>
    public void LoadAsset(Guid spriteSheetId, Guid packagedAssetId) {
        this.Clear();
        this._packagedAssetId = packagedAssetId;
        this.ContentId = spriteSheetId;
    }

    /// <summary>
    /// Resets this reference by clearing it out and initializing it with a new packaged asset.
    /// </summary>
    /// <param name="packagedAsset">The packaged asset.</param>
    public void Reset(TPackagedAsset packagedAsset) {
        if (packagedAsset.SpriteSheet is { } spriteSheet) {
            if (this.Asset == null || this.Asset.ContentId != spriteSheet.ContentId) {
                this.Clear();
            }

            this.PackagedAssetId = packagedAsset.Id;
            this.LoadAsset(spriteSheet);
        }
    }

    private void ResolvePackagedAsset() {
        if (this.PackagedAssetId != Guid.Empty && this.Asset != null && this.Asset.TryGetPackaged<TPackagedAsset>(this.PackagedAssetId, out var packagedAsset)) {
            this.PackagedAsset = packagedAsset;
        }
        else {
            this.PackagedAsset = null;
        }
    }

    private void SpriteSheet_OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
        this.RaisePropertyChanged(sender, e);
    }
}