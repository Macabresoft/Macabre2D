namespace Macabre2D.UI.Library.Models {

    using Macabre2D.UI.Library.Common;
    using Macabre2D.UI.Library.Common.Extensions;
    using Macabre2D.Framework;
    using MahApps.Metro.IconPacks;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;

    public class FolderAsset : Asset, IParent<Asset> {

        [DataMember]
        private readonly ObservableCollection<Asset> _children = new ObservableCollection<Asset>();

        public FolderAsset(string name) : base(name) {
            this._children.CollectionChanged += this.Children_CollectionChanged;
        }

        internal FolderAsset() {
            this._children.CollectionChanged += this.Children_CollectionChanged;
        }

        public event EventHandler<Asset> OnAssetAdded;

        public IReadOnlyCollection<Asset> Children {
            get {
                return this._children;
            }
        }

        public override PackIconMaterialKind Icon {
            get {
                return PackIconMaterialKind.Folder;
            }
        }

        public bool AddChild(Asset child) {
            if (child != null && !this._children.Any(x => string.Equals(x.Name, child.Name, StringComparison.CurrentCultureIgnoreCase))) {
                this._children.Add(child);
                child.OnDeleted += this.Child_OnDeleted;
                this.RaisePropertyChanged(nameof(this.Children));
                this.OnAssetAdded.SafeInvoke(this, child);
                this._children.Sort((a, b) => Asset.Compare(a, b));
                return true;
            }

            return false;
        }

        public override void Delete() {
            var children = this._children.ToList();
            foreach (var child in children) {
                child.Delete();
            }

            Directory.Delete(this.GetPath(), true);
            this.RaiseOnDeleted();
        }

        public IEnumerable<Asset> GetAllContentAssets() {
            var children = new List<Asset>();

            foreach (var child in this.Children) {
                if (child is FolderAsset folderAsset) {
                    children.AddRange(folderAsset.GetAllContentAssets());
                }
                else {
                    children.Add(child);
                }
            }

            return children;
        }

        public IEnumerable<TAsset> GetAssetsOfType<TAsset>() {
            var assets = new List<TAsset>();

            foreach (var child in this._children) {
                if (child is TAsset asset) {
                    assets.Add(asset);
                }
                else if (child is FolderAsset folder) {
                    assets.AddRange(folder.GetAssetsOfType<TAsset>());
                }
                else if (child is IParent<TAsset> parent) {
                    assets.AddRange(parent.Children);
                }
            }

            return assets;
        }

        public virtual void Refresh() {
            var path = this.GetPath();

            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            var folders = Directory.EnumerateDirectories(path).OrderBy(x => x);
            var folderAssetsToAdd = new List<FolderAsset>();
            var files = Directory.EnumerateFiles(path).OrderBy(x => x);
            var assetsToAdd = new List<Asset>();

            var children = this._children.ToList();
            this._children.Clear();

            foreach (var folder in folders) {
                var folderName = Path.GetFileName(folder);

                if (children.FirstOrDefault(x => string.Equals(x.Name, folderName)) is FolderAsset folderAsset) {
                    children.Remove(folderAsset);
                }
                else {
                    folderAsset = new FolderAsset(folderName);
                }

                this.AddChild(folderAsset);
                folderAsset.Refresh();
            }

            foreach (var file in files) {
                var fileName = Path.GetFileName(file);

                var asset = children.FirstOrDefault(x => string.Equals(x.Name, fileName));
                if (asset != null) {
                    children.Remove(asset);
                }
                else {
                    asset = GetAssetFromFilePath(file);
                }

                this.AddChild(asset);
            }

            foreach (var child in children) {
                if (!child.IsContent) {
                    this._children.Add(child);

                    if (child is FolderAsset folderAsset) {
                        Directory.CreateDirectory(folderAsset.GetPath());
                    }
                }
                else if (child is IIdentifiableAsset identifiable) {
                    foreach (var indentifier in identifiable.GetOwnedAssetIds()) {
                        this.RemoveIdentifiableContentFromScenes(indentifier);
                    }
                }
            }

            this.RaisePropertyChanged(nameof(this.Children));
        }

        public bool RemoveChild(Asset child) {
            if (this._children.Remove(child)) {
                this.RaisePropertyChanged(nameof(this.Children));
                return true;
            }

            return false;
        }

        internal static Asset GetAssetFromFilePath(string filePath) {
            Asset result;
            var fileName = Path.GetFileName(filePath);
            if (filePath.ToUpper().EndsWith(FileHelper.SceneExtension.ToUpper())) {
                result = CreateAssetFromMetadata<SceneAsset>(fileName);
            }
            else if (FileHelper.IsImageFile(fileName)) {
                result = CreateAssetFromMetadata<ImageAsset>(fileName);
            }
            else if (FileHelper.IsAudioFile(fileName)) {
                result = CreateAssetFromMetadata<AudioAsset>(fileName);
            }
            else if (filePath.ToUpper().EndsWith(FileHelper.SpriteAnimationExtension.ToUpper())) {
                result = CreateAssetFromMetadata<SpriteAnimationAsset>(fileName);
            }
            else if (filePath.ToUpper().EndsWith(FileHelper.SpriteFontExtension.ToUpper())) {
                result = CreateAssetFromMetadata<FontAsset>(fileName);
            }
            else if (filePath.ToUpper().EndsWith(FileHelper.AutoTileSetExtension.ToUpper())) {
                result = CreateAssetFromMetadata<AutoTileSetAsset>(fileName);
            }
            else if (filePath.ToUpper().EndsWith(FileHelper.PrefabExtension.ToUpper())) {
                result = CreateAssetFromMetadata<PrefabAsset>(fileName);
            }
            else if (filePath.ToUpper().EndsWith(FileHelper.ShaderExtension.ToUpper())) {
                result = CreateAssetFromMetadata<ShaderAsset>(fileName);
            }
            else {
                result = new Asset(fileName);
            }

            return result;
        }

        internal override void RemoveAssetReference(Guid id) {
            foreach (var child in this.Children) {
                child.RemoveAssetReference(id);
            }
        }

        private static T CreateAssetFromMetadata<T>(string fileName) where T : MetadataAsset, new() {
            var result = new T {
                Name = fileName
            };

            return result;
        }

        private void Child_OnDeleted(object sender, EventArgs e) {
            if (sender is Asset asset) {
                this._children.Remove(asset);
            }
        }

        private void Child_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName != nameof(MetadataAsset.HasChanges) || (sender is MetadataAsset metadataAsset && metadataAsset.HasChanges)) {
                this.RaisePropertyChanged(nameof(this.Children));
            }
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (var child in e.NewItems.Cast<Asset>().Where(x => x.Parent != this)) {
                    child.Parent = this;
                    child.PropertyChanged += this.Child_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (var child in e.OldItems.Cast<Asset>().Where(x => x.Parent == this)) {
                    child.Parent = null;
                    child.PropertyChanged -= this.Child_PropertyChanged;
                }
            }
        }
    }
}