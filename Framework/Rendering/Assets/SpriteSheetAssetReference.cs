namespace Macabresoft.Macabre2D.Framework;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Graphics;

/// <summary>
/// An asset reference for an asset packaged inside of a <see cref="SpriteSheet" />.
/// </summary>
/// <typeparam name="TPackagedAsset">The type of packaged asset.</typeparam>
public class SpriteSheetAssetReference<TPackagedAsset> : AssetReference<SpriteSheet, Texture2D> where TPackagedAsset : SpriteSheetMember {
    private TPackagedAsset? _packagedAsset;
    private Guid _packagedAssetId;

    /// <summary>
    /// Gets the packaged asset.
    /// </summary>
    public TPackagedAsset? PackagedAsset {
        get => this._packagedAsset;
        private set {
            var originalAsset = this._packagedAsset;

            if (this.Set(ref this._packagedAsset, value)) {
                if (originalAsset != null) {
                    originalAsset.PropertyChanged -= this.SpriteSheet_OnPropertyChanged;
                }

                if (this._packagedAsset != null) {
                    this._packagedAsset.PropertyChanged += this.SpriteSheet_OnPropertyChanged;
                }
            }
        }
    }

    /// <summary>
    /// Gets or sets the packaged asset identifier.
    /// </summary>
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